using System.Collections.Generic;
using NoCodeKit.EditorTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 23차시 — 표적이 날아오게.
    ///
    /// 교안 `23_clay_target_nocode.md` 의 실습 ① ~ ③ 을 그대로 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ① 접시 틀 만들기 (이름표 `Target` · 프리팹)</item>
    /// <item>실습 ② 발사기 + "한 발 쏘기" 버튼으로 값 맞추기</item>
    /// <item>실습 ③ 발판을 밟으면 시작 (`Is Trigger` · `On Game Start`)</item>
    /// </list>
    ///
    /// 실습 ④(난이도 미리 맛보기)는 값만 바꿔보는 것이라 만들 게 없습니다.
    /// </summary>
    public static class Build23ClayTarget
    {
        const string FromScene = "22_완성";
        const string StartScene = "23_시작";
        const string CompleteScene = "23_완성";

        [MenuItem("Tools/교안 씬 빌더/23차시 — 표적이 날아오게", false, 23)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "23차시 씬 만들기",
                    $"{FromScene} 을 복제해 {StartScene} · {CompleteScene} 을 만듭니다.\n" +
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

            BuildKit.BeginFrom(FromScene, StartScene);

            GameObject targetPrefab = BuildTargetPrefab();
            TargetLauncher launcher = BuildLauncher(targetPrefab);
            BuildLaunchButton(launcher);
            BuildStartZone(launcher);

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ① — 접시 틀 만들기
        // ══════════════════════════════════════════════════════

        static GameObject BuildTargetPrefab()
        {
            // 이름표는 그것을 다는 것을 만들기 전에 등록해야 합니다.
            BuildKit.EnsureTag("Target");

            Material clay = BuildKit.Mat("Target", new Color(0.95f, 0.45f, 0.12f));   // 눈에 띄는 주황

            GameObject dish = BuildKit.Shape("Target", PrimitiveType.Cylinder)
                                      .At(new Vector3(0f, 1f, 0f), scale: BuildConfig.TargetScale)
                                      .Paint(clay);
            dish.tag = "Target";

            var body = dish.AddComponent<Rigidbody>();   // 날아가려면 물리를 따라야 합니다 (21차시)

            // 총알이 뚫고 지나가지 않게. 총알 쪽은 Continuous Dynamic 이라
            // 표적이 Continuous 이기만 하면 서로 훑어봅니다.
            body.collisionDetectionMode = CollisionDetectionMode.Continuous;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            EditorUtility.SetDirty(body);

            var life = BuildKit.Add<TargetLife>(dish);
            life.lifeTime = BuildConfig.TargetLifeTime;
            EditorUtility.SetDirty(life);

            // 틀로 저장하고 씬의 원본은 지웁니다. 틀만 있으면 됩니다 (15차시).
            return BuildKit.SaveAsPrefab(dish, "Target");
        }

        // ══════════════════════════════════════════════════════
        //  실습 ② — 발사기
        // ══════════════════════════════════════════════════════

        static TargetLauncher BuildLauncher(GameObject targetPrefab)
        {
            Material metal = BuildKit.Mat("Launcher", new Color(0.30f, 0.32f, 0.36f));

            GameObject root = BuildKit.Empty("Launcher").At(new Vector3(0f, 0f, 15f));

            BuildKit.Shape("LauncherBase", PrimitiveType.Cube, root.transform)
                    .At(Vector3.zero, scale: new Vector3(1f, 0.5f, 1f))
                    .Paint(metal);

            // Rotation Y = 180 이라야 플레이어 쪽을 향합니다. X = -45 는 위로 쏘는 각도입니다.
            GameObject point = BuildKit.Empty("LaunchPoint", root.transform)
                                       .At(new Vector3(0f, 0.5f, 0f), rot: new Vector3(-45f, 180f, 0f));

            var launcher = BuildKit.Add<TargetLauncher>(root);
            launcher.targetPrefab = targetPrefab;
            launcher.launchPoint = point.transform;
            launcher.interval = BuildConfig.LaunchInterval;
            launcher.launchForce = BuildConfig.LaunchForce;
            launcher.spreadAngle = BuildConfig.SpreadAngle;
            EditorUtility.SetDirty(launcher);

            return launcher;
        }

        /// <summary>실습 ② 6번 — 값을 맞출 때 쓰는 "한 발 쏘기" 버튼입니다.</summary>
        static void BuildLaunchButton(TargetLauncher launcher)
        {
            Canvas canvas = BuildKitUi.Root();

            Button button = BuildKitUi.Button("LaunchOneButton", "한 발 쏘기",
                                              canvas.transform,
                                              new Vector2(-140f, 60f), new Vector2(220f, 70f));
            button.Anchor(new Vector2(1f, 0f));   // 오른쪽 아래

            BuildKit.Wire(button.onClick, launcher.LaunchOne);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ — 발판을 밟으면 시작
        // ══════════════════════════════════════════════════════

        static void BuildStartZone(TargetLauncher launcher)
        {
            Material pad = BuildKit.Mat("StartZone", new Color(0.95f, 0.85f, 0.20f));   // 노란 발판

            GameObject zone = BuildKit.Shape("StartZone", PrimitiveType.Cube)
                                      .At(new Vector3(0f, 0.1f, -15f), scale: new Vector3(3f, 0.2f, 3f))
                                      .Paint(pad);

            // 밟고 지나갈 수 있게 통과시킵니다 (21차시).
            var box = zone.GetComponent<BoxCollider>();
            box.isTrigger = true;
            EditorUtility.SetDirty(box);

            // 교안이 특히 짚는 곳 — GameStarter 는 GameManager 가 아니라 발판에 붙습니다.
            var starter = BuildKit.Add<GameStarter>(zone);
            starter.triggerTag = "Player";
            starter.startOnce = true;
            EditorUtility.SetDirty(starter);

            BuildKit.Wire(starter.onGameStart, launcher.StartLaunching);

            // 25 · 26차시에서 채웁니다. 지금은 빈 그릇입니다.
            BuildKit.Empty("GameManager");
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
                    $"✅ 23차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    "▶ 를 누르고 ① 오른쪽 아래 '한 발 쏘기' 버튼으로 한 발씩 확인 " +
                    "② 노란 발판을 밟으면 자동으로 날아오기 시작합니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 23차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
