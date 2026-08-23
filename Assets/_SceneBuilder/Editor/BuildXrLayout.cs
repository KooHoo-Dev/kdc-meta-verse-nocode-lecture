using UnityEngine;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 갈래 C(28 ~ 31차시)의 <b>배치표</b>.
    ///
    /// 갈래 A와 달리 <b>쌓이는 양이 적습니다.</b> 28차시가 놓는 것이 거의 전부고,
    /// 29 ~ 31차시는 <b>이미 있는 것을 맞추거나 부품을 하나씩 붙입니다.</b>
    /// 그래서 배치표도 짧습니다.
    /// </summary>
    public static class BuildXrLayout
    {
        // ══════════════════════════════════════════════════════
        //  XRI 예제 꾸러미의 틀 이름
        // ══════════════════════════════════════════════════════

        /// <summary>28차시 실습 ③ 2단계 — VR 속의 «나».</summary>
        public const string RigPrefab = "XR Origin (XR Rig)";

        /// <summary>28차시 실습 ③ 3단계 — 고글 흉내 내는 부품.</summary>
        public const string SimulatorPrefab = "XR Interaction Simulator";

        // ══════════════════════════════════════════════════════
        //  28차시 — 확인할 공간
        // ══════════════════════════════════════════════════════

        /// <summary>바닥. 교안 실습 ③ 4단계 1번 — *"바닥이 없으면 허공이라 확인이 어렵습니다"*.</summary>
        public static readonly Vector3 GroundScale = new Vector3(2f, 1f, 2f);

        /// <summary>
        /// 기준이 될 기둥들.
        ///
        /// <b>왜 두나</b> — 교안이 바닥을 놓는 이유와 <b>같은 이유</b>입니다.
        /// 회색 바닥만 있으면 <b>둘러봐도 걸어 다녀도 움직이는지 알 수가 없습니다.</b>
        /// 29차시 실습 ①이 *"걸어 다녀지나요?"* 를 묻는데, 볼 게 있어야 답할 수 있습니다.
        ///
        /// 30차시에서 <b>하늘이 바뀌는 것</b>도 기둥에 빛이 닿는 걸로 보입니다.
        ///
        /// <b>📝 기둥을 통과하는 것은 고장이 아닙니다.</b>
        /// 고글 흉내 부품의 <c>W A S D</c> 는 <b>기기 자체를 옮깁니다.</b>
        /// 게임의 걷기 부품(과 그 몸통 충돌)을 <b>거치지 않아서</b> 벽이 있어도 지나갑니다.
        /// 29차시 교안 실습 ③ 2단계가 그 이유를 따로 설명해뒀습니다 —
        /// *"고글을 손으로 들고 옮기는 것처럼 동작하는 거예요"*.
        ///
        /// 그래서 기둥은 <b>막는 벽이 아니라 «움직이는 게 보이게» 하는 이정표</b>입니다.
        /// </summary>
        public static readonly Vector3[] Pillars =
        {
            new Vector3(-4f, 1f, 5f),
            new Vector3(4f, 1f, 5f),
            new Vector3(-6f, 1f, -2f),
            new Vector3(6f, 1f, -2f),
            new Vector3(0f, 1f, 8f),
        };

        public static readonly Vector3 PillarScale = new Vector3(0.6f, 2f, 0.6f);

        // ══════════════════════════════════════════════════════
        //  29차시 — 눈높이
        // ══════════════════════════════════════════════════════

        /// <summary>교안 실습 ② 5번 — *"`1.6` → 보통 어른 눈높이"*.</summary>
        public const float EyeHeight = 1.6f;

        // ══════════════════════════════════════════════════════
        //  30차시 — 하늘과 화면
        // ══════════════════════════════════════════════════════

        /// <summary>«다음 배경» 버튼.</summary>
        public static readonly Vector2 NextSkyButton = new Vector2(0f, 120f);
        public static readonly Vector2 NextSkyButtonSize = new Vector2(240f, 70f);

        /// <summary>지금 배경 이름을 보여주는 글자.</summary>
        public static readonly Vector2 SkyLabel = new Vector2(0f, 210f);
        public static readonly Vector2 SkyLabelSize = new Vector2(400f, 60f);

        // ══════════════════════════════════════════════════════
        //  31차시 — 잡을 물건
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 잡히는 큐브가 설 자리 — <b>리그(사람)를 기준으로 한 상대 위치</b>입니다.
        ///
        /// <b>월드 좌표로 두면 안 됩니다.</b> 리그가 <c>Z -2</c> 에 서 있어서,
        /// 월드 <c>Z 1</c> 에 두면 <b>사람 앞 3m</b> 가 됩니다. 손이 안 닿습니다.
        /// 교안은 *"손이 닿을 만한 곳 (눈앞 1m 정도)"* 이라고 합니다.
        ///
        /// 눈높이가 <c>1.6</c> 이라 <c>Y</c> 를 그보다 조금 낮게 둡니다.
        /// 손이 자연스럽게 내려오는 높이예요.
        /// </summary>
        public static readonly Vector3 GrabCubeOffset = new Vector3(0f, 1.2f, 0.9f);

        /// <summary>
        /// 큐브 크기. 교안은 <c>0.2</c> 를 예로 들지만 *"정도"* 라고 했고,
        /// <b>시뮬레이터에서 마우스로 손을 갖다 대기엔 너무 작습니다.</b> 두 배로 키웠습니다.
        /// </summary>
        public static readonly Vector3 GrabCubeScale = new Vector3(0.4f, 0.4f, 0.4f);

        /// <summary>31차시 순간이동 바닥이 쓸 이름표. <b>프로젝트에 없어서 빌더가 만듭니다.</b></summary>
        public const string TeleportLayer = "Teleport";

        // ══════════════════════════════════════════════════════
        //  카메라 — 작업대에서 볼 각도
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// <b>VR 씬에서는 <c>Main Camera</c> 가 없습니다.</b>
        /// 28차시 실습 ③ 1단계가 지우고, 그 자리를 <c>XR Origin</c> 안의 카메라가 대신합니다.
        ///
        /// 그래서 <c>BuildKit.AimCamera</c> 는 <b>여기서 쓸 수 없습니다</b>
        /// (그건 <c>Camera.main</c> 을 옮기는 도구입니다).
        /// 완성 씬의 그림은 <b>Scene 창</b>에서 보게 되므로, 대신 <b>리그를 놓는 자리</b>만 정합니다.
        /// </summary>
        public static readonly Vector3 RigPosition = new Vector3(0f, 0f, -2f);
    }
}
