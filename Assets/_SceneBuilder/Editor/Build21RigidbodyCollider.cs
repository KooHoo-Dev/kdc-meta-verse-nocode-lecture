using System.Collections.Generic;
using NoCodeKit.EditorTools;
using UnityEditor;
using UnityEngine;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 21차시 — 떨어뜨리고, 부딪히면 반응하기. <b>갈래 A의 마지막 차시입니다.</b>
    ///
    /// 교안 `21_rigidbody_collider_nocode.md` 에서 <b>씬에 남는 것</b>만 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ① 바닥과 떨어지는 공 (<c>Rigidbody</c>)</item>
    /// <item>실습 ③ <b>바닥에 닿으면 빨개지게</b> — 이름표 + <c>CollisionReporter</c> + <c>On Hit</c></item>
    /// <item>실습 ④ <b>통과하는데 감지는 되는</b> 영역 (<c>Is Trigger</c>)</item>
    /// </list>
    ///
    /// <b>실습 ②는 만들지 않습니다.</b> <c>Collider</c> 의 <c>Radius</c> 를 키웠다 되돌리는 실습입니다.
    ///
    /// 여기서 만든 «닿으면 반응한다» 가 <b>22차시부터 만들 사격장의 뼈대</b>입니다.
    /// 총알이 표적에 맞으면 터지고 점수가 오르는 것 — 구조가 똑같습니다.
    /// </summary>
    public static class Build21RigidbodyCollider
    {
        const string FromScene = "20_완성";
        const string StartScene = "21_시작";
        const string CompleteScene = "21_완성";

        const string GroundTag = "Ground";
        const string GateTag = "Gate";

        [MenuItem("Tools/교안 씬 빌더/21차시 — 떨어뜨리고 부딪히기", false, 21)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "21차시 씬 만들기",
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

            BuildKit.EnsureTag(GroundTag);
            BuildKit.EnsureTag(GateTag);

            BuildGround();
            GameObject ball = BuildBall();
            MakeItReact(ball);
            BuildGate();

            // 화면 요소를 안 만드는 차시입니다. 20차시 상태를 그대로 둡니다.
            BuildBasicsLayout.ApplyUiGroups(BuildKitUi.Root(), 21);

            BuildBasicsLayout.CameraFor(21, out Vector3 from, out Vector3 lookAt);
            BuildKit.AimCamera(from, lookAt);

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ① — 바닥과 공
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 바닥을 깝니다. 교안은 <c>Scale 2,1,2</c> 를 예로 들지만 *"좁으면 키우세요"* 라고 했고,
        /// 여기는 <b>15차시 자동차가 <c>X 12</c> 까지 늘어서 있어서</b> 3 이라야 다 받칩니다.
        ///
        /// 이름표 <c>Ground</c> 를 <b>실제로 달아둡니다.</b>
        /// 교안 실습 ③이 *"3번까지만 하면 만들기만 한 것"* 이라고 못박는 그 함정입니다.
        /// </summary>
        static void BuildGround()
        {
            // 재질 이름을 «Ground» 로 하면 안 됩니다.
            // 재질 폴더는 갈래끼리 나누지 않는데(`plans/10`), 22차시가 이미 «Ground» 를 씁니다.
            // 같은 이름을 부르면 BuildKit.Mat 이 그걸 그대로 돌려줘서 «사격장 잔디» 가 깔립니다.
            Material mat = BuildKit.Mat("Floor", new Color(0.55f, 0.55f, 0.58f));

            GameObject ground = BuildKit.Shape("Ground", PrimitiveType.Plane)
                                        .At(Vector3.zero, scale: BuildBasicsLayout.GroundScale)
                                        .Paint(mat);

            ground.tag = GroundTag;
            EditorUtility.SetDirty(ground);
        }

        /// <summary>
        /// 공중에 뜬 공에 <c>Rigidbody</c> 를 붙입니다. 높이는 교안대로 <c>5</c> 입니다.
        ///
        /// 교안 실습 ① 6번의 <b>«무게가 달라도 똑같이 떨어진다»</b> 비교용 복제본은 안 만듭니다.
        /// 나란히 놓고 확인한 뒤 지우는 실습이라서요.
        /// </summary>
        static GameObject BuildBall()
        {
            Material mat = BuildKit.Mat("Ball", new Color(0.95f, 0.95f, 0.95f));

            GameObject ball = BuildKit.Shape("Ball", PrimitiveType.Sphere)
                                      .At(BuildBasicsLayout.Ball)
                                      .Paint(mat);

            ball.AddComponent<Rigidbody>();
            EditorUtility.SetDirty(ball);
            return ball;
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ — 바닥에 닿으면 반응하기 (핵심)
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// <b>닿는 쪽(공)</b>이 알아채고, <b>닿으면 색을 바꾸라</b>고 연결합니다.
        ///
        /// 색을 바꾸는 일은 14차시부터 쓰던 <c>CubeSpinner</c> 가 합니다.
        /// <b>돌면 안 되니 <c>Is Spinning</c> 은 끕니다</b> — 교안 실습 ③ 9번 그대로입니다.
        ///
        /// <c>On Hit</c> 은 버튼의 <c>On Click</c> · 19차시의 <c>On Stopped</c> 와 <b>똑같이 생겼습니다.</b>
        /// 이 강의의 결론이 여기 다시 나옵니다 — <b>«무슨 일이 일어났을 때, 무엇을 할지»</b>.
        /// </summary>
        static void MakeItReact(GameObject ball)
        {
            var spinner = BuildKit.Add<CubeSpinner>(ball);
            spinner.isSpinning = false;
            spinner.targetColor = new Color(0.9f, 0.2f, 0.2f);
            EditorUtility.SetDirty(spinner);

            var reporter = BuildKit.Add<CollisionReporter>(ball);
            reporter.targetTag = GroundTag;
            reporter.reportToConsole = true;
            EditorUtility.SetDirty(reporter);

            BuildKit.Wire(reporter.onHit, spinner.ChangeColor);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ④ — 통과하면서 감지하기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 공이 떨어지는 길목에 <b>통과되는 판</b>을 놓습니다.
        ///
        /// <c>Is Trigger</c> 를 켜면 <b>막지는 않고 «지나갔다» 만 알려줍니다.</b>
        /// 골인 지점 · 함정 영역 · 아이템 줍기가 전부 이 방식이고,
        /// <b>23차시 시작 발판 · 25차시 표적 명중</b>이 바로 이걸 씁니다.
        ///
        /// <b>다만 공의 <c>Target Tag</c> 는 <c>Ground</c> 로 둡니다.</b>
        /// 교안 실습 ④ 7번은 이걸 <c>Gate</c> 로 바꾸게 하는데,
        /// 그러면 <b>이 차시의 핵심인 «바닥에 닿으면 빨개진다» 가 정답 씬에서 사라집니다.</b>
        /// 판만 놓아두면 <b>통과하는 것도 보이고 바닥 반응도 남습니다.</b>
        /// 바꿔보는 건 수강생이 직접 하시면 됩니다.
        /// </summary>
        static void BuildGate()
        {
            Material mat = BuildKit.Mat("Gate", new Color(0.35f, 0.75f, 0.9f, 0.5f));

            GameObject gate = BuildKit.Shape("Gate", PrimitiveType.Cube)
                                      .At(BuildBasicsLayout.Gate,
                                          scale: BuildBasicsLayout.GateScale)
                                      .Paint(mat);

            gate.tag = GateTag;
            gate.GetComponent<BoxCollider>().isTrigger = true;
            EditorUtility.SetDirty(gate);
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            var reporter = Object.FindFirstObjectByType<CollisionReporter>();
            int wired = reporter != null ? reporter.onHit.GetPersistentEventCount() : -1;

            GameObject ground = GameObject.Find("Ground");
            bool tagged = ground != null && ground.CompareTag(GroundTag);

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 21차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다. **갈래 A(13 ~ 21차시)가 여기서 끝납니다.**\n" +
                    $"바닥 이름표(Ground): {(tagged ? "달림" : "❌ 안 달림")}\n" +
                    $"On Hit 연결: {wired}개 (1개면 정상)\n" +
                    "▶ 를 누르면 공이 파란 판을 «통과해» 바닥까지 떨어지고,\n" +
                    "닿는 순간 «빨개지면서» Console 에 줄이 하나 떠야 합니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 21차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
