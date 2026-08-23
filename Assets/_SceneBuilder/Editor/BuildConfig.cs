using UnityEngine;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 씬에 넣을 값을 한곳에 모아둡니다.
    ///
    /// <b>U-7 에서 숫자가 정해지면 여기 한 줄만 고치고 다시 돌리면 됩니다.</b>
    /// 그러면 그 값을 쓰는 씬 전부에 반영됩니다. 씬을 하나씩 열어 고칠 필요가 없습니다.
    ///
    /// 🟨 표시는 <b>아직 실제로 돌려보고 정하지 않은 값</b>입니다. (`plans/08` §3)
    /// </summary>
    public static class BuildConfig
    {
        // ── 22차시 · 걷기와 시야 ──────────────────────────────
        /// <summary>✅ U-7 `1-5` 확정 (2026-08-23) — 실제로 돌려보고 자연스러움을 확인했습니다</summary>
        public const float Sensitivity = 0.15f;

        public const float MoveSpeed = 5f;
        public const float JumpForce = 5f;

        // ── 23차시 · 표적 발사 ────────────────────────────────
        /// <summary>
        /// ✅ U-7 `2-8` 확정 (2026-08-23) — 실제로 쏘아보고 정했습니다.
        /// <b>12 ~ 20 이면 조준할 만합니다.</b> 26차시 난이도 세 단계도 이 범위 안에서 잡습니다.
        /// </summary>
        public const float LaunchForce = 12f;

        public const float LaunchInterval = 1.5f;
        public const float SpreadAngle = 25f;
        public const float TargetLifeTime = 5f;

        /// <summary>
        /// ✅ 실제로 쏴보고 정했습니다 (2026-08-23).
        /// 교안 초안은 <c>0.4</c> 였는데 <b>맞히기가 너무 어려웠습니다.</b>
        /// 실제로 쏴보며 <c>0.4</c> → <c>0.5</c> → <c>1.0</c> 으로 올렸습니다.
        /// </summary>
        public static readonly Vector3 TargetScale = new Vector3(1f, 0.05f, 1f);

        // ── 24차시 · 총과 총알 ────────────────────────────────
        /// <summary>
        /// ✅ U-7 `3-5` 확정 (2026-08-23) — 실제로 쏴보고 정했습니다.
        /// 40 에서 60 으로 올렸습니다. 날아가는 게 여전히 보이면서 조준이 편해집니다.
        /// </summary>
        public const float BulletSpeed = 60f;

        public const float FireRate = 0.3f;
        public const float BulletLifeTime = 3f;

        /// <summary>
        /// 총알의 부딪히는 부품 반지름(틀 안 기준). 기본은 0.5 인데 조금 키웠습니다.
        /// 크기 0.08 을 곱하면 실제로는 약 6cm 입니다.
        /// </summary>
        public const float BulletColliderRadius = 0.8f;

        /// <summary>
        /// ✅ U-7 `3-4` 로 알게 된 값 (2026-08-23) — <b>몇 미터 앞에서 총알이 십자선과 만날지.</b>
        /// 사격장은 발판(z = -15)에서 발사기(z = 15)까지 30m 라, 표적은 25m 안팎에 뜹니다.
        /// </summary>
        public const float ZeroDistance = 25f;

        // ── 24차시 · 총구 불꽃 ────────────────────────────────
        // 지금은 만들지 않습니다. 기본 도형 파티클이 보기에 좋지 않아서,
        // 실제 효과 리소스가 들어오면 그때 값을 다시 잡습니다.

        // ── 25차시 · 점수 ────────────────────────────────────
        public const int PointsPerHit = 10;

        /// <summary>🟨 U-7 `4-7` — 터지는 효과가 접시 자리에 뜨도록</summary>
        public const float SpawnOffset = 0f;

        // ── 26차시 · 제한 시간 ────────────────────────────────
        public const float RoundDuration = 60f;

        // ── 사격장 크기 ──────────────────────────────────────
        public const float GroundScale = 5f;      // Plane 은 10칸이라 5면 50칸
        public const float FenceHalf = 25f;       // 사격장 반지름
        public const float FenceHeight = 3f;
        public const float PlayerStartZ = -20f;   // 뒤쪽에서 시작
        public const float EyeHeight = 0.6f;      // 캡슐의 머리 높이
    }
}
