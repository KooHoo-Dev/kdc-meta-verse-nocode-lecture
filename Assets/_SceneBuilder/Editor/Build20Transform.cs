using System.Collections.Generic;
using NoCodeKit.EditorTools;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 20차시 — 버튼으로 물건을 움직이기 (Transform).
    ///
    /// 교안 `20_transform_nocode.md` 에서 <b>씬에 남는 것</b>만 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ③ <c>TransformDriver</c> + <b>버튼 일곱 개</b>로 이동 · 회전 · 확대 · 되돌리기</item>
    /// <item>실습 ④ 움직일 때마다 <b>글자가 갱신되게</b> — <c>Static</c> 을 쓰는 유일한 곳</item>
    /// </list>
    ///
    /// <b>실습 ① · ②는 만들지 않습니다.</b>
    /// ①은 축의 색(빨강 X · 초록 Y · 파랑 Z)을 눈으로 보는 것,
    /// ②는 <c>Global</c> ↔ <c>Local</c> 을 오가며 손잡이가 어떻게 달라지는지 보는 것입니다.
    /// 둘 다 <b>씬에 남는 게 없습니다.</b>
    /// </summary>
    public static class Build20Transform
    {
        const string FromScene = "19_완성";
        const string StartScene = "20_시작";
        const string CompleteScene = "20_완성";

        [MenuItem("Tools/교안 씬 빌더/20차시 — 버튼으로 움직이기", false, 20)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "20차시 씬 만들기",
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

            TransformDriver driver = BuildDriveCube();
            BuildDrivePanel(driver);
            BuildInfoText(driver);

            BuildBasicsLayout.CameraFor(20, out Vector3 from, out Vector3 lookAt);
            BuildKit.AimCamera(from, lookAt);

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ 1단계 — 움직일 물건
        // ══════════════════════════════════════════════════════

        static TransformDriver BuildDriveCube()
        {
            Material mat = BuildKit.Mat("DriveCube", new Color(0.35f, 0.7f, 0.45f));

            GameObject cube = BuildKit.Shape("DriveCube", PrimitiveType.Cube)
                                      .At(BuildBasicsLayout.DriveCube)
                                      .Paint(mat);

            var driver = BuildKit.Add<TransformDriver>(cube);
            driver.moveStep = BuildBasicsLayout.MoveStep;
            driver.rotateStep = BuildBasicsLayout.RotateStep;
            driver.scaleStep = BuildBasicsLayout.ScaleStep;
            EditorUtility.SetDirty(driver);

            return driver;
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ 2 · 3단계 — 버튼 일곱 개
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 버튼 일곱 개짜리 조작 패널을 만듭니다.
        ///
        /// 교안은 <b>19차시에 만든 <c>ControlPanel</c> 틀을 끌어다 놓고 복제</b>하라고 하면서
        /// *"없으시면 `UI ▸ Panel` 로 새로 만드셔도 됩니다"* 라고 덧붙입니다.
        /// <b>빌더는 새로 만드는 쪽</b>입니다 — 틀에서 찍어낸 뒤 버튼을 넷 더 붙이면
        /// <b>«틀과 다른 부분» 이 잔뜩 생겨</b> 정답 씬이 지저분해집니다. 결과물은 같습니다.
        ///
        /// <b>버튼 글자에 화살표 말고는 기호를 안 씁니다.</b>
        /// 한글 글꼴에 없는 기호(예: 시계 방향 화살표)는 <b>네모(☐)로 나옵니다.</b>
        /// </summary>
        static void BuildDrivePanel(TransformDriver driver)
        {
            Canvas canvas = BuildKitUi.Root();
            Transform group = BuildKitUi.Group(canvas, BuildBasicsLayout.UiGroup.L20);

            Image panel = BuildKitUi.Box(
                "DrivePanel", group,
                BuildBasicsLayout.DrivePanel, BuildBasicsLayout.DrivePanelSize,
                new Color(0.1f, 0.1f, 0.13f, 0.55f), blocksClick: true);
            panel.Anchor(BuildBasicsLayout.BottomCenter);

            (string name, string label, UnityEngine.Events.UnityAction call)[] buttons =
            {
                ("MoveLeftButton",  "← 왼쪽",   driver.MoveXMinus),
                ("MoveRightButton", "→ 오른쪽", driver.MoveXPlus),
                ("MoveUpButton",    "↑ 위",     driver.MoveYPlus),
                ("MoveDownButton",  "↓ 아래",   driver.MoveYMinus),
                ("RotateButton",    "돌리기",   driver.RotateYPlus),
                ("ScaleUpButton",   "크게",     driver.ScaleUp),
                ("ResetButton",     "처음으로", driver.ResetAll),
            };

            float[] xs = BuildBasicsLayout.DriveButtonX;

            for (int i = 0; i < buttons.Length; i++)
            {
                (string name, string label, UnityEngine.Events.UnityAction call) = buttons[i];

                Button b = BuildKitUi.Button(
                    name, label, panel.transform,
                    new Vector2(xs[i], 0f), BuildBasicsLayout.DriveButton);

                BuildKit.Wire(b.onClick, call);
            }
        }

        // ══════════════════════════════════════════════════════
        //  실습 ④ — 넘겨줄 게 없으면 Static
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 움직일 때마다 글자가 다시 그려지게 합니다.
        ///
        /// <b>여기가 갈래 A에서 유일하게 «아래쪽 <c>Static</c>» 을 고르는 곳입니다.</b>
        ///
        /// 18차시 슬라이더는 <b>«지금 값이 얼마다» 를 같이 넘겨줬습니다</b> → 위쪽 <c>Dynamic float</c>.
        /// 그런데 <c>On Changed</c> 는 <b>«바뀌었다» 만 알려주고 숫자는 안 넘깁니다</b>
        /// → 넘겨받을 값이 없으니 <b>아래쪽 <c>Static</c></b> 에 <c>0</c> 을 적어둡니다.
        ///
        /// <b>넘겨줄 게 있으면 Dynamic, 없으면 Static.</b> 이 대비가 실습 ④의 전부입니다.
        ///
        /// <b>처음 글자를 «높이: 0.0» 이 아니라 «버튼을 눌러보세요» 로 둡니다.</b>
        /// <c>Static</c> 은 늘 같은 <c>0</c> 을 보내므로 <b>화면에 아무 변화가 없어서</b>
        /// 연결이 걸렸는지 확인할 방법이 없습니다.
        /// 처음 글자를 다르게 두면 <b>첫 클릭에 한 번 바뀌는 것</b>이 그 증거가 됩니다.
        /// </summary>
        static void BuildInfoText(TransformDriver driver)
        {
            Canvas canvas = BuildKitUi.Root();
            Transform group = BuildKitUi.Group(canvas, BuildBasicsLayout.UiGroup.L20);

            TextMeshProUGUI text = BuildKitUi.Text(
                "InfoText", "버튼을 눌러보세요", group,
                BuildBasicsLayout.InfoText, BuildBasicsLayout.InfoTextSize, 30f);
            text.Anchor(BuildBasicsLayout.MiddleRight);

            var display = BuildKit.Add<ValueDisplay>(text.gameObject);
            display.prefix = "높이: ";
            display.suffix = "";
            display.decimals = 1;
            EditorUtility.SetDirty(display);

            BuildKit.WireFloat(driver.onChanged, display.SetValue, 0f);

            BuildBasicsLayout.ApplyUiGroups(canvas, 20);
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            GameObject panel = GameObject.Find("DrivePanel");
            int buttons = panel != null ? panel.GetComponentsInChildren<Button>(true).Length : -1;

            var driver = Object.FindFirstObjectByType<TransformDriver>();
            int changed = driver != null ? driver.onChanged.GetPersistentEventCount() : -1;

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 20차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    $"조작 패널 안의 버튼: {buttons}개 (7개면 정상)\n" +
                    $"On Changed 연결: {changed}개 (1개면 정상 · 아래쪽 Static)\n" +
                    "▶ 를 누르고 버튼을 눌러보세요. 큐브가 움직이고 Inspector 숫자가 같이 변합니다.\n" +
                    "엉망이 되면 «처음으로» 하나면 돌아옵니다.\n" +
                    "오른쪽 글자는 «버튼을 눌러보세요» → 첫 클릭에 «높이: 0.0» 으로 한 번만 바뀝니다.\n" +
                    "두 번째부터 안 바뀌는 게 정상입니다 — Static 이라 늘 같은 0 을 보냅니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 20차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
