using System.Collections.Generic;
using NoCodeKit.EditorTools;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using Object = UnityEngine.Object;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 32차시 — VR로 총을 잡고 쏘기.
    ///
    /// 교안 `32_vr_clay_shooting_nocode.md` 의 실습 ① ~ ④ 를 그대로 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ① 몸을 바꿔 끼우기 — <c>Player</c> ▸ <c>XR Origin</c> · 바닥을 순간이동 목적지로</item>
    /// <item>실습 ② 총을 손에 쥐기 — <c>MouseShooter</c> 떼고 <c>XR Grab Interactable</c></item>
    /// <item>실습 ③ <b>방아쇠</b> — <c>Activated</c> ▸ <c>SimpleGun.Fire</c></item>
    /// <item>실습 ④ 화면을 세상 안으로 — <c>World Space</c> 판 · 3D 버튼</item>
    /// </list>
    ///
    /// <b>새로 만드는 부품이 없습니다.</b> XRI 기본 부품과 22 ~ 27차시 부품만 씁니다.
    /// </summary>
    public static class Build32VrClayShooting
    {
        const string FromScene = "27_완성";
        const string StartScene = "32_시작";
        const string CompleteScene = "32_완성";

        [MenuItem("Tools/교안 씬 빌더/32차시 — VR로 총을 잡고 쏘기", false, 32)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "32차시 씬 만들기",
                    $"{FromScene} 을 복제해 {StartScene} · {CompleteScene} 을 만듭니다.\n" +
                    "이미 있으면 덮어씁니다.\n\n" +
                    "저장 안 된 작업이 있으면 먼저 저장해주세요.",
                    "만들기", "그만두기"))
            {
                return;
            }

            Run();
        }

        /// <summary>확인 창 없이 바로 만듭니다. 「VR 사격 전체 다시 만들기」가 부릅니다.</summary>
        public static void Run()
        {
            BuildKit.BeginFrom(FromScene, StartScene);

            GameObject gun = SwapBody();
            MakeGunGrabbable(gun);
            WireTrigger(gun);
            MoveScreenIntoWorld();

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ① — 몸을 바꿔 끼우기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 캡슐로 된 몸을 <c>XR Origin</c> 으로 바꿔 끼웁니다.
        ///
        /// 🚨 <b>총을 먼저 꺼내야 합니다.</b>
        /// <c>Gun</c> 은 <c>Player ▸ Main Camera</c> 안에 들어 있습니다.
        /// 그대로 <c>Player</c> 를 지우면 <b>총도 통째로 사라집니다.</b>
        /// 14차시의 «부모가 사라지면 자식도 사라진다» 가 여기서 물립니다.
        /// </summary>
        static GameObject SwapBody()
        {
            GameObject gun = GameObject.Find("Gun");
            if (gun == null)
            {
                throw new System.InvalidOperationException(
                    "24차시에서 만든 총(Gun)이 없습니다. 22 ~ 27차시부터 다시 만들어주세요.");
            }

            // 총을 먼저 밖으로. 이 한 줄을 빼면 아래에서 총이 같이 지워집니다.
            gun.transform.SetParent(null, true);

            GameObject player = GameObject.Find("Player");
            if (player != null) Object.DestroyImmediate(player);

            TidyUpAfterPlayer();

            BuildKit.PlaceSamplePrefab(BuildXrLayout.RigPrefab, BuildVrClayLayout.RigPosition);
            BuildKit.PlaceSamplePrefab(BuildXrLayout.SimulatorPrefab, Vector3.zero);

            MakeFloorTeleportable();
            return gun;
        }

        /// <summary>
        /// <c>Player</c> 를 지우면서 <b>끊어진 줄</b>을 정리합니다.
        ///
        /// 26차시가 <c>On Game Start</c> 에 여섯 줄을 걸어뒀는데,
        /// 그중 <c>MouseLook.LockCursor</c> 와 <c>PlayerMover.EnableMove</c> 는
        /// <b>방금 지운 몸 안에 있던 것</b>입니다. <c>On Game End</c> 쪽도 마찬가지고요.
        ///
        /// 남겨둬도 돌아가는 데 지장은 없지만
        /// <b>점검기가 «끊어진 연결» 로 짚습니다.</b> 정답 씬이니 치웁니다.
        /// </summary>
        static void TidyUpAfterPlayer()
        {
            var starter = Object.FindFirstObjectByType<GameStarter>();
            if (starter == null) return;

            int dropped = BuildKit.DropBrokenListeners(starter.onGameStart)
                        + BuildKit.DropBrokenListeners(starter.onGameEnd);

            if (dropped > 0)
            {
                Debug.Log($"몸을 바꿔 끼우면서 끊어진 연결 {dropped}줄을 치웠습니다. " +
                          "(마우스 시야 · 캡슐 걷기 — VR 에서는 XR Origin 이 대신합니다)");
            }

            EditorUtility.SetDirty(starter);
        }

        /// <summary>
        /// 사격장 바닥을 <b>순간이동 목적지</b>로 만듭니다.
        ///
        /// 31차시에서 배운 그대로인데, <b>그건 다른 씬이었습니다.</b>
        /// 이 씬은 27차시 사격장에서 출발하므로 바닥에 아무것도 안 붙어 있습니다.
        ///
        /// 사격장이 <b>가로 50미터</b>라 걸어서만 다니면 느립니다.
        /// 33차시에서 사격장이 셋이 되면 더 그렇고요.
        /// </summary>
        static void MakeFloorTeleportable()
        {
            GameObject ground = GameObject.Find("Ground");
            if (ground == null) return;

            int layer = BuildKit.EnsureInteractionLayer(BuildVrClayLayout.TeleportLayer);
            if (layer < 0) return;

            var area = BuildKit.Ensure<TeleportationArea>(ground);

            area.interactionLayers = 1 << layer;
            EditorUtility.SetDirty(area);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ② — 총을 손에 쥐기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 마우스 부품을 떼고, 잡힐 몸을 만들고, 받침대에 올려둡니다.
        ///
        /// <b>24차시에 나눠둔 값을 여기서 받습니다.</b>
        /// <c>MouseShooter</c> 만 떼면 <c>SimpleGun</c> 은 손도 안 대도 됩니다.
        /// </summary>
        static void MakeGunGrabbable(GameObject gun)
        {
            var shooter = gun.GetComponent<MouseShooter>();
            if (shooter != null) Object.DestroyImmediate(shooter);

            // 24차시에서 지웠던 «부딪히는 몸» 을 손잡이 쪽에만 다시 붙입니다.
            var box = BuildKit.Ensure<BoxCollider>(gun);
            box.center = Vector3.zero;
            box.size = BuildVrClayLayout.GunColliderSize;
            EditorUtility.SetDirty(box);

            var grab = BuildKit.Ensure<XRGrabInteractable>(gun);
            grab.selectMode = InteractableSelectMode.Single;   // 한 손 (plans/08 §6-3)
            grab.throwOnDetach = false;                        // 총은 던지는 물건이 아닙니다
            EditorUtility.SetDirty(grab);

            Material wood = BuildKit.Mat("GunStand", new Color(0.45f, 0.33f, 0.22f));

            BuildKit.Shape("GunStand", PrimitiveType.Cube)
                    .At(BuildVrClayLayout.GunStand, scale: BuildVrClayLayout.GunStandScale)
                    .Paint(wood);

            gun.At(BuildVrClayLayout.GunOnStand, rot: Vector3.zero);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ — 방아쇠 (핵심)
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// <b>«썼다» 칸에 «쏘기» 를 겁니다.</b> 오늘의 봉우리입니다.
        ///
        /// 31차시가 예고해둔 그대로 —
        /// *"«잡았다» 로 총을 쥐고, 안 쓴 «썼다» 로 방아쇠를 당깁니다."*
        ///
        /// <c>Activated</c> 는 «무엇이 어떻게 쓰였는지» 를 같이 넘겨주는 칸이라
        /// 인자 없는 <c>Fire</c> 를 걸려면 <see cref="BuildKit.WireVoid"/> 를 씁니다.
        /// </summary>
        static void WireTrigger(GameObject gun)
        {
            var grab = gun.GetComponent<XRGrabInteractable>();
            var simple = gun.GetComponent<SimpleGun>();

            if (grab == null || simple == null)
            {
                throw new System.InvalidOperationException(
                    "총에 SimpleGun 이나 XR Grab Interactable 이 없습니다.");
            }

            BuildKit.WireVoid(grab.activated, simple.Fire);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ④ — 화면을 세상 안으로
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 화면에 붙어 있던 것들을 <b>세상 안</b>으로 옮깁니다.
        ///
        /// 🚨 <c>Screen Space - Overlay</c> 는 <b>실제 고글에서 렌더링되지 않습니다.</b>
        /// 고글 흉내 부품의 미리보기 창에서는 보여서 30차시까지 문제가 없었지만,
        /// 32차시는 완성본이라 여기서 정리합니다. (`plans/08` §6-1)
        /// </summary>
        static void MoveScreenIntoWorld()
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null) return;

            var origin = Object.FindFirstObjectByType<XROrigin>();
            Camera eye = origin != null ? origin.Camera : null;

            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = eye;

            Transform board = BuildScoreBoard();

            var rt = canvas.GetComponent<RectTransform>();
            rt.SetParent(board, false);
            rt.sizeDelta = BuildVrClayLayout.BoardSize;
            rt.localPosition = BuildVrClayLayout.BoardCanvas;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one * BuildVrClayLayout.BoardScale;
            EditorUtility.SetDirty(canvas);

            // VR 은 십자선을 안 씁니다. 총구가 곧 조준선이에요.
            GameObject crosshair = GameObject.Find("Crosshair");
            if (crosshair != null) Object.DestroyImmediate(crosshair);

            HideStartPanel();
            BuildTouchButtons();
        }

        /// <summary>
        /// 화면에 붙어 있던 시작 화면은 <b>지우지 않고 꺼둡니다.</b>
        /// 되돌리고 싶을 수 있고, 26차시가 걸어둔 연결도 그대로 남습니다.
        /// </summary>
        static void HideStartPanel()
        {
            GameObject panel = GameObject.Find("StartPanel");
            if (panel == null) return;

            panel.SetActive(false);
            EditorUtility.SetDirty(panel);
        }

        /// <summary>
        /// 시작 · 난이도를 <b>손으로 누르는 3D 물건</b>으로 만듭니다.
        ///
        /// 커리큘럼 원문이 *"게임 시작 <b>오브젝트</b>"* 라고 적어둔 그것입니다.
        ///
        /// <b><c>Grab</c> 이 아니라 <c>Simple</c> 입니다.</b>
        /// <c>Grab</c> 을 붙이면 버튼이 <b>손에 딸려옵니다.</b>
        /// </summary>
        static void BuildTouchButtons()
        {
            var starter = Object.FindFirstObjectByType<GameStarter>();
            var preset = Object.FindFirstObjectByType<DifficultyPreset>();

            if (starter == null)
            {
                throw new System.InvalidOperationException(
                    "23차시에서 만든 GameStarter 가 없습니다. 22 ~ 27차시부터 다시 만들어주세요.");
            }

            Transform console = BuildConsole();

            TouchButton(console, "StartButton3D", "시작",
                        BuildVrClayLayout.StartButton,
                        BuildVrClayLayout.ButtonScale, BuildVrClayLayout.LabelSize,
                        new Color(0.25f, 0.70f, 0.35f), starter.StartGame);

            if (preset == null) return;

            (string name, string label, Color color, UnityEngine.Events.UnityAction call)[] levels =
            {
                ("EasyButton3D",   "쉬움",   new Color(0.35f, 0.72f, 0.45f), preset.ApplyEasy),
                ("NormalButton3D", "보통",   new Color(0.80f, 0.72f, 0.28f), preset.ApplyNormal),
                ("HardButton3D",   "어려움", new Color(0.80f, 0.32f, 0.30f), preset.ApplyHard),
            };

            for (int i = 0; i < levels.Length; i++)
            {
                (string name, string label, Color color, UnityEngine.Events.UnityAction call) = levels[i];

                TouchButton(console, name, label,
                            BuildVrClayLayout.DifficultyButtons[i],
                            BuildVrClayLayout.SmallButtonScale, BuildVrClayLayout.SmallLabelSize,
                            color, call);
            }
        }

        /// <summary>
        /// 판을 <b>사람이 읽을 수 있는 방향</b>으로 세웁니다.
        ///
        /// 🚨 <b>«사람 쪽을 보게» 가 아닙니다.</b>
        /// 세상 글자와 화면은 <b>«local +Z 가 보는 방향»</b> 이라,
        /// <b>+Z 를 사람 반대쪽</b>(사람이 보는 방향)으로 돌려야 바로 읽힙니다.
        /// 반대로 돌리면 <b>좌우로 뒤집혀</b> 보입니다. 두 번 틀리고 알아낸 것입니다.
        ///
        /// 각도를 손으로 넣지 않고 <b>사람이 선 자리에서 계산</b>합니다.
        /// 리그를 옮겨도 알아서 따라 돕니다.
        /// </summary>
        static void FaceThePlayer(Transform what)
        {
            var origin = Object.FindFirstObjectByType<XROrigin>();
            Vector3 stand = origin != null ? origin.transform.position
                                           : BuildVrClayLayout.RigPosition;

            // 사람 → 판 방향. 이게 «사람이 보는 방향» 이고, 판의 +Z 가 이쪽을 봐야 읽힙니다.
            Vector3 lineOfSight = what.position - stand;
            lineOfSight.y = 0f;

            if (lineOfSight.sqrMagnitude < 0.001f) return;

            what.rotation = Quaternion.LookRotation(lineOfSight.normalized);
            EditorUtility.SetDirty(what);
        }

        /// <summary>
        /// <b>점수 게시판</b>을 세웁니다. 기둥 하나에 판 하나 — 실제 사격장처럼요.
        ///
        /// <b>사람 쪽을 보게 돌려둡니다.</b> 각도를 손으로 넣지 않고
        /// <b>사람이 선 자리에서 계산</b>합니다. 리그를 옮겨도 알아서 따라 돕니다.
        ///
        /// 화면(Canvas)은 이 판의 자식으로 들어갑니다.
        /// 그러면 게시판을 통째로 옮겨도 <b>글자가 같이 따라옵니다.</b> 14차시의 그 원리예요.
        /// </summary>
        static Transform BuildScoreBoard()
        {
            GameObject board = BuildKit.Empty("ScoreBoard").At(BuildVrClayLayout.ScoreBoard);
            FaceThePlayer(board.transform);

            Material metal = BuildKit.Mat("BoardMetal", new Color(0.20f, 0.21f, 0.24f));

            BuildKit.Shape("Leg", PrimitiveType.Cube, board.transform)
                    .At(BuildVrClayLayout.BoardLeg, scale: BuildVrClayLayout.BoardLegScale)
                    .Paint(metal)
                    .NoCollider();

            BuildKit.Shape("Panel", PrimitiveType.Cube, board.transform)
                    .At(BuildVrClayLayout.BoardPanel, scale: BuildVrClayLayout.BoardPanelScale)
                    .Paint(metal)
                    .NoCollider();

            return board.transform;
        }

        /// <summary>
        /// 버튼들을 얹을 <b>조작 콘솔</b>을 세웁니다. 기둥 하나에 판 하나.
        ///
        /// <b>왜 판을 세우나</b> — 상자 넷이 허공에 떠 있으면 <b>기계로 안 보입니다.</b>
        /// 그리고 판이 사람 쪽을 보고 있어야 <b>버튼을 겨누기 쉽습니다.</b>
        /// </summary>
        static Transform BuildConsole()
        {
            GameObject console = BuildKit.Empty("ControlConsole").At(BuildVrClayLayout.Console);
            FaceThePlayer(console.transform);

            Material metal = BuildKit.Mat("ConsoleMetal", new Color(0.22f, 0.23f, 0.26f));

            BuildKit.Shape("Leg", PrimitiveType.Cube, console.transform)
                    .At(BuildVrClayLayout.ConsoleLeg, scale: BuildVrClayLayout.ConsoleLegScale)
                    .Paint(metal)
                    .NoCollider();

            BuildKit.Shape("Panel", PrimitiveType.Cube, console.transform)
                    .At(BuildVrClayLayout.ConsolePanel, scale: BuildVrClayLayout.ConsolePanelScale)
                    .Paint(metal)
                    .NoCollider();

            return console.transform;
        }

        /// <summary>
        /// <b>이름이 적힌 3D 버튼</b> 하나를 만듭니다.
        ///
        /// 글자가 없으면 <b>색깔 상자만 늘어서 있어</b> 어느 게 무슨 버튼인지 알 수 없습니다.
        ///
        /// 🚨 <b>글자는 버튼의 «자식» 이 아니라 «형제» 입니다.</b>
        /// 버튼 큐브는 <c>0.44 × 0.20 × 0.12</c> 로 <b>납작하게 눌려</b> 있습니다.
        /// 그 안에 글자를 넣으면 <b>글자도 같이 눌립니다.</b>
        /// 눌린 만큼 되돌리려고 «역수 크기» 를 곱해봤지만,
        /// 그 보정이 회전과 겹치면서 <b>글자가 좌우로 뒤집혔습니다.</b>
        ///
        /// 둘 다 <b>콘솔의 자식</b>으로 나란히 두면 그런 일이 없습니다.
        /// 교안도 같은 방식으로 안내합니다 — 수강생이 따라 하기에도 이쪽이 단계가 적습니다.
        /// </summary>
        static void TouchButton(Transform console, string name, string label,
                                Vector3 where, Vector3 scale, Vector2 labelSize,
                                Color color, UnityEngine.Events.UnityAction call)
        {
            Material mat = BuildKit.Mat(name, color);

            // 큐브가 그대로 «부딪히는 몸» 입니다. 기본으로 붙어 오는 것을 그냥 씁니다.
            GameObject go = BuildKit.Shape(name, PrimitiveType.Cube, console)
                                    .At(where, scale: scale)
                                    .Paint(mat);

            // 글자는 버튼 «앞»(사람 쪽 = 콘솔 기준 −Z)에, 버튼과 나란히.
            // 돌릴 필요가 없습니다 — 콘솔이 이미 읽히는 방향으로 서 있으니까요.
            TextMeshPro text = BuildKit.Label3D(
                $"{name}_Label", label, console, labelSize, Color.white);

            text.transform.localPosition =
                where + new Vector3(0f, 0f, -(scale.z * 0.5f + 0.01f));

            var simple = BuildKit.Ensure<XRSimpleInteractable>(go);
            BuildKit.WireVoid(simple.selectEntered, call);
            EditorUtility.SetDirty(simple);

            // 「제대로 걸렸나」를 버튼마다 찍어둡니다.
            // 안 눌릴 때 «연결 문제인가 겨냥 문제인가» 를 여기서 가릅니다.
            string wiring = BuildKit.Describe(simple, "m_SelectEntered");
            Debug.Log($"[3D 버튼] {name} «{label}» → {wiring}", go);
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            GameObject gun = GameObject.Find("Gun");
            var grab = gun != null ? gun.GetComponent<XRGrabInteractable>() : null;
            bool mouseGone = gun != null && gun.GetComponent<MouseShooter>() == null;
            int fired = grab != null ? grab.activated.GetPersistentEventCount() : -1;

            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            bool world = canvas != null && canvas.renderMode == RenderMode.WorldSpace;

            int buttons = Object.FindObjectsByType<XRSimpleInteractable>(
                FindObjectsInactive.Include, FindObjectsSortMode.None).Length;

            // 「제대로 걸렸나」를 눈으로 확인할 수 있게 찍어줍니다.
            // 버튼이 안 눌릴 때 «연결 문제인가 겨냥 문제인가» 를 여기서 가릅니다.
            GameObject startButton = GameObject.Find("StartButton3D");
            string startWiring = startButton != null
                ? BuildKit.Describe(startButton.GetComponent<XRSimpleInteractable>(), "m_SelectEntered")
                : "(버튼 없음)";
            string triggerWiring = grab != null
                ? BuildKit.Describe(grab, "m_Activated")
                : "(총 없음)";

            var starter = Object.FindFirstObjectByType<GameStarter>();
            int onStart = starter != null ? starter.onGameStart.GetPersistentEventCount() : -1;
            int onEnd = starter != null ? starter.onGameEnd.GetPersistentEventCount() : -1;

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 32차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    $"총 — MouseShooter 떼짐: {(mouseGone ? "네" : "❌ 아직 붙어 있음")}" +
                    $" · 잡히기: {(grab != null ? "됨" : "❌ 안 됨")}" +
                    $" · 한 손: {(grab != null && grab.selectMode == InteractableSelectMode.Single ? "네" : "❌")}\n" +
                    $"방아쇠(Activated) 연결: {fired}개 (1개면 정상)\n" +
                    $"점수판이 세상 안에: {(world ? "네" : "❌ 아직 화면에 붙어 있음")}\n" +
                    $"손으로 누르는 버튼: {buttons}개 (시작 1 + 난이도 3 = 4개면 정상)\n" +
                    $"  시작 버튼 연결 → {startWiring}\n" +
                    $"  방아쇠 연결   → {triggerWiring}\n" +
                    $"On Game Start: {onStart}개 · On Game End: {onEnd}개 (각 4개 · 2개면 정상)\n" +
                    "  — 26차시의 여섯 줄에서 마우스 시야 · 캡슐 걷기 두 줄이 빠진 결과입니다\n" +
                    "\n▶ 를 누른 뒤 이 순서대로 해보세요 —\n" +
                    "  ① 미리보기 창 클릭  ② `]` 로 오른손\n" +
                    "  ③ **초록 시작 버튼**을 겨누고 `G`   ← 손을 대기만 해서는 안 됩니다\n" +
                    "  ④ 총에 손을 대고 `G` 를 «누른 채로»\n" +
                    "  ⑤ 그 상태로 `T`  ← 방아쇠\n\n" +
                    "🚨 순서를 바꾸면 총알이 안 나갑니다. 26차시에 «시작 전에는 못 쏘게» 해뒀거든요.\n" +
                    "   (SimpleGun 의 Can Fire — 시작 버튼이 켜줍니다)");
                return;
            }

            Debug.LogWarning($"⚠️ 32차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
