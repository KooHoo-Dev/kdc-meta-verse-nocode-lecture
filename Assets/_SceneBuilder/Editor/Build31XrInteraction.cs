using System.Collections.Generic;
using NoCodeKit.EditorTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using Object = UnityEngine.Object;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 31차시 — 잡고, 던지고, 순간이동. <b>갈래 C의 마지막 차시입니다.</b>
    ///
    /// 교안 `31_xr_interaction_nocode.md` 에서 <b>씬에 남는 것</b>만 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ② <b>잡히는 큐브</b> (<c>XR Grab Interactable</c>)</item>
    /// <item>실습 ③ 3단계 <b>순간이동 바닥</b> (<c>Teleportation Area</c> + 이름표)</item>
    /// </list>
    ///
    /// <b>실습 ① · ④는 만들지 않습니다.</b>
    /// ①은 손에서 나오는 선을 보는 <b>탐색</b>이고,
    /// ④(잡으면 효과가 터지게)는 <b>효과 파일이 아직 없습니다.</b> (`plans/11`)
    ///
    /// 여기서 만든 <b>«잡았다» · «썼다»</b> 가 32차시에서 총을 쥐고 쏘는 데 그대로 쓰입니다.
    /// </summary>
    public static class Build31XrInteraction
    {
        const string FromScene = "30_완성";
        const string StartScene = "31_시작";
        const string CompleteScene = "31_완성";

        [MenuItem("Tools/교안 씬 빌더/31차시 — 잡고 던지고 순간이동", false, 31)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "31차시 씬 만들기",
                    $"{FromScene} 을 복제해 {StartScene} · {CompleteScene} 을 만듭니다.\n" +
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
            BuildKit.BeginFrom(FromScene, StartScene);

            BuildGrabCube();
            MakeFloorTeleportable();

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ② — 잡히는 물건
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 손에 들 만한 큐브를 놓고 <b>잡히는 물건 부품</b>을 붙입니다.
        ///
        /// <c>XR Grab Interactable</c> 을 붙이면 <b><c>Rigidbody</c> 가 같이 생깁니다.</b>
        /// 교안이 *"놀라지 마세요"* 라고 하는 그 부분인데,
        /// <b>21차시의 그 떨어지는 부품</b>입니다. 잡고 던지려면 물리가 필요하거든요.
        /// </summary>
        static void BuildGrabCube()
        {
            Material mat = BuildKit.Mat("GrabCube", new Color(0.3f, 0.65f, 0.85f));

            // 사람이 선 자리를 기준으로 놓습니다. 월드 좌표로 박아두면
            // 리그가 조금만 옮겨져도 «손이 안 닿는 자리» 가 됩니다.
            var origin = Object.FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();
            Vector3 stand = origin != null ? origin.transform.position
                                           : BuildXrLayout.RigPosition;

            GameObject cube = BuildKit.Shape("GrabCube", PrimitiveType.Cube)
                                      .At(stand + BuildXrLayout.GrabCubeOffset,
                                          scale: BuildXrLayout.GrabCubeScale)
                                      .Paint(mat);

            var grab = cube.AddComponent<XRGrabInteractable>();

            // 교안 실습 ③ 2단계 — «던져지지 않고 툭 떨어진다면 Throw On Detach 를 보세요».
            // 기본값이 켜짐이지만, 정답 씬이니 명시해둡니다.
            grab.throwOnDetach = true;

            EditorUtility.SetDirty(grab);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ 3단계 — 순간이동 바닥
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 바닥을 <b>순간이동 목적지</b>로 만듭니다.
        ///
        /// <b>이름표를 맞추는 게 절반입니다.</b>
        /// 순간이동 선은 <c>Teleport</c> 이름표가 붙은 것만 목적지로 인정합니다.
        /// 부품을 붙이면 이름표가 <c>Default</c> 로 들어와서 <b>선이 빨갛게 뜨고 이동이 안 됩니다.</b>
        ///
        /// 🚨 <b>그런데 이 프로젝트에는 <c>Teleport</c> 이름표가 아예 없었습니다.</b>
        /// 목록에 <c>Default</c> 하나뿐이라 <b>고르고 싶어도 고를 수가 없습니다.</b>
        /// 그래서 <c>EnsureInteractionLayer</c> 가 <b>먼저 만들어둡니다.</b>
        ///
        /// 21차시가 가르친 그대로입니다 — <b>만들어야 고를 수 있고, 골라야 반응합니다.</b>
        /// </summary>
        static void MakeFloorTeleportable()
        {
            GameObject ground = GameObject.Find("Ground");
            if (ground == null)
            {
                throw new System.InvalidOperationException(
                    "28차시에서 놓은 바닥(Ground)이 없습니다. 28차시부터 다시 만들어주세요.");
            }

            int layer = BuildKit.EnsureInteractionLayer(BuildXrLayout.TeleportLayer);
            if (layer < 0)
            {
                throw new System.InvalidOperationException(
                    $"'{BuildXrLayout.TeleportLayer}' 상호작용 이름표를 못 만들었습니다.");
            }

            var area = BuildKit.Ensure<TeleportationArea>(ground);

            // 이름표 번호를 직접 씁니다. 이름으로 찾으면 방금 등록한 게
            // 아직 캐시에 안 올라와 «못 찾음» 이 될 수 있습니다.
            area.interactionLayers = 1 << layer;

            EditorUtility.SetDirty(area);
        }

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            var grab = Object.FindFirstObjectByType<XRGrabInteractable>();
            var area = Object.FindFirstObjectByType<TeleportationArea>();

            int layer = BuildKit.EnsureInteractionLayer(BuildXrLayout.TeleportLayer);
            bool tagged = area != null && layer >= 0
                          && ((int)area.interactionLayers & (1 << layer)) != 0;
            bool onlyTeleport = area != null && (int)area.interactionLayers == (1 << layer);

            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 31차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다. **갈래 C(28 ~ 31차시)가 여기서 끝납니다.**\n" +
                    $"잡히는 물건: {(grab != null ? "있음" : "❌ 없음")}" +
                    $" · 던지기: {(grab != null && grab.throwOnDetach ? "켜짐" : "❌ 꺼짐")}\n" +
                    $"순간이동 바닥: {(area != null ? "있음" : "❌ 없음")}" +
                    $" · 이름표 Teleport({layer}): {(tagged ? "맞음" : "❌ 안 맞음")}" +
                    $"{(tagged && !onlyTeleport ? " (Default 가 아직 켜져 있습니다)" : "")}\n" +
                    $"큐브: 사람 앞 {BuildXrLayout.GrabCubeOffset.z:0.0}m · 높이 " +
                    $"{BuildXrLayout.GrabCubeOffset.y:0.0} · 크기 {BuildXrLayout.GrabCubeScale.x:0.0}\n" +
                    "▶ → Game 창 클릭 → `]` 로 오른손 → 큐브에 대고 `G` 로 잡기 → 휘두르며 놓기.\n" +
                    "`I` 를 누른 채 바닥을 겨누면 선이 휘어야 합니다. **빨간 선이면 이름표가 안 맞은 것**입니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 31차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
