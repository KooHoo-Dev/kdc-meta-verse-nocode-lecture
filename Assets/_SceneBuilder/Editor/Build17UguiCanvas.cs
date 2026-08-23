using System.Collections.Generic;
using NoCodeKit.EditorTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 17차시 — 안 깨지는 화면 틀 (Canvas · 앵커).
    ///
    /// 교안 `17_ugui_canvas_nocode.md` 에서 <b>씬에 남는 것</b>만 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ① 판 위에 그림 하나 올려보기 (<c>DemoImage</c>)</item>
    /// <item>실습 ② <b>화면 크기 맞춤</b>(<c>Canvas Scaler</c>) 켜기</item>
    /// <item>실습 ③ <b>앵커로 자리를 못 박은 HUD 네 조각</b></item>
    /// <item>실습 ④ <c>HealthBar</c> 를 <b>채워지는 그림</b>으로 (18차시가 이걸 씁니다)</item>
    /// </list>
    ///
    /// <b>판(Canvas)은 14차시에 버튼을 만들면서 이미 생겼습니다.</b>
    /// 그때는 <b>유니티 기본값</b>으로 뒀습니다 — 화면 크기 맞춤은 오늘 켜는 것이라서요.
    /// </summary>
    public static class Build17UguiCanvas
    {
        const string FromScene = "16_완성";
        const string StartScene = "17_시작";
        const string CompleteScene = "17_완성";

        [MenuItem("Tools/교안 씬 빌더/17차시 — 안 깨지는 화면 틀", false, 17)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "17차시 씬 만들기",
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
            TurnOnScaling(canvas);
            BuildDemoImage(canvas);
            BuildHud(canvas);

            BuildBasicsLayout.CameraFor(17, out Vector3 from, out Vector3 lookAt);
            BuildKit.AimCamera(from, lookAt);

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ② — 화면 크기 맞춤
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// <c>Canvas Scaler</c> 를 <b>화면 크기에 맞춰 늘어나게</b> 바꿉니다.
        /// 교안 실습 ② — <c>Scale With Screen Size</c> · <c>1920 × 1080</c> · <c>Match 0.5</c>.
        ///
        /// <b>이게 이 차시의 절반입니다.</b> 14차시가 만든 판은 기본값이라
        /// 해상도가 바뀌면 화면 요소가 제멋대로 커지고 작아집니다.
        /// </summary>
        static void TurnOnScaling(Canvas canvas)
        {
            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null) scaler = canvas.gameObject.AddComponent<CanvasScaler>();

            BuildKitUi.ScaleWithScreen(scaler);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ① · ② — 판 위에 그림 하나
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 실습 ①에서 만든 그림입니다. 가운데에 그대로 둡니다.
        ///
        /// 실습 ② 7번은 여기에 <b>15차시에 넣어둔 그림 파일</b>을 끼우게 합니다.
        /// 그 파일은 아직 없으므로 <b>색만 있는 사각형</b>으로 둡니다.
        /// 유니티에서 <c>UI ▸ Image</c> 를 만들어도 처음엔 이 모습입니다.
        /// </summary>
        static void BuildDemoImage(Canvas canvas)
        {
            Transform group = BuildKitUi.Group(canvas, BuildBasicsLayout.UiGroup.L17);

            Image demo = BuildKitUi.Box(
                "DemoImage", group,
                BuildBasicsLayout.DemoImage, BuildBasicsLayout.DemoImageSize,
                new Color(0.95f, 0.85f, 0.35f, 0.75f), blocksClick: true);

            demo.Anchor(BuildBasicsLayout.MiddleCenter);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ · ④ — 앵커로 못 박은 네 조각
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 네 조각을 만들고 <b>서로 다른 앵커</b>를 줍니다. 교안 실습 ③ 2단계 표 그대로입니다.
        ///
        /// | 이름 | 앵커 | 화면이 커지면 |
        /// | <c>HealthBar</c> | 왼쪽 위 한 점 | 자리만 지킵니다 |
        /// | <c>ScoreBox</c> | 오른쪽 위 한 점 | 〃 |
        /// | <c>TopBar</c> · <c>BottomBar</c> | <b>가로로 늘어남</b> | <b>같이 넓어집니다</b> |
        ///
        /// 색은 서로 다르게 합니다. 교안 실습 ③ 3번 — *"구분되게 합니다"*.
        /// </summary>
        static void BuildHud(Canvas canvas)
        {
            Transform group = BuildKitUi.Group(canvas, BuildBasicsLayout.UiGroup.L17);
            Color bar = new Color(0.12f, 0.12f, 0.16f, 0.9f);

            // HealthBar · ScoreBox 는 «판 바로 아래» 에 둡니다. 묶음 안이 아닙니다.
            // 18차시 교안이 이름을 대며 찾고(«17차시에 만든 ScoreBox 위에서 우클릭»),
            // 끝까지 켜져 있어야 해서 접히면 안 됩니다.

            Image health = BuildKitUi.Box(
                "HealthBar", canvas.transform,
                BuildBasicsLayout.HealthBar, BuildBasicsLayout.HealthBarSize,
                new Color(0.85f, 0.25f, 0.3f), blocksClick: true);
            health.Anchor(BuildBasicsLayout.TopLeft);
            MakeFillable(health);
            Label(health, "왼쪽 위", 22f);

            Image score = BuildKitUi.Box(
                "ScoreBox", canvas.transform,
                BuildBasicsLayout.ScoreBox, BuildBasicsLayout.ScoreBoxSize,
                new Color(0.2f, 0.35f, 0.65f), blocksClick: true);
            score.Anchor(BuildBasicsLayout.TopRight);
            Label(score, "오른쪽 위", 22f);

            Label(BuildKitUi.Box("TopBar", group, Vector2.zero, Vector2.zero,
                                 bar, blocksClick: true)
                            .StretchX(top: true, height: BuildBasicsLayout.TopBarHeight),
                  "위쪽 · 가로로 늘어남", 28f);

            Label(BuildKitUi.Box("BottomBar", group, Vector2.zero, Vector2.zero,
                                 bar, blocksClick: true)
                            .StretchX(top: false, height: BuildBasicsLayout.BottomBarHeight),
                  "아래쪽 · 가로로 늘어남", 28f);

            BuildBasicsLayout.ApplyUiGroups(canvas, 17);
        }

        /// <summary>
        /// 조각 안에 <b>이름을 적어둡니다.</b>
        ///
        /// <b>왜 필요한가</b> — 안 적으면 <b>색깔 사각형 네 개</b>일 뿐입니다.
        /// 이 차시의 핵심은 *"압정을 어디에 꽂았느냐"* 인데,
        /// 화면만 봐서는 <b>어느 게 어떤 앵커인지 알 수가 없습니다.</b>
        ///
        /// 이름을 적어두면 해상도를 바꿔볼 때
        /// «왼쪽 위» 는 왼쪽 위에 붙어 있고 «가로로 늘어남» 은 같이 넓어지는 게 <b>바로 읽힙니다.</b>
        /// </summary>
        static void Label(Component piece, string text, float fontSize)
        {
            TMPro.TextMeshProUGUI label = BuildKitUi.Text(
                "Label", text, piece.transform, Vector2.zero, Vector2.zero, fontSize);

            label.Stretch();
            label.color = Color.white;
            label.raycastTarget = false;   // 글자가 클릭을 가로채면 안 됩니다 (19차시에서 배웁니다)
            EditorUtility.SetDirty(label);
        }

        /// <summary>
        /// <c>HealthBar</c> 를 <b>일부만 채워지는 그림</b>으로 만듭니다. 교안 실습 ④.
        ///
        /// <b>선택 실습이지만 반드시 해둡니다.</b>
        /// 18차시 실습 ④ 1번이 *"`Image Type` 이 `Filled` 인지 확인합니다"* 로 시작해서,
        /// 여기가 안 돼 있으면 <b>슬라이더로 체력바를 움직이는 실습이 성립하지 않습니다.</b>
        /// </summary>
        static void MakeFillable(Image image)
        {
            // 채우기는 그림(Sprite)이 있어야 동작합니다. 내장 사각형을 씁니다.
            //
            // ⚠ UISprite.psd 를 쓰면 안 됩니다. 그건 «모서리가 둥근» 그림이라,
            //   채우기 모드에서는 테두리를 안 자르고 통째로 늘려서 «알약» 모양이 됩니다.
            //   Background.psd 는 각진 사각형이라 체력바로 제대로 보입니다.
            image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");

            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = (int)Image.OriginHorizontal.Left;
            image.fillAmount = 1f;

            EditorUtility.SetDirty(image);
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            var canvas = Object.FindFirstObjectByType<Canvas>();
            var scaler = canvas != null ? canvas.GetComponent<CanvasScaler>() : null;
            bool scaled = scaler != null
                          && scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize;

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 17차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    $"화면 크기 맞춤: {(scaled ? "켜짐" : "❌ 꺼짐")}\n" +
                    "Game 창의 해상도 목록을 16:9 → 4:3 → 세로로 긴 것 순서로 바꿔보세요.\n" +
                    "네 조각이 각자 자리를 지켜야 합니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 17차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
