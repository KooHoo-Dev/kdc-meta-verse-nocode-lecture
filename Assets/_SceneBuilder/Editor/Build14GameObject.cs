using System.Collections.Generic;
using NoCodeKit.EditorTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 14차시 — 게임오브젝트와 부품, 그리고 자동차.
    ///
    /// 교안 `14_gameobject_component_nocode.md` 에서 <b>씬에 남는 것</b>만 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ③ 자동차 (빈 그릇 + 몸체 + 바퀴 4) · <c>CubeSpinner</c></item>
    /// <item>실습 ④ 버튼 하나로 멈추기 (<c>On Click</c> ▸ <c>StopSpin</c>)</item>
    /// </list>
    ///
    /// <b>실습 ① · ②는 만들 것이 없습니다.</b>
    /// 부품을 하나씩 붙여보고, 세 창을 오가며 마우스로 다뤄보는 실습이라 씬에 안 남습니다.
    /// 교안 실습 ③ 1번도 *"실습 ①②에서 만든 것은 전부 지워주세요"* 라고 합니다.
    /// </summary>
    public static class Build14GameObject
    {
        const string FromScene = "13_완성";
        const string StartScene = "14_시작";
        const string CompleteScene = "14_완성";

        [MenuItem("Tools/교안 씬 빌더/14차시 — 게임오브젝트와 부품", false, 14)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "14차시 씬 만들기",
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

            GameObject car = BuildCar();
            CubeSpinner spinner = BuildKit.Add<CubeSpinner>(car);
            BuildSpinButtons(spinner);

            BuildBasicsLayout.CameraFor(14, out Vector3 from, out Vector3 lookAt);
            BuildKit.AimCamera(from, lookAt);

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ — 자동차 만들기
        // ══════════════════════════════════════════════════════

        /// <summary>바퀴 네 개의 이름과 자리. 교안 실습 ③ 3단계 표 그대로입니다.</summary>
        static readonly (string name, Vector3 pos)[] Wheels =
        {
            ("Wheel_FL", new Vector3(0.7f, 0.2f, 0.55f)),
            ("Wheel_FR", new Vector3(0.7f, 0.2f, -0.55f)),
            ("Wheel_RL", new Vector3(-0.7f, 0.2f, 0.55f)),
            ("Wheel_RR", new Vector3(-0.7f, 0.2f, -0.55f)),
        };

        /// <summary>
        /// 바퀴를 눕히는 회전. <b><c>X</c> 가 90 입니다. <c>Z</c> 가 아닙니다.</b>
        ///
        /// 유니티 원기둥은 축이 <b>Y</b> 방향입니다. 이걸 바퀴 축 방향으로 돌려야 하는데,
        /// 몸통이 <c>Scale 2, 0.5, 1</c> 이라 <b>차는 X 방향으로 길고</b>,
        /// 바퀴는 <c>Z ±0.55</c> 로 <b>옆면에</b> 붙습니다.
        /// → 차가 X 로 굴러가려면 <b>바퀴 축이 Z</b> 여야 하고, 그러려면 <b>X 축으로 90°</b> 입니다.
        ///
        /// <c>Z</c> 에 넣으면 축이 X 가 되어 <b>앞뒤 범퍼에 원반이 붙은 모양</b>이 됩니다.
        /// </summary>
        static readonly Vector3 WheelRotation = new Vector3(90f, 0f, 0f);
        static readonly Vector3 WheelScale = new Vector3(0.4f, 0.1f, 0.4f);

        /// <summary>
        /// 빈 그릇 하나에 도형 다섯을 자식으로 넣습니다.
        ///
        /// <b>이 차시의 핵심은 «묶으면 한 몸처럼 움직인다»</b> 입니다.
        /// 그래서 <c>CubeSpinner</c> 는 <b>부모 <c>Car</c> 하나에만</b> 붙습니다.
        /// 바퀴 네 개에 각각 붙이지 않습니다.
        /// </summary>
        static GameObject BuildCar()
        {
            GameObject car = BuildKit.Empty("Car").At(BuildBasicsLayout.CarOrigin);

            // 몸체는 색을 안 입힙니다.
            // 15차시 실습 ③ 3단계가 «틀 안의 Body 에 색을 입혀 다섯 대를 한꺼번에 바꾸는» 실습이라,
            // 지금 칠해두면 그 차시의 «전부 바뀌었다» 순간이 안 보입니다.
            BuildKit.Shape("Body", PrimitiveType.Cube, car.transform)
                    .At(new Vector3(0f, 0.5f, 0f), scale: new Vector3(2f, 0.5f, 1f));

            // 바퀴는 어둡게 칠해둡니다. 교안이 색을 정해두지 않은 부분이고,
            // 안 칠하면 몸체와 같은 흰색이라 «자동차처럼 보이기만 하면 성공» 이 안 됩니다.
            Material rubber = BuildKit.Mat("Wheel", new Color(0.18f, 0.18f, 0.2f));

            foreach ((string name, Vector3 pos) in Wheels)
            {
                BuildKit.Shape(name, PrimitiveType.Cylinder, car.transform)
                        .At(pos, WheelRotation, WheelScale)
                        .Paint(rubber);
            }

            return car;
        }

        // ══════════════════════════════════════════════════════
        //  실습 ④ — 버튼으로 조종하기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 버튼 <b>두 개</b>를 만들어 <c>StopSpin</c> · <c>StartSpin</c> 에 겁니다.
        ///
        /// <b>왜 둘인가</b> — 멈추기만 있으면 <b>다시 돌릴 방법이 없습니다.</b>
        /// ▶ 를 껐다 켜야 하죠. 교안 실습 ④도 하나 더 만들라고 안내합니다.
        /// 19차시에서 이 패널이 <b>버튼 세 개</b>(시작 · 멈춤 · 색 바꾸기)로 자랍니다.
        ///
        /// 교안은 이 버튼을 <c>UI ▸ Button - TextMeshPro</c> 로 만들게 하고,
        /// <b>판(Canvas)이 저절로 생기는 것</b>을 *"놀라지 마세요"* 로 넘깁니다.
        /// 판을 제대로 다루는 건 17차시입니다.
        ///
        /// 그래서 여기서는 <b>화면 크기 맞춤을 켜지 않습니다.</b>
        /// 그건 17차시 실습 ②에서 손으로 하는 일이라, 미리 해두면 그 차시가 빈 차시가 됩니다.
        /// </summary>
        static void BuildSpinButtons(CubeSpinner spinner)
        {
            Canvas canvas = BuildKitUi.Root(scaleWithScreen: false);
            Transform group = BuildKitUi.Group(canvas, BuildBasicsLayout.UiGroup.L14);

            Vector2 size = BuildBasicsLayout.SideButton;
            Vector2 top = BuildBasicsLayout.StopButton;

            Button stop = BuildKitUi.Button("StopButton", "멈춤", group, top, size);
            stop.Anchor(BuildBasicsLayout.MiddleLeft);
            BuildKit.Wire(stop.onClick, spinner.StopSpin);

            // 바로 아래에 한 칸 띄워 놓습니다. 16차시 버튼 네 개가 그 아래로 이어집니다.
            Vector2 below = new Vector2(top.x, top.y - (size.y + 10f));

            Button start = BuildKitUi.Button("StartButton", "다시 돌리기", group, below, size);
            start.Anchor(BuildBasicsLayout.MiddleLeft);
            BuildKit.Wire(start.onClick, spinner.StartSpin);

            BuildBasicsLayout.ApplyUiGroups(canvas, 14);
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            GameObject car = GameObject.Find("Car");
            int children = car != null ? car.transform.childCount : -1;

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 14차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    $"Car 의 자식이 {children}개입니다. (몸체 1 + 바퀴 4 = 5개면 정상)\n" +
                    "▶ 를 누르면 자동차가 통째로 돌고, «멈춤» 을 누르면 멈춰야 합니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 14차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
