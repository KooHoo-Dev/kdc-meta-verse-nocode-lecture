using System.Collections.Generic;
using NoCodeKit.EditorTools;
using Unity.XR.CoreUtils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 28차시 — VR 화면이 뜨게 하기.
    ///
    /// 교안 `28_xr_setup_nocode.md` 에서 <b>씬에 남는 것</b>만 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ③ 1단계 원래 카메라 치우기</item>
    /// <item>실습 ③ 2단계 <b><c>XR Origin (XR Rig)</c> 놓기</b> — VR 속의 «나»</item>
    /// <item>실습 ③ 3단계 <b><c>XR Interaction Simulator</c> 놓기</b> — 고글 흉내</item>
    /// <item>실습 ③ 4단계 바닥</item>
    /// </list>
    ///
    /// <b>실습 ① · ②는 씬에 안 남습니다.</b>
    /// ①은 «지금은 VR이 아니다» 를 확인하는 것,
    /// ②는 <b>Package Manager 에서 부품을 받아오고 XR Plug-in Management 를 켜는</b> 프로젝트 설정입니다.
    /// <b>씬이 아니라 프로젝트에 남는 일</b>이라 빌더가 만들 것이 없습니다.
    ///
    /// <b>이 차시는 빈 씬에서 시작합니다.</b> 교안이 *"반드시 새 씬을 만들어서"* 라고 못박아 뒀습니다.
    /// </summary>
    public static class Build28XrSetup
    {
        const string StartScene = "28_시작";
        const string CompleteScene = "28_완성";

        [MenuItem("Tools/교안 씬 빌더/28차시 — VR 화면이 뜨게 하기", false, 28)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "28차시 씬 만들기",
                    $"지금 열려 있는 씬을 닫고 {StartScene} · {CompleteScene} 을 새로 만듭니다.\n" +
                    "이미 있으면 덮어씁니다.\n\n" +
                    "저장 안 된 작업이 있으면 먼저 저장해주세요.",
                    "만들기", "그만두기"))
            {
                return;
            }

            Run();
        }

        /// <summary>확인 창 없이 바로 만듭니다. 「VR 전체 다시 만들기」가 부릅니다.</summary>
        public static void Run()
        {
            BuildKit.BeginFromEmpty(StartScene);

            RemoveDefaultCamera();
            BuildRoom();

            BuildKit.PlaceSamplePrefab(BuildXrLayout.RigPrefab, BuildXrLayout.RigPosition);
            BuildKit.PlaceSamplePrefab(BuildXrLayout.SimulatorPrefab, Vector3.zero);

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ 1단계 — 원래 카메라 치우기
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 새 씬에 딸려온 <c>Main Camera</c> 를 지웁니다.
        ///
        /// <b>눈이 두 개면 어느 쪽으로 볼지 헷갈립니다.</b>
        /// 이제 <c>XR Origin</c> 이 자기 눈을 가지고 옵니다.
        /// </summary>
        static void RemoveDefaultCamera()
        {
            Camera cam = Camera.main;
            if (cam != null) Object.DestroyImmediate(cam.gameObject);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ 4단계 — 확인할 공간
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 바닥과 기둥 몇 개를 놓습니다.
        ///
        /// 교안은 바닥만 말하지만 <b>이유가 그대로 기둥에도 적용됩니다</b> —
        /// *"바닥이 없으면 허공이라 확인이 어렵습니다"*.
        ///
        /// 회색 바닥만 있으면 <b>둘러봐도 걸어도 움직이는지 알 수가 없습니다.</b>
        /// 29차시 실습 ①이 *"걸어 다녀지나요?"* 를 묻는데, <b>볼 게 있어야 답할 수 있습니다.</b>
        /// </summary>
        static void BuildRoom()
        {
            Material floor = BuildKit.Mat("XrFloor", new Color(0.45f, 0.47f, 0.5f));
            Material pillar = BuildKit.Mat("XrPillar", new Color(0.8f, 0.55f, 0.3f));

            BuildKit.Shape("Ground", PrimitiveType.Plane)
                    .At(Vector3.zero, scale: BuildXrLayout.GroundScale)
                    .Paint(floor);

            Vector3[] spots = BuildXrLayout.Pillars;

            for (int i = 0; i < spots.Length; i++)
            {
                BuildKit.Shape($"Pillar_{i + 1}", PrimitiveType.Cube)
                        .At(spots[i], scale: BuildXrLayout.PillarScale)
                        .Paint(pillar);
            }
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            var origin = Object.FindFirstObjectByType<XROrigin>();
            bool simulator = GameObject.Find(BuildXrLayout.SimulatorPrefab) != null;
            bool oldCamera = Object.FindFirstObjectByType<Camera>() != null
                             && origin != null && origin.Camera != null;

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 28차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    $"VR 속의 나(XR Origin): {(origin != null ? "있음" : "❌ 없음")}\n" +
                    $"고글 흉내 부품(Simulator): {(simulator ? "있음" : "❌ 없음")}\n" +
                    $"눈은 XR Origin 안의 카메라 하나: {(oldCamera ? "네" : "확인 필요")}\n" +
                    "▶ 를 누르고 **Game 창을 한 번 클릭**한 뒤,\n" +
                    "마우스 오른쪽 버튼을 누른 채로 움직여보세요. 시점이 따라 돌아야 합니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 28차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
