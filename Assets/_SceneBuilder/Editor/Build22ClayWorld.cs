using System.Collections.Generic;
using NoCodeKit.EditorTools;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 22차시 — 사격장을 만들고 걸어 다니기.
    ///
    /// 교안 `22_clay_world_nocode.md` 의 실습 ① ~ ③ 을 그대로 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ① 바닥과 울타리로 닫힌 공간</item>
    /// <item>실습 ② 안 넘어지는 몸 (Freeze Rotation)</item>
    /// <item>실습 ③ 걸어 다니고 둘러보기 (PlayerMover · MouseLook)</item>
    /// </list>
    ///
    /// 실습 ④(조작감 맞추기)는 값만 바꾸는 것이라 따로 만들 게 없습니다.
    /// </summary>
    public static class Build22ClayWorld
    {
        const string StartScene = "22_시작";
        const string CompleteScene = "22_완성";

        [MenuItem("Tools/교안 씬 빌더/22차시 — 사격장을 만들고 걸어 다니기", false, 22)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "22차시 씬 만들기",
                    $"지금 열려 있는 씬을 닫고 {StartScene} · {CompleteScene} 을 새로 만듭니다.\n" +
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

            // 22차시는 새 씬에서 시작합니다. 교안 실습 ① 1번.
            BuildKit.BeginFromEmpty(StartScene);

            BuildRange();
            GameObject player = BuildPlayer();
            AttachEye(player);

            BuildKit.SaveComplete(CompleteScene);

            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ① — 사격장 짓기
        // ══════════════════════════════════════════════════════

        static void BuildRange()
        {
            Material grass = BuildKit.Mat("Ground", new Color(0.34f, 0.46f, 0.28f));
            Material wood = BuildKit.Mat("Fence", new Color(0.45f, 0.33f, 0.22f));

            BuildKit.Shape("Ground", PrimitiveType.Plane)
                    .At(Vector3.zero, scale: new Vector3(BuildConfig.GroundScale, 1f, BuildConfig.GroundScale))
                    .Paint(grass);

            float h = BuildConfig.FenceHeight;
            float half = BuildConfig.FenceHalf;
            float len = half * 2f;
            float y = h * 0.5f;

            BuildKit.Shape("Fence_Back", PrimitiveType.Cube)
                    .At(new Vector3(0f, y, half), scale: new Vector3(len, h, 1f))
                    .Paint(wood);

            BuildKit.Shape("Fence_Left", PrimitiveType.Cube)
                    .At(new Vector3(-half, y, 0f), scale: new Vector3(1f, h, len))
                    .Paint(wood);

            BuildKit.Shape("Fence_Right", PrimitiveType.Cube)
                    .At(new Vector3(half, y, 0f), scale: new Vector3(1f, h, len))
                    .Paint(wood);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ② — 안 넘어지는 몸
        // ══════════════════════════════════════════════════════

        static GameObject BuildPlayer()
        {
            BuildKit.EnsureTag("Player");   // 유니티 기본 이름표라 보통은 아무 일도 안 합니다

            GameObject player = BuildKit.Shape("Player", PrimitiveType.Capsule)
                                        .At(new Vector3(0f, 1f, BuildConfig.PlayerStartZ));
            player.tag = "Player";

            // PlayerMover 가 Rigidbody 를 요구하지만, 회전을 잠그려면 먼저 붙여야 합니다.
            var body = player.AddComponent<Rigidbody>();
            body.constraints = RigidbodyConstraints.FreezeRotation;   // 안 하면 넘어집니다
            EditorUtility.SetDirty(body);

            var mover = BuildKit.Add<PlayerMover>(player);
            mover.moveSpeed = BuildConfig.MoveSpeed;
            mover.jumpForce = BuildConfig.JumpForce;
            mover.canMove = true;
            EditorUtility.SetDirty(mover);

            return player;
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ — 눈을 달고 둘러보기
        // ══════════════════════════════════════════════════════

        static void AttachEye(GameObject player)
        {
            Camera cam = Object.FindFirstObjectByType<Camera>();
            if (cam == null)
            {
                Debug.LogError("빈 씬에 Main Camera 가 없습니다. 씬 만들기가 잘못됐습니다.");
                return;
            }

            // 몸의 자식으로 넣습니다. 부모가 움직이면 자식이 따라옵니다 (14차시).
            cam.transform.SetParent(player.transform, false);
            cam.transform.localPosition = new Vector3(0f, BuildConfig.EyeHeight, 0f);
            cam.transform.localRotation = Quaternion.identity;
            EditorUtility.SetDirty(cam.gameObject);

            var look = BuildKit.Add<MouseLook>(cam.gameObject);
            look.sensitivity = BuildConfig.Sensitivity;
            look.lockCursor = true;
            EditorUtility.SetDirty(look);
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
                    $"✅ 22차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    "▶ 를 눌러 W A S D 로 걸어보고 마우스로 둘러보세요. (커서는 Esc 로 되찾습니다)");
                return;
            }

            Debug.LogWarning($"⚠️ 22차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
