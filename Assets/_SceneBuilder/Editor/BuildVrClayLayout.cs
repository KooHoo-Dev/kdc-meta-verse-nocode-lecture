using UnityEngine;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 갈래 D(32 · 33차시)의 <b>배치표</b>.
    ///
    /// 27차시 사격장 위에 얹는 것들이라, 그 씬의 좌표를 기준으로 잡았습니다.
    /// <list type="bullet">
    /// <item>사람이 서는 자리 — <c>Z -20</c> (<c>BuildConfig.PlayerStartZ</c>)</item>
    /// <item>발사기 — <c>Z +15</c></item>
    /// <item>바닥 — 가로세로 50, 울타리가 <c>±25</c></item>
    /// </list>
    /// </summary>
    public static class BuildVrClayLayout
    {
        // ══════════════════════════════════════════════════════
        //  32차시 — 사람 · 총 · 버튼
        // ══════════════════════════════════════════════════════

        /// <summary>VR 속의 나. 27차시 사람이 서던 그 자리입니다.</summary>
        public static readonly Vector3 RigPosition = new Vector3(0f, 0f, BuildConfig.PlayerStartZ);

        /// <summary>총 받침대. 사람 오른쪽 앞, 손이 닿는 자리.</summary>
        public static readonly Vector3 GunStand = new Vector3(0.5f, 0.5f, -19f);
        public static readonly Vector3 GunStandScale = new Vector3(0.6f, 1f, 0.6f);

        /// <summary>받침대 위에 올려둔 총.</summary>
        public static readonly Vector3 GunOnStand = new Vector3(0.5f, 1.1f, -19f);

        /// <summary>
        /// 총에 다시 붙이는 <b>잡힐 몸</b>.
        ///
        /// 🚨 <b><c>Z</c> 가 <c>0.4</c> 를 넘으면 안 됩니다.</b>
        /// 총알은 <c>MuzzlePoint</c>(<c>Z 0.5</c>)에서 나옵니다.
        /// 몸이 거기까지 뻗으면 <b>총알이 나오자마자 총에 맞고 사라집니다.</b>
        /// 24차시에 부딪히는 부품을 통째로 지웠던 이유가 바로 그것이었어요.
        /// </summary>
        public static readonly Vector3 GunColliderSize = new Vector3(0.12f, 0.14f, 0.4f);

        // ══════════════════════════════════════════════════════
        //  32차시 — 조작 콘솔 (게임 시작 오브젝트)
        // ══════════════════════════════════════════════════════
        //
        //  사람이 Z -20 에 서서 +Z 쪽(발사기)을 봅니다.
        //  콘솔은 그 앞 1.2미터, 왼쪽에 세워 둡니다.
        //
        //     ┌─────────────┐  ← 판 (사람 쪽을 봄)
        //     │   [ 시작 ]  │
        //     │ [쉬움][보통][어려움] │
        //     └──────┬──────┘
        //            │  ← 기둥
        //  ══════════════════════════════════════════════════════

        /// <summary>
        /// 조작 콘솔이 서는 자리. <b>사람 왼쪽 앞 1미터</b>, 손이 닿는 거리입니다.
        ///
        /// 좌우 <c>0.7</c> · 앞뒤 <c>0.7</c> 이라 사람에게서 <b>정확히 45°</b> 입니다.
        /// 교안이 <c>Rotation Y</c> 에 <b><c>-45</c></b> 라는 <b>딱 떨어지는 숫자</b>를
        /// 적을 수 있는 이유가 이것입니다.
        /// </summary>
        public static readonly Vector3 Console = new Vector3(-0.7f, 0f, -19.3f);

        /// <summary>콘솔 기둥.</summary>
        public static readonly Vector3 ConsoleLeg = new Vector3(0f, 0.5f, 0f);
        public static readonly Vector3 ConsoleLegScale = new Vector3(0.12f, 1f, 0.12f);

        /// <summary>콘솔 판. 기둥 위에 얹힙니다.</summary>
        public static readonly Vector3 ConsolePanel = new Vector3(0f, 1.15f, 0f);
        public static readonly Vector3 ConsolePanelScale = new Vector3(1.15f, 0.72f, 0.08f);

        /// <summary>
        /// 버튼 크기. <b>판 앞으로 튀어나오게</b> 얇고 넓게 잡았습니다.
        ///
        /// 처음엔 <c>0.3</c> 짜리 정육면체였는데 <b>고글 흉내 부품으로 맞히기가 어려웠습니다.</b>
        /// 손을 마우스로 움직여 겨눠야 해서, <b>넓고 사람 쪽을 보는 면</b>이라야 잡힙니다.
        /// </summary>
        public static readonly Vector3 ButtonScale = new Vector3(0.44f, 0.20f, 0.12f);
        public static readonly Vector3 SmallButtonScale = new Vector3(0.32f, 0.18f, 0.12f);

        /// <summary>시작 버튼 — 판 위쪽 가운데. 제일 크게.</summary>
        public static readonly Vector3 StartButton = new Vector3(0f, 1.36f, -0.09f);

        /// <summary>난이도 버튼 셋 — 판 아래쪽에 나란히.</summary>
        public static readonly Vector3[] DifficultyButtons =
        {
            new Vector3(-0.36f, 1.02f, -0.09f),
            new Vector3(0f, 1.02f, -0.09f),
            new Vector3(0.36f, 1.02f, -0.09f),
        };

        /// <summary>버튼 앞면에 붙는 글자. 버튼보다 조금 작게.</summary>
        public static readonly Vector2 LabelSize = new Vector2(0.40f, 0.16f);
        public static readonly Vector2 SmallLabelSize = new Vector2(0.30f, 0.15f);

        // 🚨 글자가 뒤집히는 이유 — 두 번 틀리고 알아낸 것
        //
        //    세상 글자(TextMeshPro)와 세상 화면(World Space Canvas)은
        //    **«local +Z 가 보는 방향»** 입니다. **+Z 가 향하는 쪽이 «앞» 이 아닙니다.**
        //
        //    | 판의 local +Z 가 | 결과 |
        //    | 사람 쪽을 향함   | ❌ 좌우로 뒤집혀 보임 |
        //    | 사람 반대쪽      | ✅ 바로 읽힘 |
        //
        //    그래서 콘솔 · 게시판은 **+Z 를 사람 반대쪽(사람이 보는 방향)** 으로 세웁니다.
        //    자식(버튼 · 글자 · 화면)은 **부모 기준 −Z** 로 튀어나와야 사람 앞에 옵니다.
        //
        //    ① 처음엔 판을 안 돌리고 «글자만» 180° 돌렸다가 — 크기 보정과 겹쳐 뒤집힘
        //    ② 다음엔 판의 +Z 를 사람 쪽으로 돌렸다가 — 여전히 뒤집힘
        //    ③ 지금이 맞습니다.

        // ══════════════════════════════════════════════════════
        //  32차시 — 세상에 세운 점수판
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 점수 게시판이 서는 자리. <b>사람 오른쪽 앞</b>입니다.
        ///
        /// 처음엔 8미터 앞 공중(<c>0, 4, -12</c>)에 띄웠는데
        /// <b>표적이 날아오는 길과 겹치고 너무 멀어서</b> 읽기 어려웠습니다.
        /// 실제 사격장처럼 <b>옆에 세워두는 쪽</b>이 낫습니다.
        ///
        /// 좌우 <c>3</c> · 앞뒤 <c>3</c> — 콘솔과 마찬가지로 <b>정확히 45°</b> 입니다.
        /// 사람에게서 <b>4.2미터</b>, 가로 2미터 판이 한눈에 들어오는 거리예요.
        /// </summary>
        public static readonly Vector3 ScoreBoard = new Vector3(3f, 0f, -17f);

        /// <summary>게시판 기둥과 판.</summary>
        public static readonly Vector3 BoardLeg = new Vector3(0f, 0.7f, 0f);
        public static readonly Vector3 BoardLegScale = new Vector3(0.14f, 1.4f, 0.14f);
        public static readonly Vector3 BoardPanel = new Vector3(0f, 1.8f, 0f);
        public static readonly Vector3 BoardPanelScale = new Vector3(2f, 1f, 0.08f);

        /// <summary>글자가 올라갈 판(Canvas)의 자리와 크기.</summary>
        public static readonly Vector3 BoardCanvas = new Vector3(0f, 1.8f, -0.05f);
        public static readonly Vector2 BoardSize = new Vector2(800f, 400f);

        /// <summary>
        /// 판을 세상 크기로 줄이는 값.
        ///
        /// 화면에 붙어 있을 때 <c>800</c> 은 <b>점 800개</b>였습니다.
        /// 세상 안에서는 <b>800미터</b>예요. 그대로 두면 하늘을 다 덮습니다.
        /// <c>0.0023</c> 을 곱하면 <b>가로 1.84미터</b> — 게시판 판(2미터) 안에 딱 들어갑니다.
        /// </summary>
        public const float BoardScale = 0.0023f;

        /// <summary>31차시에 만든 순간이동 이름표. 32차시 바닥도 이걸 씁니다.</summary>
        public const string TeleportLayer = "Teleport";

        // ══════════════════════════════════════════════════════
        //  33차시 — 사격장 여러 개
        // ══════════════════════════════════════════════════════

        /// <summary>사대 바닥. 사람이 서는 자리를 표시하는 납작한 판입니다.</summary>
        public static readonly Vector3 RangeFloor = new Vector3(0f, 0.05f, BuildConfig.PlayerStartZ);
        public static readonly Vector3 RangeFloorScale = new Vector3(6f, 0.1f, 6f);

        /// <summary>
        /// 사격장 세 곳의 자리. 울타리(<c>±25</c>) 안에 나란히 들어갑니다.
        ///
        /// 첫 번째가 <c>0</c> 인 이유 — <b>원본이 그 자리에 이미 있습니다.</b>
        /// 27차시 발사기가 <c>Z 15</c>, 사람이 <c>Z -20</c> 인 그 사격장이에요.
        /// </summary>
        public static readonly float[] RangeX = { 0f, -18f, 18f };

        /// <summary>
        /// 사격장마다 다른 난이도. 26차시 실습 ①의 세 단계 그대로입니다.
        ///
        /// <c>DifficultyPreset</c> 은 발사기를 <b>하나만</b> 가리킵니다.
        /// 그래서 난이도 버튼은 1번 사격장만 바꿔요.
        /// 대신 <b>곳마다 값을 박아두면</b> 성격이 다른 사격장이 됩니다.
        /// </summary>
        public struct Level
        {
            public string Label;
            public float Interval;
            public float Force;
            public float Spread;
            public Color Floor;
        }

        public static readonly Level[] Levels =
        {
            new Level { Label = "보통",   Interval = 1.5f, Force = 12f, Spread = 25f,
                        Floor = new Color(0.85f, 0.80f, 0.35f) },
            new Level { Label = "쉬움",   Interval = 2.5f, Force = 8f,  Spread = 10f,
                        Floor = new Color(0.35f, 0.75f, 0.45f) },
            new Level { Label = "어려움", Interval = 0.8f, Force = 16f, Spread = 40f,
                        Floor = new Color(0.85f, 0.35f, 0.35f) },
        };
    }
}
