using UnityEngine;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 갈래 A(13 ~ 21차시)의 <b>배치표</b>. 자리와 크기를 전부 여기 모읍니다.
    ///
    /// <b>왜 한곳에 모으나</b> — 갈래 A는 <b>아홉 차시가 한 씬에 계속 쌓입니다.</b>
    /// 21차시 완성 씬에는 13차시 큐브부터 전부 들어 있어요.
    /// 자리를 각 빌더에 흩어놓으면 <b>무엇이 무엇과 겹치는지 알 수가 없습니다.</b>
    ///
    /// 화면이나 바닥이 붐비면 <b>이 파일만 고치고</b>
    /// <c>Tools ▸ 교안 씬 빌더 ▸ 기초 전체 다시 만들기</c> 를 돌리면 됩니다.
    ///
    /// <b>교안이 값을 지정한 것은 그대로 씁니다.</b>
    /// (14차시 자동차 표 · 17차시 앵커 표 · 20차시 Step 값 · 21차시 낙하 높이)
    /// 교안이 "적당히" 라고 한 것만 여기서 정합니다.
    /// </summary>
    public static class BuildBasicsLayout
    {
        // ══════════════════════════════════════════════════════
        //  3D — 차시마다 자리를 나눠 씁니다
        // ══════════════════════════════════════════════════════

        /// <summary>13차시 큐브. 자동차 무리에서 떨어뜨려 둡니다.</summary>
        public static readonly Vector3 DemoCube = new Vector3(-6f, 0.5f, 6f);

        /// <summary>14차시 자동차. 교안이 `0, 0, 0` 이라고 못박아 뒀습니다.</summary>
        public static readonly Vector3 CarOrigin = Vector3.zero;

        /// <summary>15차시에 놓는 다섯 대의 X 값. 교안 실습 ③ 7번의 그 숫자입니다.</summary>
        public static readonly float[] CarLineX = { 0f, 3f, 6f, 9f, 12f };

        /// <summary>16차시 갈아 끼우기 대상. 교안은 `0, 1, 0` 이지만 자동차와 겹쳐서 옮깁니다.</summary>
        public static readonly Vector3 Display = new Vector3(-6f, 1.5f, 0f);

        /// <summary>20차시 버튼으로 움직일 큐브.</summary>
        public static readonly Vector3 DriveCube = new Vector3(-6f, 1f, -6f);

        /// <summary>21차시 공이 떨어지기 시작하는 자리. 교안의 높이 `5` 를 지킵니다.</summary>
        public static readonly Vector3 Ball = new Vector3(6f, 5f, -6f);

        /// <summary>21차시 통과 영역(실습 ④). 공이 떨어지는 길목입니다.</summary>
        public static readonly Vector3 Gate = new Vector3(6f, 2f, -6f);
        public static readonly Vector3 GateScale = new Vector3(3f, 0.2f, 3f);

        /// <summary>
        /// 21차시 바닥.
        ///
        /// 교안은 <c>Scale 2,1,2</c> 를 예로 들지만 *"좁으면 키우세요"* 라고 했습니다.
        /// <b>여기서는 3 입니다</b> — 15차시 자동차가 <c>X 12</c> 까지 늘어서 있어서,
        /// 2(가로 20, X ±10)면 <b>마지막 한 대가 바닥 밖으로 나갑니다.</b>
        /// </summary>
        public static readonly Vector3 GroundScale = new Vector3(3f, 1f, 3f);

        // ══════════════════════════════════════════════════════
        //  화면(Canvas) — 1920 × 1080 기준
        // ══════════════════════════════════════════════════════
        //
        //  ┌──────────── TopBar ────────────┐
        //  │ HealthBar          ScoreBox    │   ← 17차시
        //  │                    (ScoreText) │   ← 18차시
        //  │ [멈춤]        DemoImage        │   ← 14 · 17차시
        //  │ [다시 돌리기]                  │   ← 14차시
        //  │ [입체]              [눌러보기] │   ← 16 · 18차시
        //  │ [그림]                 InfoText│   ← 20차시
        //  │ [효과]                         │
        //  │ [끄기]   ◀ 비워둡니다 ▶        │   ← 자동차 · 큐브가 서는 자리
        //  │              Slider            │   ← 18차시
        //  │          ┌ DrivePanel ┐        │   ← 20차시
        //  │          └ ControlPanel ┘      │   ← 19차시
        //  └────────── BottomBar ───────────┘
        //
        //  ⚠ 가운데 띠(아래에서 480 ~ 700)는 비워둡니다.
        //     19 · 20차시의 주인공이 거기 섭니다. UI 를 놓으면 가로막습니다.
        //
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 14차시 "멈춤" 버튼. 왼쪽 줄의 맨 위입니다.
        ///
        /// <b>"다시 돌리기" 버튼은 이 바로 아래</b>에 붙습니다 (<c>Y - (높이 + 10)</c>).
        /// 따로 값을 두지 않는 이유는 <b>둘이 항상 붙어 다녀야</b> 하기 때문입니다.
        /// 이 값을 옮기면 아래 버튼도 같이 따라옵니다.
        /// </summary>
        public static readonly Vector2 StopButton = new Vector2(180f, 220f);

        /// <summary>16차시 버튼 네 개(입체 · 그림 · 효과 · 끄기)의 Y 값. X 는 왼쪽 줄과 같습니다.</summary>
        public static readonly float SideColumnX = 180f;
        public static readonly float[] ModeButtonY = { 60f, 0f, -60f, -120f };

        /// <summary>왼쪽 줄 버튼 크기. 14차시 · 16차시가 같이 씁니다.</summary>
        public static readonly Vector2 SideButton = new Vector2(160f, 50f);

        /// <summary>
        /// 17차시 실습 ①②의 데모 그림.
        ///
        /// <b>가운데 빈 띠보다 위</b>로 올려둡니다. 아래로 내리면 자동차 · 큐브를 덮습니다.
        /// </summary>
        public static readonly Vector2 DemoImage = new Vector2(0f, 330f);
        public static readonly Vector2 DemoImageSize = new Vector2(160f, 160f);

        /// <summary>17차시 HUD 네 조각. 크기는 교안 실습 ③ 표 그대로입니다.</summary>
        public static readonly Vector2 HealthBar = new Vector2(170f, -110f);
        public static readonly Vector2 HealthBarSize = new Vector2(300f, 40f);
        public static readonly Vector2 ScoreBox = new Vector2(-120f, -120f);
        public static readonly Vector2 ScoreBoxSize = new Vector2(200f, 60f);
        public static readonly float TopBarHeight = 80f;
        public static readonly float BottomBarHeight = 120f;

        /// <summary>18차시 실습 ①의 «색이 변하는 버튼». 19차시 실습 ①에서 이 색 변화를 확인합니다.</summary>
        public static readonly Vector2 ColorButton = new Vector2(-260f, 120f);

        /// <summary>
        /// 18차시 슬라이더. 아래쪽 패널 두 개 바로 위입니다.
        ///
        /// <b>화면 한가운데에 두면 안 됩니다.</b>
        /// 거기는 <b>자동차 · 큐브가 서는 자리</b>라, 19 · 20차시에서
        /// <b>주인공을 가로막는 막대</b>가 됩니다. 실제로 그렇게 만들어봤습니다.
        /// </summary>
        public static readonly Vector2 Slider = new Vector2(0f, 450f);
        public static readonly Vector2 SliderSize = new Vector2(400f, 30f);

        /// <summary>19차시 조작 패널. 교안이 말한 `Width 600, Height 140` 입니다.</summary>
        public static readonly Vector2 ControlPanel = new Vector2(0f, 130f);
        public static readonly Vector2 ControlPanelSize = new Vector2(600f, 140f);

        /// <summary>19차시 버튼 세 개의 X 값. 교안 실습 ③ 8번의 그 숫자입니다.</summary>
        public static readonly float[] ControlButtonX = { -200f, 0f, 200f };

        /// <summary>20차시 조작 패널. 19차시 것 바로 위에, 버튼이 일곱이라 훨씬 넓습니다.</summary>
        public static readonly Vector2 DrivePanel = new Vector2(0f, 285f);
        public static readonly Vector2 DrivePanelSize = new Vector2(1200f, 140f);

        /// <summary>
        /// 20차시 버튼 일곱 개의 X 값. 간격 165 · 너비 150 이라 <b>15칸씩 벌어집니다.</b>
        /// 좁히면 «← 왼쪽» 같은 글자가 서로 붙어 읽기 어려워집니다.
        /// </summary>
        public static readonly float[] DriveButtonX =
            { -495f, -330f, -165f, 0f, 165f, 330f, 495f };

        /// <summary>19차시 조작 패널 안의 버튼. «회전 시작» 이 안 잘리게 넉넉히 잡았습니다.</summary>
        public static readonly Vector2 PanelButton = new Vector2(160f, 60f);

        /// <summary>20차시 조작 패널 안의 버튼. 일곱 개가 들어가야 해서 조금 좁습니다.</summary>
        public static readonly Vector2 DriveButton = new Vector2(150f, 60f);

        /// <summary>20차시 실습 ④의 안내 글자.</summary>
        public static readonly Vector2 InfoText = new Vector2(-260f, 0f);
        public static readonly Vector2 InfoTextSize = new Vector2(300f, 60f);

        // ══════════════════════════════════════════════════════
        //  화면 묶음 — 지난 차시 것을 접습니다
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 차시별 <b>화면 묶음</b>의 이름과 «어느 차시에 무엇을 켤까» 규칙.
        ///
        /// <b>왜</b> — 아홉 차시가 한 판에 쌓이면 <c>Canvas</c> 자식이 <b>열일곱 개</b>가 됩니다.
        /// 버튼만 열여섯 개예요. 그 차시 결과가 뭔지 화면만 봐서는 알 수 없습니다.
        ///
        /// <b>지우지 않고 접습니다.</b> Hierarchy 에는 그대로 있고 체크 한 번이면 돌아옵니다.
        /// 정답지로서 «다 들어 있다» 도 유지됩니다.
        ///
        /// 설계 근거는 <c>plans/13_씬_정리_계획.md</c>.
        /// </summary>
        public static class UiGroup
        {
            public const string L14 = "14차시 조작";
            public const string L16 = "16차시 갈아 끼우기";
            public const string L17 = "17차시 화면 틀";
            public const string L18 = "18차시 버튼과 슬라이더";
            public const string L19 = "19차시 조작 패널";
            public const string L20 = "20차시 조작 패널";

            public static readonly string[] All = { L14, L16, L17, L18, L19, L20 };

            /// <summary>
            /// <b>그 차시에 만든 것 + 그 차시 교안이 이름을 대며 쓰는 것</b> 만 켭니다.
            ///
            /// <list type="bullet">
            /// <item><b>15</b> — 자동차를 멈춰두고 다섯 대를 놓는 차시라 14차시 버튼이 필요합니다</item>
            /// <item><b>16</b> — 17차시 교안이 *"14 · 16차시에 맛보기로 써본 버튼"* 이라 하므로 나란히 둡니다</item>
            /// <item><b>18</b> — 교안 *"17차시에 만든 화면 틀을 그대로 씁니다"*</item>
            /// <item><b>19</b> — 실습 ①이 «버튼이 안 눌린다» 를 확인해야 해서 18차시 버튼을 켜둡니다</item>
            /// <item><b>20</b> — 교안 *"19차시에 만든 조작 패널을 그대로 재활용"*. 나란히 둬야 그 말이 보입니다</item>
            /// <item><b>21</b> — 화면 요소를 안 만드는 차시라 20차시 상태 그대로</item>
            /// </list>
            ///
            /// <c>HealthBar</c> · <c>ScoreBox</c> 는 <b>묶음 밖</b>이라 17차시부터 늘 켜져 있습니다.
            /// 18차시 교안이 <b>이름을 대며 찾기</b> 때문입니다 — 묶음 안에 있으면 한 겹 더 헤맵니다.
            /// </summary>
            public static string[] VisibleAt(int lesson)
            {
                switch (lesson)
                {
                    case 14: return new[] { L14 };
                    case 15: return new[] { L14 };
                    case 16: return new[] { L14, L16 };
                    case 17: return new[] { L17 };
                    case 18: return new[] { L17, L18 };
                    case 19: return new[] { L18, L19 };
                    case 20: return new[] { L19, L20 };
                    case 21: return new[] { L19, L20 };
                    default: return All;
                }
            }
        }

        /// <summary>그 차시 규칙대로 화면 묶음을 켜고 끕니다. 빌더 끝에서 한 번 부릅니다.</summary>
        public static void ApplyUiGroups(Canvas canvas, int lesson)
        {
            if (canvas == null) return;
            BuildKitUi.SetGroupsActive(canvas, UiGroup.All, UiGroup.VisibleAt(lesson));
        }

        // ══════════════════════════════════════════════════════
        //  앵커 — 17차시에서 배우는 그 압정 자리
        // ══════════════════════════════════════════════════════

        public static readonly Vector2 TopLeft = new Vector2(0f, 1f);
        public static readonly Vector2 TopRight = new Vector2(1f, 1f);
        public static readonly Vector2 MiddleLeft = new Vector2(0f, 0.5f);
        public static readonly Vector2 MiddleRight = new Vector2(1f, 0.5f);
        public static readonly Vector2 MiddleCenter = new Vector2(0.5f, 0.5f);
        public static readonly Vector2 BottomCenter = new Vector2(0.5f, 0f);

        // ══════════════════════════════════════════════════════
        //  부품 값 — 교안이 표로 지정한 것들
        // ══════════════════════════════════════════════════════

        /// <summary>20차시 실습 ③ 2번 표.</summary>
        public const float MoveStep = 0.5f;
        public const float RotateStep = 15f;
        public const float ScaleStep = 1.2f;

        /// <summary>18차시 실습 ③ 1단계 표.</summary>
        public const float SliderMin = 0f;
        public const float SliderMax = 100f;
        public const float SliderStart = 50f;

        /// <summary>13차시 실습 ④. 교안이 예로 든 문구입니다.</summary>
        public const string ConsoleMessage = "안녕하세요";

        // ══════════════════════════════════════════════════════
        //  카메라 — 차시마다 그 결과물이 보이게
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 차시 번호를 주면 <b>그 차시 결과물을 담는 카메라 자리</b>를 돌려줍니다.
        ///
        /// 씬은 계속 쌓이지만 <b>완성 씬 스크린샷은 그 차시를 보여줘야</b> 합니다.
        /// 씬 내용은 건드리지 않고 카메라만 옮겨서 풉니다. (`plans/10` 의 결론)
        /// </summary>
        public static void CameraFor(int lesson, out Vector3 from, out Vector3 lookAt)
        {
            switch (lesson)
            {
                case 13:                                  // 큐브 하나
                    from = DemoCube + new Vector3(0f, 2f, -6f);
                    lookAt = DemoCube;
                    break;

                case 14:                                  // 자동차 한 대
                    from = new Vector3(4f, 3f, -6f);
                    lookAt = CarOrigin + new Vector3(0f, 0.5f, 0f);
                    break;

                case 15:                                  // 다섯 대가 나란히
                    from = new Vector3(6f, 7f, -13f);
                    lookAt = new Vector3(6f, 0.5f, 0f);
                    break;

                case 16:                                  // 갈아 끼우는 자리
                    // 정면에서 보면 큐브도 납작한 그림도 «흰 사각형» 하나로 보입니다.
                    // 살짝 비스듬히 봐야 큐브가 «면 세 개짜리 덩어리» 로 읽힙니다.
                    // 다만 너무 옆에서 보면 그림이 종이처럼 얇아져 안 보입니다 (교안이 경고하는 그것).
                    from = Display + new Vector3(1.8f, 1.1f, -4.2f);
                    lookAt = Display;
                    break;

                case 19:                                  // 색이 바뀌는 «첫 번째» 자동차
                    // 다섯 대 중 한 대만 파래집니다. 멀리서 보면 뭐가 바뀌었는지 안 보입니다.
                    // 첫 대를 크게 잡고, 나머지 넷은 뒤로 물러나게 두면 «빨강 넷 · 파랑 하나» 가 됩니다.
                    // 조준점을 자동차보다 «아래» 로 두는 이유 — 자동차가 화면 가운데보다 위로 올라와
                    // 아래쪽 슬라이더 · 패널에 안 가립니다.
                    from = new Vector3(-2.6f, 1.5f, -2.8f);
                    lookAt = new Vector3(0.3f, 0.15f, 0f);
                    break;

                case 20:                                  // 버튼으로 움직일 큐브
                    from = DriveCube + new Vector3(2.6f, 1.4f, -2.8f);
                    lookAt = DriveCube + new Vector3(0f, -0.25f, 0f);
                    break;

                case 21:                                  // 공이 떨어지는 길
                    from = Ball + new Vector3(-3f, -1f, -8f);
                    lookAt = new Vector3(Ball.x, 1f, Ball.z);
                    break;

                default:                                  // 17 · 18 — 화면(UI)이 주인공
                    from = new Vector3(0f, 8f, -16f);     // 자동차와 큐브가 함께 들어오게
                    lookAt = new Vector3(2f, 0.5f, 0f);
                    break;
            }
        }
    }
}
