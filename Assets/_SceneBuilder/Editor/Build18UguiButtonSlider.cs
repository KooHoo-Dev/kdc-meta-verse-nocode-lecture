using System.Collections.Generic;
using NoCodeKit.EditorTools;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 18차시 — 버튼 · 슬라이더 · 글자.
    ///
    /// 교안 `18_ugui_button_slider_nocode.md` 에서 <b>씬에 남는 것</b>만 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ① 눌리는 상태마다 색이 바뀌는 버튼 (<c>ColorButton</c>)</item>
    /// <item>실습 ② <c>ScoreBox</c> 안의 글자 (<c>ScoreText</c>)</item>
    /// <item>실습 ③ <b>슬라이더를 움직이면 숫자가 따라 바뀌게</b> — 이 차시의 핵심</item>
    /// <item>실습 ④ <b>하나를 움직여 둘을 바꾸기</b> (숫자 + 체력바)</item>
    /// </list>
    ///
    /// <b>이 차시에 갈래 A 최대의 함정이 있습니다.</b>
    /// 값을 넘기는 연결은 목록 <b>위쪽 <c>Dynamic float</c></b> 이라야 합니다.
    /// 아래쪽 <c>Static Parameters</c> 를 고르면 <b>미리 적어둔 고정 숫자</b>가 들어갑니다.
    /// 25 · 26차시에서 같은 함정이 두 번 더 나옵니다.
    /// </summary>
    public static class Build18UguiButtonSlider
    {
        const string FromScene = "17_완성";
        const string StartScene = "18_시작";
        const string CompleteScene = "18_완성";

        [MenuItem("Tools/교안 씬 빌더/18차시 — 버튼과 슬라이더", false, 18)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "18차시 씬 만들기",
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

            Canvas canvas = BuildKitUi.Root();

            BuildColorButton(canvas);
            ValueDisplay display = BuildScoreText();
            Slider slider = BuildSlider(canvas, display);
            LinkHealthBar(slider, display);

            BuildBasicsLayout.ApplyUiGroups(canvas, 18);

            BuildBasicsLayout.CameraFor(18, out Vector3 from, out Vector3 lookAt);
            BuildKit.AimCamera(from, lookAt);

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ① — 눌리는 상태마다 색이 바뀌는 버튼
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 버튼의 <b>네 가지 상태 색</b>을 아주 다르게 잡습니다. 교안 실습 ① 3번 표 그대로입니다.
        ///
        /// <b>19차시가 이 버튼을 씁니다.</b>
        /// 19차시 실습 ①은 안내데스크(<c>EventSystem</c>)를 지우고
        /// *"마우스를 올려도 색조차 안 변했죠?"* 를 확인시킵니다.
        /// 색이 확 달라야 그게 눈에 보입니다.
        /// </summary>
        static void BuildColorButton(Canvas canvas)
        {
            Transform group = BuildKitUi.Group(canvas, BuildBasicsLayout.UiGroup.L18);

            Button button = BuildKitUi.Button(
                "ColorButton", "눌러보기", group,
                BuildBasicsLayout.ColorButton, BuildBasicsLayout.SideButton);
            button.Anchor(BuildBasicsLayout.MiddleRight);

            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.92f, 0.3f);   // 마우스를 올리면 노랑
            colors.pressedColor = new Color(0.9f, 0.25f, 0.25f);    // 누르면 빨강
            colors.disabledColor = new Color(0.35f, 0.35f, 0.35f);  // 못 누를 때 진회색
            button.colors = colors;

            // 교안 실습 ① 5번은 Interactable 을 잠깐 꺼보게 합니다. 확인용이라 켜둡니다.
            button.interactable = true;
            EditorUtility.SetDirty(button);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ② — ScoreBox 안의 글자
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 17차시에 만든 <c>ScoreBox</c> <b>안에</b> 글자를 넣습니다. 교안 실습 ② 1번.
        ///
        /// 자식으로 넣고 <b>상자를 꽉 채우게</b> 해두면,
        /// 화면이 커져 상자가 움직여도 <b>글자가 같이 따라갑니다.</b> 14차시의 부모-자식 그대로입니다.
        /// </summary>
        /// <summary>
        /// 17차시가 붙여둔 <b>앵커 이름표</b>(«왼쪽 위» · «오른쪽 위»)를 뗍니다.
        ///
        /// 그건 <b>17차시에서 «어느 게 어떤 앵커인지» 보여주려고</b> 적어둔 것입니다.
        /// 오늘부터 이 둘은 <b>진짜 점수판 · 진짜 체력바</b>가 되니 이름표는 치웁니다.
        /// 위아래 띠(<c>TopBar</c> · <c>BottomBar</c>)는 계속 장식이라 그대로 둡니다.
        /// </summary>
        static void DropAnchorLabel(GameObject piece)
        {
            Transform label = piece.transform.Find("Label");
            if (label != null) Object.DestroyImmediate(label.gameObject);
        }

        static ValueDisplay BuildScoreText()
        {
            GameObject box = GameObject.Find("ScoreBox");
            if (box == null)
            {
                throw new System.InvalidOperationException(
                    "17차시에서 만든 ScoreBox 가 없습니다. 17차시부터 다시 만들어주세요.");
            }

            DropAnchorLabel(box);

            TextMeshProUGUI text = BuildKitUi.Text(
                "ScoreText", "점수: 0", box.transform,
                Vector2.zero, BuildBasicsLayout.ScoreBoxSize, 36f);
            text.Stretch();

            var display = BuildKit.Add<ValueDisplay>(text.gameObject);
            display.prefix = "점수: ";
            display.suffix = " 점";
            display.decimals = 0;
            EditorUtility.SetDirty(display);

            return display;
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ — 슬라이더로 숫자 바꾸기 (핵심)
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 슬라이더를 놓고 <b>글자에 이어줍니다.</b>
        ///
        /// 연결은 <b>목록 위쪽 <c>Dynamic float</c></b> 입니다.
        /// 슬라이더가 <b>«지금 값이 얼마다» 까지 같이 넘겨주기</b> 때문입니다.
        /// 버튼의 <c>On Click</c> 과 다른 점이 딱 그것 하나입니다.
        /// </summary>
        static Slider BuildSlider(Canvas canvas, ValueDisplay display)
        {
            Transform group = BuildKitUi.Group(canvas, BuildBasicsLayout.UiGroup.L18);

            Slider slider = BuildKitUi.Slider(
                "Slider", group,
                BuildBasicsLayout.Slider, BuildBasicsLayout.SliderSize,
                BuildBasicsLayout.SliderMin, BuildBasicsLayout.SliderMax,
                BuildBasicsLayout.SliderStart, wholeNumbers: true);

            slider.Anchor(BuildBasicsLayout.BottomCenter);

            BuildKit.WireDynamic(slider.onValueChanged, display.SetValue);
            return slider;
        }

        // ══════════════════════════════════════════════════════
        //  실습 ④ — 하나를 움직여 둘을 바꾸기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 같은 슬라이더에 <b>칸을 하나 더</b> 만들어 체력바까지 움직입니다.
        ///
        /// <b>«칸을 여러 개 만들면 한꺼번에 실행된다»</b> — 이 원리가 여기서 처음 나옵니다.
        /// 19차시 <c>On Stopped</c>, 25차시 점수, 26차시 <c>On Game Start</c> 여섯 개까지 이어집니다.
        ///
        /// 체력바의 <c>Fill Amount</c> 는 <b>0 ~ 1 사이 값</b>이라
        /// 슬라이더의 최대값도 <c>1</c> 로 내려야 합니다. 교안 실습 ④ 5번.
        /// 그러면 정수만 나오게 해둔 것도 풀어야 해서, 소수 두 자리로 바꿉니다.
        /// </summary>
        static void LinkHealthBar(Slider slider, ValueDisplay display)
        {
            GameObject bar = GameObject.Find("HealthBar");
            if (bar == null)
            {
                throw new System.InvalidOperationException(
                    "17차시에서 만든 HealthBar 가 없습니다. 17차시부터 다시 만들어주세요.");
            }

            var image = bar.GetComponent<Image>();
            if (image == null || image.type != Image.Type.Filled)
            {
                throw new System.InvalidOperationException(
                    "HealthBar 가 «채워지는 그림(Filled)» 이 아닙니다. 17차시부터 다시 만들어주세요.");
            }

            // 체력바가 줄어들면 이름표만 떠 있게 되므로 같이 뗍니다.
            DropAnchorLabel(bar);

            // fillAmount 는 칸이 아니라 «값 넣는 자리» 라서 set_fillAmount 를 찾아 겁니다.
            BuildKit.WireDynamicProperty(slider.onValueChanged, image, "fillAmount");

            slider.wholeNumbers = false;
            slider.maxValue = 1f;
            slider.value = 0.5f;
            EditorUtility.SetDirty(slider);

            display.decimals = 2;
            EditorUtility.SetDirty(display);
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            var slider = Object.FindFirstObjectByType<Slider>();
            int wired = slider != null ? slider.onValueChanged.GetPersistentEventCount() : -1;

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 18차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    $"슬라이더의 On Value Changed 에 {wired}개가 걸렸습니다. (2개면 정상)\n" +
                    "▶ 를 누르고 슬라이더를 끌어보세요.\n" +
                    "오른쪽 위 숫자와 왼쪽 위 체력바가 «같이» 움직여야 합니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 18차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
