using System.Collections.Generic;
using NoCodeKit.EditorTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 19차시 — 안내데스크(EventSystem)와 자동차 조작 패널.
    ///
    /// 교안 `19_ugui_eventsystem_nocode.md` 에서 <b>씬에 남는 것</b>만 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ③ <b>버튼 세 개로 자동차를 조종하는 패널</b> · <c>On Stopped</c> 연결</item>
    /// <item>실습 ④ 그 패널을 <b>틀(Prefab)로</b> — <b>20차시가 이 틀을 다시 씁니다</b></item>
    /// </list>
    ///
    /// <b>실습 ① · ②는 만들지 않습니다.</b>
    /// <list type="bullet">
    /// <item>①은 <c>EventSystem</c> 을 <b>지웠다 되살리는</b> 실습입니다. 끝나면 원래대로예요</item>
    /// <item>②의 <c>Cover</c> 는 교안이 *"확인이 끝나면 지우셔도 됩니다"* 라고 합니다</item>
    /// </list>
    /// </summary>
    public static class Build19UguiEventSystem
    {
        const string FromScene = "18_완성";
        const string StartScene = "19_시작";
        const string CompleteScene = "19_완성";

        /// <summary>
        /// 멈출 때 바뀔 색. <b>빨강이면 안 됩니다.</b>
        ///
        /// 15차시가 자동차 몸체를 <c>RedMat</c>(빨강)으로 칠해뒀는데
        /// <c>CubeSpinner</c> 의 <c>Target Color</c> 기본값도 <b>빨강</b>입니다.
        /// 그대로 두면 «색 바꾸기» 를 눌러도 <b>아무 변화가 없습니다.</b>
        /// 교안 실습 ③ 트러블슈팅이 짚는 그 상황입니다.
        /// </summary>
        static readonly Color StoppedColor = new Color(0.2f, 0.5f, 0.9f);

        [MenuItem("Tools/교안 씬 빌더/19차시 — 자동차 조작 패널", false, 19)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "19차시 씬 만들기",
                    $"{FromScene} 을 복제해 {StartScene} · {CompleteScene} 을 만듭니다.\n" +
                    "이미 있으면 덮어씁니다.\n\n" +
                    "저장 안 된 작업이 있으면 먼저 저장해주세요.",
                    "만들기", "그만두기"))
            {
                return;
            }

            Run();
        }

        /// <summary>확인 창 없이 바로 만듭니다. 「기초 전체 다시 만들기」가 부릅니다.</summary>
        public static void Run()
        {
            BuildKit.BeginFrom(FromScene, StartScene);

            // 실습 ①의 결론 — 이게 없으면 화면의 어떤 것도 안 눌립니다.
            BuildKitUi.EnsureEventSystem();

            CubeSpinner spinner = FindCarSpinner();
            PickStoppedColor(spinner);

            Image panel = BuildControlPanel(spinner);
            BuildKit.SaveAsPrefab(panel.gameObject, "ControlPanel", connect: true);

            BuildBasicsLayout.CameraFor(19, out Vector3 from, out Vector3 lookAt);
            BuildKit.AimCamera(from, lookAt);

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        static CubeSpinner FindCarSpinner()
        {
            GameObject car = GameObject.Find("Car");
            CubeSpinner spinner = car != null ? car.GetComponent<CubeSpinner>() : null;

            if (spinner == null)
            {
                throw new System.InvalidOperationException(
                    "14차시에서 만든 자동차(Car)와 CubeSpinner 가 없습니다. 14차시부터 다시 만들어주세요.");
            }
            return spinner;
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ 4단계 — 멈추면 색도 바뀌게
        // ══════════════════════════════════════════════════════

        static void PickStoppedColor(CubeSpinner spinner)
        {
            spinner.targetColor = StoppedColor;
            EditorUtility.SetDirty(spinner);

            // «회전이 멈추면 색을 바꿔라» — 버튼의 On Click 과 똑같이 생긴 칸입니다.
            // 「색 바꾸기」 버튼을 누르지 않아도 멈추기만 하면 색이 바뀝니다.
            BuildKit.Wire(spinner.onStopped, spinner.ChangeColor);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ 1 ~ 3단계 — 조작 패널
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 버튼 세 개를 담은 <b>그릇(Panel)</b> 을 만듭니다.
        ///
        /// 버튼을 판에 <b>자식으로</b> 넣어두면
        /// 판 하나만 옮겨도 <b>세 개가 통째로</b> 따라오고, 껐다 켜면 <b>한꺼번에</b> 사라졌다 나타납니다.
        /// 14차시의 부모-자식이 화면 쪽에서 그대로 쓰이는 것입니다.
        /// </summary>
        static Image BuildControlPanel(CubeSpinner spinner)
        {
            Canvas canvas = BuildKitUi.Root();
            Transform group = BuildKitUi.Group(canvas, BuildBasicsLayout.UiGroup.L19);

            Image panel = BuildKitUi.Box(
                "ControlPanel", group,
                BuildBasicsLayout.ControlPanel, BuildBasicsLayout.ControlPanelSize,
                new Color(0.1f, 0.1f, 0.13f, 0.55f), blocksClick: true);
            panel.Anchor(BuildBasicsLayout.BottomCenter);

            (string name, string label, UnityEngine.Events.UnityAction call)[] buttons =
            {
                ("StartSpinButton",   "회전 시작",  spinner.StartSpin),
                ("StopSpinButton",    "멈춤",       spinner.StopSpin),
                ("ChangeColorButton", "색 바꾸기",  spinner.ChangeColor),
            };

            float[] xs = BuildBasicsLayout.ControlButtonX;

            for (int i = 0; i < buttons.Length; i++)
            {
                (string name, string label, UnityEngine.Events.UnityAction call) = buttons[i];

                Button b = BuildKitUi.Button(
                    name, label, panel.transform,
                    new Vector2(xs[i], 0f), BuildBasicsLayout.PanelButton);

                BuildKit.Wire(b.onClick, call);
            }

            BuildBasicsLayout.ApplyUiGroups(canvas, 19);
            return panel;
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            bool desk = Object.FindFirstObjectByType<EventSystem>() != null;
            GameObject panel = GameObject.Find("ControlPanel");
            int buttons = panel != null ? panel.GetComponentsInChildren<Button>(true).Length : -1;
            bool mold = BuildKit.LoadPrefab("ControlPanel") != null;

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 19차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    $"안내데스크(EventSystem): {(desk ? "있음" : "❌ 없음")}\n" +
                    $"조작 패널 안의 버튼: {buttons}개 (3개면 정상)\n" +
                    $"패널 틀(Prefab): {(mold ? "만들어짐" : "❌ 없음")} — 20차시가 이걸 씁니다\n" +
                    "▶ 를 누르고 «회전 시작 → 멈춤» 순서로 눌러보세요.\n" +
                    "멈추는 순간 자동차가 파랗게 바뀌어야 합니다. «색 바꾸기» 는 안 눌렀는데도요.");
                return;
            }

            Debug.LogWarning($"⚠️ 19차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
