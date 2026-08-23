using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 차시별 씬 빌더가 공통으로 쓰는 도구.
    ///
    /// <b>이 폴더는 수강생에게 배포하지 않습니다.</b> 정답 씬 생성기이기 때문입니다.
    ///
    /// 씬을 만드는 방식은 하나뿐입니다 — <c>plans/10</c> §1.
    /// <code>
    /// 이전 차시 완성 씬 복제 → NN_시작 저장 → 이번 차시 것만 얹기 → NN_완성 저장
    /// </code>
    /// </summary>
    public static class BuildKit
    {
        public const string SceneDir = "Assets/_NoCodeKit/Scenes";
        public const string PrefabFolderName = "Prefabs";
        public const string MaterialDir = "Assets/_NoCodeKit/Materials";

        // ══════════════════════════════════════════════════════
        //  씬
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 씬은 <b>차시 번호 폴더 아래</b>에 둡니다.
        /// 예) <c>22_완성</c> → <c>Assets/_NoCodeKit/Scenes/22/22_완성.unity</c>
        /// </summary>
        public static string ScenePath(string sceneName)
        {
            return $"{SceneFolder(sceneName)}/{sceneName}.unity";
        }

        /// <summary>그 씬이 들어갈 차시 폴더입니다.</summary>
        public static string SceneFolder(string sceneName)
        {
            int cut = sceneName.IndexOf('_');
            string lesson = cut > 0 ? sceneName.Substring(0, cut) : sceneName;
            return $"{SceneDir}/{lesson}";
        }

        /// <summary>빈 씬에서 시작합니다. 13 · 22 · 28차시만 씁니다.</summary>
        public static Scene BeginFromEmpty(string startSceneName)
        {
            EnsureFolder(SceneFolder(startSceneName));
            Scene scene = EditorSceneManager.NewScene(
                NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, ScenePath(startSceneName));
            return scene;
        }

        /// <summary>
        /// 앞 차시 완성 씬을 복제해 이번 차시 시작 씬으로 만들고 엽니다.
        /// 14 ~ 21 · 23 ~ 27 · 29 ~ 31 · 32차시가 씁니다.
        /// </summary>
        public static Scene BeginFrom(string sourceCompleteScene, string startSceneName)
        {
            EnsureFolder(SceneFolder(startSceneName));

            string from = ScenePath(sourceCompleteScene);
            string to = ScenePath(startSceneName);

            if (!File.Exists(from))
            {
                throw new FileNotFoundException(
                    $"앞 차시 완성 씬이 없습니다: {from}\n" +
                    "갈래 안에서는 차시 순서를 건너뛸 수 없습니다. 앞 차시부터 만들어주세요.");
            }

            if (File.Exists(to)) AssetDatabase.DeleteAsset(to);
            if (!AssetDatabase.CopyAsset(from, to))
            {
                throw new IOException($"씬 복제 실패: {from} → {to}");
            }

            AssetDatabase.Refresh();
            return EditorSceneManager.OpenScene(to, OpenSceneMode.Single);
        }

        /// <summary>이번 차시 완성 씬으로 저장합니다.</summary>
        public static void SaveComplete(string completeSceneName)
        {
            EnsureFolder(SceneFolder(completeSceneName));

            Scene scene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath(completeSceneName));
            AssetDatabase.SaveAssets();
        }

        // ══════════════════════════════════════════════════════
        //  오브젝트
        // ══════════════════════════════════════════════════════

        /// <summary>빈 그릇을 만듭니다.</summary>
        public static GameObject Empty(string name, Transform parent = null)
        {
            var go = new GameObject(name);
            Place(go, parent);
            return go;
        }

        /// <summary>기본 도형을 만듭니다.</summary>
        public static GameObject Shape(string name, PrimitiveType type, Transform parent = null)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            Place(go, parent);
            return go;
        }

        /// <summary>
        /// 납작한 그림(<c>SpriteRenderer</c>)을 하나 만듭니다.
        /// 에디터의 <c>2D Object ▸ Sprites ▸ Square</c> 와 같은 것입니다.
        ///
        /// 16차시에서 <b>입체 · 납작한 그림 · 효과</b> 셋을 견주는 데 씁니다.
        /// 그림 파일을 안 넣으면 <b>유니티 내장 흰 사각형</b>이 나옵니다. 교안이 그 상태를 전제합니다.
        /// </summary>
        /// <param name="worldSize">
        /// <b>화면에서 몇 칸으로 보이게 할지.</b>
        ///
        /// 내장 스프라이트는 <b>그냥 두면 아주 작습니다.</b>
        /// 텍스처가 작은 데다 <c>Pixels Per Unit</c> 이 100 이라 <b>0.2칸도 안 됩니다.</b>
        /// 옆에 1칸짜리 큐브를 놓고 견주는 차시라 <b>점처럼 보여서 비교가 안 됩니다.</b>
        ///
        /// 그래서 <b>실제 크기를 재서</b> 원하는 칸 수에 맞춥니다.
        /// 그림 파일을 바꿔 끼워도 크기가 그대로라 안전합니다.
        /// </param>
        public static SpriteRenderer Sprite2D(string name, Transform parent = null,
                                              float worldSize = 1f)
        {
            var go = new GameObject(name);
            var sr = go.AddComponent<SpriteRenderer>();

            // 내장 사각형 스프라이트. 없더라도 빌드는 계속됩니다.
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            sr.color = Color.white;

            if (sr.sprite != null && worldSize > 0f)
            {
                Vector3 size = sr.sprite.bounds.size;
                float longest = Mathf.Max(size.x, size.y);

                if (longest > 0.0001f)
                {
                    go.transform.localScale = Vector3.one * (worldSize / longest);
                }
            }

            Place(go, parent);
            EditorUtility.SetDirty(sr);
            return sr;
        }

        /// <summary>
        /// 흩날리는 효과(<c>ParticleSystem</c>)를 하나 만듭니다.
        ///
        /// <paramref name="loop"/> 를 끄면 <b>한 번만 터지고 스스로 사라집니다.</b>
        /// (16차시는 계속 뿜어야 하므로 켜고, 25차시 명중 효과는 끕니다)
        /// </summary>
        public static ParticleSystem Particle(string name, Color color, Transform parent = null,
                                              bool loop = true, float lifeTime = 1f,
                                              float startSpeed = 3f, float rate = 30f)
        {
            var go = new GameObject(name);
            var ps = go.AddComponent<ParticleSystem>();

            ParticleSystem.MainModule main = ps.main;
            main.duration = 1f;
            main.loop = loop;
            main.playOnAwake = true;
            main.startLifetime = lifeTime;
            main.startSpeed = startSpeed;
            main.startSize = 0.15f;
            main.startColor = color;
            main.stopAction = loop ? ParticleSystemStopAction.None
                                   : ParticleSystemStopAction.Destroy;

            ParticleSystem.EmissionModule emission = ps.emission;
            emission.rateOverTime = rate;

            // 재질을 안 주면 분홍색으로 나옵니다 (URP 기본 파티클 셰이더가 아니라서)
            Material mat = ParticleMat($"{name}Mat", color);
            if (mat != null) go.GetComponent<ParticleSystemRenderer>().sharedMaterial = mat;

            Place(go, parent);
            EditorUtility.SetDirty(ps);
            return ps;
        }

        static void Place(GameObject go, Transform parent)
        {
            if (parent != null) go.transform.SetParent(parent, false);
            Undo.RegisterCreatedObjectUndo(go, $"{go.name} 만들기");
            EditorUtility.SetDirty(go);
        }

        /// <summary>
        /// 부품을 붙입니다. <b>반드시 이걸로 붙이세요.</b> <c>AddComponent</c> 를 직접 쓰면 안 됩니다.
        ///
        /// 이유 — 부품의 이벤트 칸(<c>public UnityEvent onGameStart;</c>)은 초기값이 없습니다.
        /// Inspector 로 붙일 때는 유니티가 직렬화하면서 채워주지만,
        /// <b>코드로 붙이면 빈 채로 남아서</b> 이벤트를 걸 때 터집니다.
        /// 여기서 미리 만들어둡니다.
        /// </summary>
        public static T Add<T>(GameObject go) where T : Component
        {
            var c = go.AddComponent<T>();
            InitEventFields(c);
            EditorUtility.SetDirty(c);
            return c;
        }

        /// <summary>비어 있는 이벤트 칸을 만들어둡니다.</summary>
        public static void InitEventFields(Component c)
        {
            if (c == null) return;

            foreach (FieldInfo f in c.GetType().GetFields(
                         BindingFlags.Public | BindingFlags.Instance))
            {
                if (!typeof(UnityEventBase).IsAssignableFrom(f.FieldType)) continue;
                if (f.GetValue(c) != null) continue;

                f.SetValue(c, Activator.CreateInstance(f.FieldType));
            }
        }

        /// <summary>
        /// 부품이 <b>이미 있으면 그걸 쓰고, 없으면 붙입니다.</b>
        ///
        /// 🚨 <b>절대 <c>??</c> 로 쓰지 마세요.</b>
        /// <code>
        /// var box = go.GetComponent&lt;BoxCollider&gt;() ?? go.AddComponent&lt;BoxCollider&gt;();  // ❌
        /// </code>
        ///
        /// 유니티는 <b>«없는 부품» 을 null 처럼 보이게 하려고 <c>==</c> 를 다시 만들어뒀습니다.</b>
        /// 그런데 <c>??</c> 는 <b>그 <c>==</c> 를 건너뛰고 C# 의 진짜 null 만</b> 봅니다.
        ///
        /// 그래서 «없는 부품» 을 <b>있다고 판단해 그대로 돌려주고</b>,
        /// 값을 넣는 순간 <c>MissingComponentException</c> 으로 터집니다. 실제로 겪었습니다.
        ///
        /// <c>?.</c> 도 같은 이유로 위험합니다. <b>반드시 <c>== null</c> 로 물어보세요.</b>
        /// </summary>
        public static T Ensure<T>(GameObject go) where T : Component
        {
            if (go == null) return null;

            T found = go.GetComponent<T>();
            if (found != null) return found;   // 유니티가 다시 만든 == 를 씁니다

            return Add<T>(go);
        }

        /// <summary>씬에 그 이름의 빈 그릇이 있으면 쓰고, 없으면 만듭니다.</summary>
        public static GameObject EnsureEmpty(string name)
        {
            GameObject found = GameObject.Find(name);
            return found != null ? found : Empty(name);
        }

        /// <summary>
        /// <b>세상 안에 놓는 글자</b>를 만듭니다. 화면에 붙이는 글자(UGUI)가 아닙니다.
        ///
        /// VR 에서 3D 버튼에 이름을 붙이는 데 씁니다.
        /// 글자가 없으면 <b>색깔 상자만 늘어서 있어</b> 어느 게 무슨 버튼인지 알 수 없습니다.
        ///
        /// 크기는 <b>스스로 맞추게</b> 둡니다. 세상 글자는 점 단위가 아니라 미터 단위라
        /// 숫자를 손으로 넣으면 매번 어긋납니다.
        /// </summary>
        public static TextMeshPro Label3D(string name, string text, Transform parent,
                                          Vector2 size, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var tmp = go.AddComponent<TextMeshPro>();
            tmp.text = text;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;

            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 0.02f;
            tmp.fontSizeMax = 3f;

            TMP_FontAsset font = BuildKitUi.KoreanFont();
            if (font != null) tmp.font = font;

            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = size;

            Undo.RegisterCreatedObjectUndo(go, $"{name} 만들기");
            EditorUtility.SetDirty(go);
            return tmp;
        }

        /// <summary>
        /// 이벤트 칸에 <b>무엇이 어떻게 걸려 있는지</b>를 사람이 읽을 수 있게 돌려줍니다.
        /// 빌더가 «제대로 걸렸나» 를 로그로 보여줄 때 씁니다.
        /// </summary>
        public static string Describe(Object owner, string serializedField)
        {
            if (owner == null) return "(대상 없음)";

            var so = new SerializedObject(owner);
            SerializedProperty calls =
                so.FindProperty(serializedField + ".m_PersistentCalls.m_Calls");

            if (calls == null) return $"(칸을 못 찾음: {serializedField})";
            if (calls.arraySize == 0) return "(연결 없음)";

            SerializedProperty c = calls.GetArrayElementAtIndex(0);
            Object target = c.FindPropertyRelative("m_Target").objectReferenceValue;
            string method = c.FindPropertyRelative("m_MethodName").stringValue;
            int mode = c.FindPropertyRelative("m_Mode").enumValueIndex;

            return $"{(target != null ? target.name : "없음")}.{method}  (m_Mode={mode})";
        }

        /// <summary>위치 · 회전 · 크기를 한 번에 정합니다.</summary>
        public static GameObject At(this GameObject go,
                                    Vector3? pos = null,
                                    Vector3? rot = null,
                                    Vector3? scale = null)
        {
            Transform t = go.transform;
            if (pos.HasValue) t.localPosition = pos.Value;
            if (rot.HasValue) t.localEulerAngles = rot.Value;
            if (scale.HasValue) t.localScale = scale.Value;
            EditorUtility.SetDirty(go);
            return go;
        }

        // ══════════════════════════════════════════════════════
        //  이름표 (Tag)
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 이름표를 등록합니다. 이미 있으면 아무 일도 안 합니다.
        /// <b>그 이름표를 쓰는 것을 만들기 전에</b> 불러야 합니다.
        /// 나중에 하면 틀을 다시 열어 하나씩 지정해야 합니다.
        /// </summary>
        /// <summary>
        /// <b>틀에서 찍어낸 것</b>을 고쳤을 때, 그 변경을 «틀과 다른 부분»으로 기록합니다.
        ///
        /// <b>왜 필요한가</b> — 코드로 값을 바꾸면 <b>메모리에서만 바뀌고 씬에 안 남습니다.</b>
        /// 틀에서 찍어낸 것은 «틀과 뭐가 다른지» 목록으로 저장되는데,
        /// C# 으로 값을 대입하는 것만으로는 <b>그 목록에 안 올라갑니다.</b>
        ///
        /// 실제로 겪었습니다 — 29차시에서 눈높이를 <c>1.6</c> 으로 넣었는데
        /// 저장하고 열어보니 <b>틀의 기본값(<c>1.36144</c>)으로 되돌아가 있었습니다.</b>
        ///
        /// <b>틀에서 찍어낸 것의 값을 고쳤으면 반드시 이걸 부르세요.</b>
        /// 틀이 아닌 것에 부르면 그냥 «바뀌었다» 표시만 하고 넘어갑니다.
        /// </summary>
        public static void RecordPrefabChange(Component component)
        {
            if (component == null) return;

            EditorUtility.SetDirty(component);

            if (PrefabUtility.IsPartOfPrefabInstance(component))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(component);
            }
        }

        /// <summary>
        /// <b>강의자료 바깥의 틀</b>을 이름으로 찾아 씬에 놓습니다. XRI 샘플 프리팹용입니다.
        ///
        /// <b>왜 이름으로 찾나</b> — 교안이 적어둔 경로에 <b>패키지 버전이 박혀 있습니다.</b>
        /// <c>Samples/XR Interaction Toolkit/<b>3.3.2</b>/Starter Assets/Prefabs/...</c>
        /// 버전이 올라가면 폴더 이름이 바뀌어 <b>경로가 통째로 깨집니다.</b>
        /// 이름으로 찾으면 버전이 바뀌어도 그대로 잡힙니다.
        /// </summary>
        public static GameObject PlaceSamplePrefab(string prefabName, Vector3 position)
        {
            string[] hits = AssetDatabase.FindAssets($"\"{prefabName}\" t:Prefab");

            foreach (string guid in hits)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) != prefabName) continue;

                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (asset == null) continue;

                return PlaceFromPrefab(asset, position, prefabName);
            }

            throw new IOException(
                $"틀을 못 찾았습니다: {prefabName}\n" +
                "XR Interaction Toolkit 의 예제 꾸러미(Starter Assets · XR Interaction Simulator)를\n" +
                "Package Manager 에서 가져왔는지 확인해주세요. (28차시 실습 ② 2단계)");
        }

        /// <summary>
        /// XR <b>상호작용 이름표</b>(Interaction Layer)를 등록하고 <b>그 번호</b>를 돌려줍니다.
        /// 이미 있으면 있는 번호를 그대로 씁니다. 못 넣으면 <c>-1</c>.
        ///
        /// <b>왜 필요한가</b> — 31차시가 순간이동 바닥의 <c>Interaction Layer Mask</c> 를
        /// <b><c>Teleport</c> 로 바꾸라</b>고 하는데, <b>이 프로젝트에는 그 이름표가 없습니다.</b>
        /// 목록에 `Default` 하나뿐이라 <b>고르고 싶어도 고를 수가 없습니다.</b>
        ///
        /// 21차시의 이름표(Tag)와 같은 구조입니다 —
        /// <b>만들어야 고를 수 있고, 골라야 반응합니다.</b>
        ///
        /// <b>0번은 안 건드립니다.</b> 거기는 <c>Default</c> 자리입니다.
        /// </summary>
        public static int EnsureInteractionLayer(string layerName)
        {
            if (string.IsNullOrEmpty(layerName)) return -1;

            var settings = Resources.Load<ScriptableObject>("InteractionLayerSettings");
            if (settings == null)
            {
                Debug.LogError(
                    "XR 상호작용 이름표 설정을 못 찾았습니다.\n" +
                    "Assets/XRI/Settings/Resources/InteractionLayerSettings.asset 이 있는지 확인해주세요.");
                return -1;
            }

            var so = new SerializedObject(settings);
            SerializedProperty names = so.FindProperty("m_LayerNames");

            if (names == null || !names.isArray)
            {
                Debug.LogError("상호작용 이름표 목록을 못 읽었습니다.");
                return -1;
            }

            for (int i = 0; i < names.arraySize; i++)
            {
                if (names.GetArrayElementAtIndex(i).stringValue == layerName) return i;
            }

            for (int i = 1; i < names.arraySize; i++)
            {
                SerializedProperty slot = names.GetArrayElementAtIndex(i);
                if (!string.IsNullOrEmpty(slot.stringValue)) continue;

                slot.stringValue = layerName;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                return i;
            }

            Debug.LogError($"상호작용 이름표 자리가 다 찼습니다. '{layerName}' 을 못 넣었습니다.");
            return -1;
        }

        public static void EnsureTag(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return;
            if (InternalEditorUtility.tags.Contains(tag)) return;

            Object[] assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if (assets == null || assets.Length == 0)
            {
                Debug.LogError("TagManager.asset 을 못 찾았습니다.");
                return;
            }

            var manager = new SerializedObject(assets[0]);
            SerializedProperty tags = manager.FindProperty("tags");

            tags.InsertArrayElementAtIndex(tags.arraySize);
            tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
            manager.ApplyModifiedProperties();

            Debug.Log($"이름표 '{tag}' 를 등록했습니다.");
        }

        // ══════════════════════════════════════════════════════
        //  재질 · 틀
        // ══════════════════════════════════════════════════════

        /// <summary>URP 재질을 만듭니다. 이미 있으면 색만 바꿔 다시 씁니다.</summary>
        public static Material Mat(string name, Color color)
        {
            EnsureFolder(MaterialDir);
            string path = $"{MaterialDir}/{name}.mat";

            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
            {
                existing.color = color;
                EditorUtility.SetDirty(existing);
                return existing;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                // URP 가 아니면 분홍색이 됩니다. 그냥 두지 않고 알려줍니다.
                Debug.LogError("URP/Lit 셰이더를 못 찾았습니다. 이 프로젝트는 URP 여야 합니다.");
                shader = Shader.Find("Standard");
            }

            var mat = new Material(shader) { color = color };
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        /// <summary>
        /// 부딪히는 부품을 뗍니다. <b>눈에만 보이면 되는 것</b>에 씁니다.
        ///
        /// 기본 도형은 부딪히는 부품을 달고 태어납니다. 총처럼 장식으로만 쓰는 것에 남겨두면
        /// <b>총알이 나오자마자 총에 맞고 사라집니다.</b>
        /// </summary>
        /// <summary>
        /// 흩날리는 효과(Particle)용 재질입니다.
        ///
        /// 코드로 <c>ParticleSystem</c> 을 붙이면 <b>옛 방식 재질</b>이 딸려 와서
        /// URP 에서는 <b>분홍색 네모</b>로 나옵니다. 그래서 따로 만들어 끼웁니다.
        /// </summary>
        public static Material ParticleMat(string name, Color color)
        {
            EnsureFolder(MaterialDir);
            string path = $"{MaterialDir}/{name}.mat";

            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;

            Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            if (shader == null)
            {
                Debug.LogError("URP 파티클 셰이더를 못 찾았습니다. 효과가 분홍색으로 나올 수 있습니다.");
                return null;
            }

            var mat = new Material(shader);
            mat.SetColor("_BaseColor", color);
            AssetDatabase.CreateAsset(mat, path);
            return mat;
        }

        /// <summary>
        /// 하늘 재료를 만듭니다. 유니티에 들어 있는 <b>절차적 하늘</b>을 씁니다.
        /// 사진 파일이 없어도 노을·한낮 같은 분위기를 낼 수 있습니다.
        /// </summary>
        public static Material SkyboxMat(string name, Color tint, Color ground,
                                         float thickness, float exposure)
        {
            EnsureFolder(MaterialDir);
            string path = $"{MaterialDir}/{name}.mat";

            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Shader shader = Shader.Find("Skybox/Procedural");
                if (shader == null)
                {
                    Debug.LogError("하늘 셰이더를 못 찾았습니다.");
                    return null;
                }
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }

            mat.SetColor("_SkyTint", tint);
            mat.SetColor("_GroundColor", ground);
            mat.SetFloat("_AtmosphereThickness", thickness);
            mat.SetFloat("_Exposure", exposure);
            EditorUtility.SetDirty(mat);
            return mat;
        }

        public static GameObject NoCollider(this GameObject go)
        {
            foreach (Collider c in go.GetComponentsInChildren<Collider>(true))
            {
                Object.DestroyImmediate(c);
            }
            EditorUtility.SetDirty(go);
            return go;
        }

        public static GameObject Paint(this GameObject go, Material mat)
        {
            var r = go.GetComponent<Renderer>();
            if (r != null)
            {
                r.sharedMaterial = mat;
                EditorUtility.SetDirty(r);
            }
            return go;
        }

        /// <summary>
        /// 지금 열려 있는 씬이 속한 <b>차시 폴더 안의 틀 폴더</b>입니다.
        /// 예) <c>Scenes/23/23_시작.unity</c> 를 열고 있으면 → <c>Scenes/23/Prefabs</c>
        ///
        /// 차시별로 나눠 두면 <b>어느 차시가 무엇을 만들었는지</b> 폴더만 봐도 보입니다.
        /// (재질은 나누지 않습니다 — 색은 여러 차시가 같이 쓰는 일이 많습니다)
        /// </summary>
        public static string CurrentPrefabDir()
        {
            Scene scene = EditorSceneManager.GetActiveScene();

            if (string.IsNullOrEmpty(scene.path))
            {
                Debug.LogWarning("저장 안 된 씬입니다. 틀을 어느 차시 폴더에 둘지 알 수 없습니다.");
                return $"{SceneDir}/{PrefabFolderName}";
            }

            string dir = Path.GetDirectoryName(scene.path)
                             .Replace(Path.DirectorySeparatorChar, '/');
            return $"{dir}/{PrefabFolderName}";
        }

        /// <summary>씬의 것을 틀(Prefab)로 저장합니다.</summary>
        /// <param name="connect">
        /// <b>씬에 있던 것을 그 틀의 «찍어낸 것»으로 바꿀지.</b>
        ///
        /// 15차시 실습 ③ 4번이 이걸 합니다 — *"`Car` 이름이 파란색으로 변했습니다"*.
        /// <b>연결을 안 하면 이미 걸어둔 이벤트가 끊깁니다.</b>
        /// 14차시 버튼이 씬의 그 자동차를 가리키고 있어서, 지우고 새로 놓으면 칸이 <c>None</c> 이 됩니다.
        ///
        /// 23 · 24차시처럼 <b>씬에서 치우고 재료함에만 둘</b> 때는 <c>false</c> 로 둡니다.
        /// </param>
        public static GameObject SaveAsPrefab(GameObject source, string prefabName,
                                              bool removeFromScene = true,
                                              bool connect = false)
        {
            string dir = CurrentPrefabDir();
            EnsureFolder(dir);
            string path = $"{dir}/{prefabName}.prefab";

            GameObject prefab = connect
                ? PrefabUtility.SaveAsPrefabAssetAndConnect(
                      source, path, InteractionMode.AutomatedAction, out bool okc)
                : PrefabUtility.SaveAsPrefabAsset(source, path, out okc);

            // 조용히 실패하면 씬만 덩그러니 남습니다. 여기서 멈추는 편이 낫습니다.
            if (!okc || prefab == null)
            {
                throw new IOException($"틀(Prefab) 저장 실패: {path}");
            }

            if (removeFromScene && !connect) Object.DestroyImmediate(source);
            return prefab;
        }

        /// <summary>
        /// 틀에서 하나 찍어내 씬에 놓습니다. 15차시 실습 ③ 2단계의 <b>"다섯 대 놓기"</b> 입니다.
        ///
        /// <c>Instantiate</c> 가 아니라 <c>PrefabUtility.InstantiatePrefab</c> 을 씁니다.
        /// 그래야 <b>틀에 연결된 채로</b>(파란 이름) 놓입니다.
        /// </summary>
        public static GameObject PlaceFromPrefab(GameObject prefab, Vector3 position,
                                                 string name = null)
        {
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            go.transform.position = position;
            if (!string.IsNullOrEmpty(name)) go.name = name;

            Undo.RegisterCreatedObjectUndo(go, $"{go.name} 놓기");
            EditorUtility.SetDirty(go);
            return go;
        }

        /// <summary>
        /// 이미 만들어둔 틀을 열어 고칩니다. 15차시의 <b>"틀 고치는 방"</b> 을 코드로 하는 것입니다.
        ///
        /// 25차시가 23차시의 접시 틀에 <c>Hittable</c> 을 붙이는 것처럼,
        /// <b>뒤 차시가 앞 차시의 틀을 고치는</b> 일이 자주 있습니다.
        /// </summary>
        public static void EditPrefab(string prefabName, System.Action<GameObject> edit)
        {
            GameObject asset = LoadPrefab(prefabName);
            if (asset == null)
            {
                throw new IOException(
                    $"틀을 못 찾았습니다: {prefabName}\n" +
                    "이 틀을 만드는 앞 차시부터 다시 만들어주세요.");
            }

            string path = AssetDatabase.GetAssetPath(asset);
            GameObject root = PrefabUtility.LoadPrefabContents(path);

            try
            {
                edit(root);
                PrefabUtility.SaveAsPrefabAsset(root, path, out bool ok);
                if (!ok) throw new IOException($"틀 저장 실패: {path}");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        public static GameObject LoadPrefab(string prefabName)
        {
            // 틀은 그것을 만든 차시 폴더에 있습니다. 23차시의 접시를 24차시가 그대로 쓰므로
            // 지금 차시만 보면 안 되고, 씬 폴더 전체에서 찾아야 합니다.
            foreach (string guid in AssetDatabase.FindAssets($"{prefabName} t:Prefab",
                                                             new[] { SceneDir }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == prefabName)
                {
                    return AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }
            }

            Debug.LogWarning($"틀을 못 찾았습니다: {prefabName}");
            return null;
        }

        // ══════════════════════════════════════════════════════
        //  이벤트 칸 연결  ← 손으로 하면 가장 오래 걸리는 곳
        // ══════════════════════════════════════════════════════

        /// <summary>인자 없는 동작을 겁니다. 예: On Game Start ▸ MouseLook.LockCursor</summary>
        public static void Wire(UnityEvent target, UnityAction call)
        {
            if (!Ready(target, call)) return;
            UnityEventTools.AddPersistentListener(target, call);
            MarkOwnerDirty(call);
        }

        /// <summary>
        /// <b>바뀌는 값을 넘겨주는</b> 연결입니다. 인스펙터 목록의 <b>위쪽 Dynamic</b> 쪽입니다.
        /// 예: On Score Changed ▸ ValueDisplay.SetValue
        ///
        /// 손으로 하면 아래쪽 Static 을 고르기 쉬운데, 여기서는 틀릴 수가 없습니다.
        /// </summary>
        public static void WireDynamic<T>(UnityEvent<T> target, UnityAction<T> call)
        {
            if (!Ready(target, call)) return;
            UnityEventTools.AddPersistentListener(target, call);
            MarkOwnerDirty(call);
        }

        /// <summary>
        /// <b>값을 넣는 칸</b>에 잇습니다. 예: On Difficulty Changed ▸ 글자의 <c>text</c>
        ///
        /// 인스펙터에서 목록 위쪽의 <c>text</c> 를 고르는 것과 같습니다.
        /// 칸(프로퍼티)은 동작이 아니라서 그냥은 못 잇고, 안쪽 <c>set_text</c> 를 찾아 겁니다.
        /// </summary>
        public static void WireDynamicProperty<T>(UnityEvent<T> target, Object owner,
                                                  string propertyName)
        {
            if (target == null || owner == null)
            {
                Debug.LogError("이벤트 칸이나 대상이 비어 있어서 연결할 수 없습니다.");
                return;
            }

            PropertyInfo prop = owner.GetType().GetProperty(
                propertyName, BindingFlags.Public | BindingFlags.Instance);
            MethodInfo setter = prop?.GetSetMethod();

            if (setter == null)
            {
                Debug.LogError($"{owner.GetType().Name} 에 값을 넣을 수 있는 '{propertyName}' 칸이 없습니다.");
                return;
            }

            var call = (UnityAction<T>)Delegate.CreateDelegate(
                typeof(UnityAction<T>), owner, setter);

            UnityEventTools.AddPersistentListener(target, call);
            EditorUtility.SetDirty(owner);
        }

        /// <summary>
        /// 이벤트 칸에서 <b>대상이 사라진 줄</b>을 지웁니다. 지운 개수를 돌려줍니다.
        ///
        /// <b>왜 필요한가</b> — 32차시가 <c>Player</c> 를 통째로 지웁니다.
        /// 그런데 26차시가 걸어둔 <c>On Game Start</c> 에는
        /// 그 안의 <c>MouseLook.LockCursor</c> · <c>PlayerMover.EnableMove</c> 가 들어 있습니다.
        ///
        /// 지우고 나면 그 줄들이 <b>«대상 없음»</b> 으로 남습니다.
        /// 돌아가는 데는 지장이 없지만 <b>점검기가 «끊어진 연결» 로 짚고</b>,
        /// 정답 씬으로 건네기에도 지저분합니다.
        /// </summary>
        public static int DropBrokenListeners(UnityEventBase target)
        {
            if (target == null) return 0;

            int removed = 0;

            // 뒤에서부터 지웁니다. 앞에서 지우면 뒤 번호가 밀립니다.
            for (int i = target.GetPersistentEventCount() - 1; i >= 0; i--)
            {
                if (target.GetPersistentTarget(i) != null) continue;

                UnityEventTools.RemovePersistentListener(target, i);
                removed++;
            }

            return removed;
        }

        /// <summary>
        /// <b>인자를 받는 이벤트 칸</b>에 <b>인자 없는 동작</b>을 겁니다.
        ///
        /// XRI 의 <c>Activated</c>(썼다) · <c>Select Entered</c>(잡았다) 가
        /// «무엇이 어떻게 잡혔는지» 를 같이 넘겨주는 칸이라 <see cref="Wire"/> 로는 안 걸립니다.
        ///
        /// 그런데 우리가 부를 <c>SimpleGun.Fire</c> · <c>GameStarter.StartGame</c> 은
        /// <b>받을 게 없는 동작</b>입니다. 인스펙터 목록에서 고르면 그냥 걸리는데,
        /// 코드로는 이 길을 따로 써야 합니다.
        /// </summary>
        public static void WireVoid(UnityEventBase target, UnityAction call)
        {
            if (target == null || call == null)
            {
                Debug.LogError("이벤트 칸이나 동작이 비어 있어서 연결할 수 없습니다.");
                return;
            }

            UnityEventTools.AddVoidPersistentListener(target, call);
        }

        /// <summary>미리 정해둔 참/거짓을 넘깁니다. 예: On Game Start ▸ 패널 SetActive(false)</summary>
        public static void WireBool(UnityEventBase target, UnityAction<bool> call, bool value)
        {
            if (!Ready(target, call)) return;
            UnityEventTools.AddBoolPersistentListener(target, call, value);
            MarkOwnerDirty(call);
        }

        /// <summary>미리 정해둔 숫자를 넘깁니다.</summary>
        public static void WireFloat(UnityEventBase target, UnityAction<float> call, float value)
        {
            if (!Ready(target, call)) return;
            UnityEventTools.AddFloatPersistentListener(target, call, value);
            MarkOwnerDirty(call);
        }

        /// <summary>미리 정해둔 글자를 넘깁니다.</summary>
        public static void WireString(UnityEventBase target, UnityAction<string> call, string value)
        {
            if (!Ready(target, call)) return;
            UnityEventTools.AddStringPersistentListener(target, call, value);
            MarkOwnerDirty(call);
        }

        /// <summary>연결하기 전에 양쪽이 멀쩡한지 봅니다. 여기서 걸러야 원인을 알 수 있습니다.</summary>
        static bool Ready(UnityEventBase target, Delegate call)
        {
            if (target == null)
            {
                Debug.LogError(
                    "이벤트 칸이 비어 있어서 연결할 수 없습니다. "
                    + "부품을 붙일 때 AddComponent 대신 BuildKit.Add<T>() 를 쓰셨는지 확인해주세요.");
                return false;
            }

            if (call?.Target == null)
            {
                Debug.LogError("연결할 대상이 없습니다.");
                return false;
            }

            return true;
        }

        static void MarkOwnerDirty(Delegate call)
        {
            if (call?.Target is Object owner) EditorUtility.SetDirty(owner);
        }

        // ══════════════════════════════════════════════════════
        //  도우미
        // ══════════════════════════════════════════════════════

        public static void EnsureFolder(string assetFolder)
        {
            if (AssetDatabase.IsValidFolder(assetFolder)) return;

            string[] parts = assetFolder.Split('/');
            string built = parts[0];                      // "Assets"

            for (int i = 1; i < parts.Length; i++)
            {
                string next = $"{built}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(built, parts[i]);
                }
                built = next;
            }
        }

        /// <summary>카메라를 보고 싶은 것 쪽으로 맞춥니다. 완성 화면 스크린샷용입니다.</summary>
        public static void AimCamera(Vector3 from, Vector3 lookAt)
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            cam.transform.position = from;
            cam.transform.rotation = Quaternion.LookRotation(lookAt - from);
            EditorUtility.SetDirty(cam.gameObject);
        }
    }
}
