using System.Collections.Generic;
using NoCodeKit.EditorTools;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 27차시 — 배경과 소리, 그리고 완성.
    ///
    /// 교안 `27_clay_polish_nocode.md` 의 실습 ① ~ ③ 을 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ① 하늘 바꾸기 (절차적 하늘로 노을)</item>
    /// <item>실습 ② 조명으로 분위기 (해를 눕히고 주황으로)</item>
    /// <item>실습 ③ 소리 연결 — <b>소리 파일 없이 배선만</b></item>
    /// </list>
    ///
    /// <b>소리 파일은 아직 없습니다.</b> 그래도 `Audio Source` 를 붙이고 이벤트까지 이어둡니다.
    /// 파일이 들어오면 <c>Audio Clip</c> 칸에 끼우기만 하면 소리가 납니다.
    /// 자세한 것은 `plans/11_폴리싱_대기_목록.md` 를 보세요.
    /// </summary>
    public static class Build27ClayPolish
    {
        const string FromScene = "26_완성";
        const string StartScene = "27_시작";
        const string CompleteScene = "27_완성";

        [MenuItem("Tools/교안 씬 빌더/27차시 — 배경과 소리, 그리고 완성", false, 27)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "27차시 씬 만들기",
                    $"{FromScene} 을 복제해 {StartScene} · {CompleteScene} 을 만듭니다.\n" +
                    "이미 있으면 덮어씁니다.\n\n" +
                    "저장 안 된 작업이 있으면 먼저 저장해주세요.",
                    "만들기", "그만두기"))
            {
                return;
            }

            Run();
        }

        /// <summary>확인 창 없이 바로 만듭니다. 「사격장 전체 다시 만들기」가 부릅니다.</summary>
        public static void Run()
        {
            BuildKit.BeginFrom(FromScene, StartScene);

            BuildSky();
            BuildLight();
            BuildSounds();

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ① — 하늘 바꾸기
        // ══════════════════════════════════════════════════════

        static void BuildSky()
        {
            // 사진 파일 없이도 됩니다. 유니티에 들어 있는 하늘을 노을빛으로 맞춥니다.
            Material sky = BuildKit.SkyboxMat(
                "Sky_Sunset",
                tint: new Color(0.85f, 0.62f, 0.48f),
                ground: new Color(0.28f, 0.24f, 0.20f),
                thickness: 1.6f,
                exposure: 1.15f);

            if (sky == null) return;

            RenderSettings.skybox = sky;
            DynamicGI.UpdateEnvironment();   // 하늘빛이 바닥에도 반영되게
        }

        // ══════════════════════════════════════════════════════
        //  실습 ② — 조명으로 분위기 만들기
        // ══════════════════════════════════════════════════════

        static void BuildLight()
        {
            Light sun = null;
            foreach (Light l in Object.FindObjectsByType<Light>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (l.type == LightType.Directional) { sun = l; break; }
            }
            if (sun == null) return;

            // 교안 표의 2번 — 노을. 해를 낮게 눕히면 그림자가 길어집니다.
            sun.transform.rotation = Quaternion.Euler(15f, -30f, 0f);
            sun.color = new Color(1f, 0.78f, 0.55f);
            sun.intensity = 1f;
            EditorUtility.SetDirty(sun);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ — 소리 연결하기
        // ══════════════════════════════════════════════════════

        static void BuildSounds()
        {
            SimpleGun gun = Object.FindFirstObjectByType<SimpleGun>();
            if (gun == null)
            {
                throw new System.InvalidOperationException(
                    "총이 없습니다. 24차시부터 다시 만들어주세요.");
            }

            // 1단계 — 총소리
            AudioSource shot = AddSource(gun.gameObject, playOnAwake: false, loop: false, volume: 0.6f);
            BuildKit.Wire(gun.onFired, shot.Play);

            // 2단계 — 명중음. 틀은 씬 안의 것을 가리킬 수 없어서 소리도 틀 안에 넣습니다 (25차시 제약).
            BuildKit.EditPrefab("Target", root =>
            {
                AudioSource hit = AddSource(root, playOnAwake: false, loop: false, volume: 0.8f);

                var hittable = root.GetComponent<Hittable>();
                if (hittable != null) BuildKit.Wire(hittable.onHit, hit.Play);
            });

            // 3단계 — 배경음악
            GameObject bgm = BuildKit.Empty("BGM");
            AddSource(bgm, playOnAwake: true, loop: true, volume: 0.3f);
        }

        static AudioSource AddSource(GameObject go, bool playOnAwake, bool loop, float volume)
        {
            var source = go.AddComponent<AudioSource>();
            source.clip = null;                 // 소리 파일이 들어오면 여기에 끼웁니다
            source.playOnAwake = playOnAwake;
            source.loop = loop;
            source.volume = volume;
            EditorUtility.SetDirty(source);
            return source;
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 27차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다. **사격장 갈래가 여기서 끝납니다.**\n" +
                    "▶ 를 누르고 난이도 → 시작 → 사격까지 통째로 해보세요.\n" +
                    "\n" +
                    "※ 소리 파일이 아직 없어서 Audio Clip 칸이 비어 있습니다.\n" +
                    "  배선은 다 돼 있으니 파일이 들어오면 칸에 끼우기만 하면 됩니다.\n" +
                    "  남은 폴리싱 항목은 plans/11_폴리싱_대기_목록.md 에 모아뒀습니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 27차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
