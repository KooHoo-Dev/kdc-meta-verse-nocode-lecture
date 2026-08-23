using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 빌더 기반이 실제로 도는지 확인합니다. 차시 빌더를 쓰기 전에 한 번 돌려보세요.
    ///
    /// 특히 <b>이벤트 칸 연결</b>이 코드로 제대로 걸리는지가 핵심입니다.
    /// 여기가 안 되면 차시 빌더를 아무리 써도 소용없습니다.
    /// </summary>
    public static class BuildKitSelfTest
    {
        [MenuItem("Tools/교안 씬 빌더/기반 자체 시험", false, 1)]
        public static void Run()
        {
            bool ok = EditorUtility.DisplayDialog(
                "빌더 기반 자체 시험",
                "지금 열려 있는 씬을 닫고 시험용 빈 씬을 엽니다.\n" +
                "저장 안 된 작업이 있으면 먼저 저장해주세요.\n\n" +
                "시험이 끝나면 결과가 Console 에 나옵니다.",
                "계속", "그만두기");
            if (!ok) return;

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var log = new StringBuilder();
            int pass = 0, fail = 0;

            void Check(string what, bool good, string detail = "")
            {
                if (good) { pass++; log.Append("  ✅ "); }
                else { fail++; log.Append("  ❌ "); }
                log.Append(what);
                if (!string.IsNullOrEmpty(detail)) log.Append("  —  ").Append(detail);
                log.AppendLine();
            }

            // ── 1. 오브젝트 만들기 · 부모 자식 ──────────────────
            GameObject parent = BuildKit.Empty("시험_부모");
            GameObject child = BuildKit.Shape("시험_자식", PrimitiveType.Cube, parent.transform)
                                       .At(pos: new Vector3(1, 2, 3), scale: Vector3.one * 2);

            Check("빈 그릇 만들기", parent != null);
            Check("도형을 자식으로 만들기", child != null && child.transform.parent == parent.transform);
            Check("위치 · 크기 지정", child.transform.localPosition == new Vector3(1, 2, 3)
                                       && child.transform.localScale == Vector3.one * 2);

            // ── 2. 재질 ────────────────────────────────────────
            Material mat = BuildKit.Mat("시험_재질", Color.cyan);
            child.Paint(mat);

            Check("URP 재질 만들기", mat != null && mat.shader != null,
                  mat != null ? $"셰이더: {mat.shader.name}" : "");
            Check("재질 입히기", child.GetComponent<Renderer>().sharedMaterial == mat);

            // ── 3. 이름표 ──────────────────────────────────────
            // Player 는 유니티 기본 이름표라 등록돼 있어야 합니다. 없는 걸 새로 만들지는 않습니다.
            BuildKit.EnsureTag("Player");
            Check("이름표 확인 (Player)",
                  System.Array.IndexOf(UnityEditorInternal.InternalEditorUtility.tags, "Player") >= 0);

            // ── 4. 🔴 이벤트 칸 연결 ───────────────────────────
            // GameStarter 는 붙는 순간 Collider 를 찾습니다. 먼저 붙여둬야 경고가 안 뜹니다.
            var trigger = parent.AddComponent<BoxCollider>();
            trigger.isTrigger = true;

            // AddComponent 를 직접 쓰지 않습니다. 이벤트 칸이 빈 채로 남습니다.
            var starter = BuildKit.Add<GameStarter>(parent);
            var scorer = BuildKit.Add<HitScorer>(parent);
            var display = BuildKit.Add<ValueDisplay>(child);
            var look = BuildKit.Add<MouseLook>(child);

            Check("이벤트 칸이 준비됐는지",
                  starter.onGameStart != null && starter.onGameEnd != null
                  && scorer.onScoreChanged != null,
                  "부품의 이벤트 칸은 초기값이 없어서, 코드로 붙이면 비어 있습니다");

            // (가) 인자 없는 동작
            BuildKit.Wire(starter.onGameStart, look.LockCursor);

            // (나) 바뀌는 값을 넘기는 연결 — 목록 위쪽 Dynamic
            BuildKit.WireDynamic(scorer.onScoreChanged, display.SetValue);

            // (다) 미리 정해둔 참/거짓
            BuildKit.WireBool(starter.onGameEnd, child.SetActive, false);

            Check("연결 (가) 인자 없는 동작", starter.onGameStart.GetPersistentEventCount() == 1,
                  Describe(starter, "onGameStart", 0));
            Check("연결 (나) 값을 넘기는 연결", scorer.onScoreChanged.GetPersistentEventCount() == 1,
                  Describe(scorer, "onScoreChanged", 0));
            Check("연결 (다) 정해둔 참/거짓", starter.onGameEnd.GetPersistentEventCount() == 1,
                  Describe(starter, "onGameEnd", 0));

            // 값을 넘기는 연결은 반드시 Dynamic(0) 이어야 합니다. 이게 이 강의 최대 함정입니다.
            int dynMode = Mode(scorer, "onScoreChanged", 0);
            Check("값을 넘기는 연결이 Dynamic 인지 (m_Mode = 0)", dynMode == 0,
                  $"m_Mode = {dynMode}");

            // ── 5. 틀 저장 ─────────────────────────────────────
            GameObject prefab = BuildKit.SaveAsPrefab(child, "시험_틀", removeFromScene: false);
            Check("틀(Prefab) 저장", prefab != null);

            // ── 6. 갈래 A(13 ~ 21차시)에서 더 쓰는 것들 ─────────
            //
            //  16차시 납작한 그림 · 효과 / 17차시 늘어나는 앵커 / 18차시 슬라이더.
            //  이 셋이 안 되면 16 · 17 · 18차시 빌더가 통째로 못 돕니다.

            SpriteRenderer sprite = BuildKit.Sprite2D("시험_그림", worldSize: 1.5f);
            Check("납작한 그림 만들기 (16차시)", sprite != null && sprite.sprite != null,
                  sprite != null && sprite.sprite == null ? "내장 스프라이트를 못 찾았습니다" : "");

            // 내장 스프라이트는 그냥 두면 0.2칸도 안 됩니다. 옆 큐브(1칸)와 견주려면 키워야 합니다.
            float spriteSize = sprite != null ? sprite.bounds.size.x : 0f;
            Check("납작한 그림 크기가 맞는지 (16차시)", Mathf.Abs(spriteSize - 1.5f) < 0.05f,
                  $"가로 {spriteSize:0.00}칸 (1.50 이어야 합니다)");

            ParticleSystem particle = BuildKit.Particle("시험_효과", Color.yellow);
            var pr = particle != null ? particle.GetComponent<ParticleSystemRenderer>() : null;
            Check("흩날리는 효과 만들기 (16차시)", particle != null && particle.main.loop);
            Check("효과 재질이 분홍이 아닌지", pr != null && pr.sharedMaterial != null,
                  pr != null && pr.sharedMaterial != null ? pr.sharedMaterial.shader.name : "");

            Canvas canvas = BuildKitUi.Root();

            // 늘어나는 앵커 — anchorMin.x 가 0, anchorMax.x 가 1 이라야 가로로 늘어납니다
            UnityEngine.UI.Image bar =
                BuildKitUi.Box("시험_띠", canvas.transform, Vector2.zero, Vector2.zero, Color.gray)
                          .StretchX(top: true, height: 80f);
            var barRt = bar.GetComponent<RectTransform>();
            Check("가로로 늘어나는 앵커 (17차시)",
                  Mathf.Approximately(barRt.anchorMin.x, 0f)
                  && Mathf.Approximately(barRt.anchorMax.x, 1f)
                  && Mathf.Approximately(barRt.anchorMin.y, 1f),
                  $"min {barRt.anchorMin} / max {barRt.anchorMax}");

            UnityEngine.UI.Image cover =
                BuildKitUi.Box("시험_덮개", canvas.transform, Vector2.zero, Vector2.zero,
                               Color.white, blocksClick: true).Stretch();
            var coverRt = cover.GetComponent<RectTransform>();
            Check("사방으로 늘어나는 앵커 (19차시)",
                  coverRt.anchorMin == Vector2.zero && coverRt.anchorMax == Vector2.one);
            Check("클릭을 가로채는 그림 (19차시 실습 ②)", cover.raycastTarget && !bar.raycastTarget,
                  $"덮개 {cover.raycastTarget} / 띠 {bar.raycastTarget}");

            // 슬라이더 — 세 칸이 다 이어져 있어야 실제로 끌립니다
            UnityEngine.UI.Slider slider = BuildKitUi.Slider(
                "시험_슬라이더", canvas.transform, Vector2.zero, new Vector2(400f, 30f),
                min: 0f, max: 100f, value: 50f, wholeNumbers: true);

            Check("슬라이더 만들기 (18차시)", slider != null
                  && slider.fillRect != null && slider.handleRect != null,
                  slider == null ? "" : $"fill {slider.fillRect != null} / handle {slider.handleRect != null}");
            Check("슬라이더 값 범위", slider != null
                  && Mathf.Approximately(slider.minValue, 0f)
                  && Mathf.Approximately(slider.maxValue, 100f)
                  && Mathf.Approximately(slider.value, 50f),
                  slider == null ? "" : $"{slider.minValue} ~ {slider.maxValue}, 지금 {slider.value}");

            // 18차시의 핵심 — 슬라이더 → 글자 연결이 Dynamic 이어야 합니다
            var scoreText = BuildKitUi.Text("시험_점수", "점수: 0", canvas.transform,
                                            Vector2.zero, new Vector2(300f, 60f));
            var scoreDisplay = BuildKit.Add<ValueDisplay>(scoreText.gameObject);
            BuildKit.WireDynamic(slider.onValueChanged, scoreDisplay.SetValue);

            int sliderMode = Mode(slider, "m_OnValueChanged", 0);
            Check("슬라이더 → 글자 연결이 Dynamic 인지 (18차시 함정)", sliderMode == 0,
                  $"m_Mode = {sliderMode}");

            // ── 7. 화면 묶음 접기 (갈래 A 씬 정리) ──────────────
            //
            //  아홉 차시가 한 판에 쌓이면 Canvas 자식이 열일곱 개가 됩니다.
            //  차시별 그릇에 담아 «안 쓰는 것을 접는» 장치가 제대로 도는지 봅니다.

            string keep = BuildBasicsLayout.UiGroup.L14;
            string fold = BuildBasicsLayout.UiGroup.L16;

            RectTransform kept = BuildKitUi.Group(canvas, keep);
            RectTransform folded = BuildKitUi.Group(canvas, fold);

            Check("화면 묶음 만들기", kept != null && folded != null
                  && kept.parent == canvas.transform);
            Check("묶음이 판 전체를 덮는지",
                  kept != null && kept.anchorMin == Vector2.zero && kept.anchorMax == Vector2.one,
                  kept == null ? "" : $"min {kept.anchorMin} / max {kept.anchorMax}");

            // 같은 이름으로 다시 부르면 새로 만들지 않고 있던 걸 돌려줘야 합니다
            Check("묶음을 두 번 만들지 않는지", BuildKitUi.Group(canvas, keep) == kept);

            BuildKitUi.SetGroupsActive(canvas, BuildBasicsLayout.UiGroup.All, new[] { keep });
            Check("안 쓰는 묶음이 접히는지",
                  kept != null && kept.gameObject.activeSelf
                  && folded != null && !folded.gameObject.activeSelf,
                  kept == null || folded == null ? ""
                      : $"{keep} {kept.gameObject.activeSelf} / {fold} {folded.gameObject.activeSelf}");

            // 접힌 뒤에도 Find 로 다시 잡혀야 뒤 차시 빌더가 이어서 씁니다
            Check("접힌 묶음도 다시 찾아지는지", BuildKitUi.Group(canvas, fold) == folded);

            // ── 뒷정리 ─────────────────────────────────────────
            AssetDatabase.DeleteAsset($"{BuildKit.CurrentPrefabDir()}/시험_틀.prefab");
            AssetDatabase.DeleteAsset($"{BuildKit.MaterialDir}/시험_재질.mat");
            AssetDatabase.DeleteAsset($"{BuildKit.MaterialDir}/시험_효과Mat.mat");

            string head = fail == 0
                ? $"✅ 빌더 기반 자체 시험 — {pass}개 전부 통과했습니다."
                : $"❌ 빌더 기반 자체 시험 — {pass}개 통과 / {fail}개 실패.";

            if (fail == 0) Debug.Log(head + "\n" + log);
            else Debug.LogError(head + "\n" + log);

            Debug.Log(
                "시험용 씬은 저장하지 않았습니다. 그냥 닫으시면 됩니다.\n" +
                "화면의 큐브가 분홍색인 것은 정상입니다 — 시험용 재질을 지웠기 때문입니다.");
        }

        // ── 연결 내용 읽어보기 ────────────────────────────────

        static SerializedProperty Call(Object owner, string eventField, int index)
        {
            var so = new SerializedObject(owner);
            SerializedProperty calls = so.FindProperty(eventField + ".m_PersistentCalls.m_Calls");
            if (calls == null || index >= calls.arraySize) return null;
            return calls.GetArrayElementAtIndex(index);
        }

        static string Describe(Object owner, string eventField, int index)
        {
            SerializedProperty c = Call(owner, eventField, index);
            if (c == null) return "(연결 없음)";

            var target = c.FindPropertyRelative("m_Target").objectReferenceValue;
            string method = c.FindPropertyRelative("m_MethodName").stringValue;
            int mode = c.FindPropertyRelative("m_Mode").enumValueIndex;

            return $"{(target != null ? target.name : "없음")}.{method}  (m_Mode = {mode})";
        }

        static int Mode(Object owner, string eventField, int index)
        {
            SerializedProperty c = Call(owner, eventField, index);
            return c == null ? -1 : c.FindPropertyRelative("m_Mode").enumValueIndex;
        }
    }
}
