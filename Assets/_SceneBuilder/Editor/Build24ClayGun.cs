using System.Collections.Generic;
using NoCodeKit.EditorTools;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 24차시 — 총을 쏘게.
    ///
    /// 교안 `24_clay_gun_nocode.md` 의 실습 ① ~ ③ 을 만듭니다.
    /// <list type="bullet">
    /// <item>실습 ① 총 조립 (카메라의 자식 · `MuzzlePoint`)</item>
    /// <item>실습 ② 총알 틀 (`Use Gravity` 끄기 · 이름표 `Bullet`)</item>
    /// <item>실습 ③ `SimpleGun` + `MouseShooter` + 십자선</item>
    /// </list>
    ///
    /// 실습 ④(총구 불꽃)는 <b>만들지 않습니다.</b> 실제 효과 리소스가 들어오면 붙입니다.
    /// </summary>
    public static class Build24ClayGun
    {
        const string FromScene = "23_완성";
        const string StartScene = "24_시작";
        const string CompleteScene = "24_완성";

        // 총구 자리. 총알도 불꽃도 여기서 나옵니다.
        static readonly Vector3 MuzzleLocal = new Vector3(0f, 0.03f, 0.5f);

        [MenuItem("Tools/교안 씬 빌더/24차시 — 총을 쏘게", false, 24)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "24차시 씬 만들기",
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

            (GameObject gun, Transform muzzle) = BuildGun();
            GameObject bulletPrefab = BuildBulletPrefab();
            AttachShooting(gun, muzzle, bulletPrefab);
            BuildCrosshair();

            // 실습 ④(총구 불꽃)는 만들지 않습니다. §총구 불꽃 참고.

            BuildKit.SaveComplete(CompleteScene);
            Report();
        }

        // ══════════════════════════════════════════════════════
        //  실습 ① — 총 조립하기
        // ══════════════════════════════════════════════════════

        static (GameObject, Transform) BuildGun()
        {
            Camera cam = Object.FindFirstObjectByType<Camera>();
            if (cam == null)
            {
                throw new System.InvalidOperationException(
                    "씬에 카메라가 없습니다. 22차시 씬부터 다시 만들어주세요.");
            }

            Material metal = BuildKit.Mat("Gun", new Color(0.16f, 0.17f, 0.19f));   // 어두운 색

            // 눈(카메라)의 자식으로 답니다. 몸 → 눈 → 총, 세 겹입니다 (14차시).
            GameObject gun = BuildKit.Empty("Gun", cam.transform)
                                     .At(new Vector3(0.3f, -0.25f, 0.5f));

            // 총은 눈에만 보이면 됩니다. 부딪히는 부품을 남겨두면
            // 총알이 나오자마자 총에 맞고 사라집니다.
            BuildKit.Shape("GunBody", PrimitiveType.Cube, gun.transform)
                    .At(Vector3.zero, scale: new Vector3(0.08f, 0.1f, 0.35f))
                    .Paint(metal)
                    .NoCollider();

            BuildKit.Shape("GunBarrel", PrimitiveType.Cylinder, gun.transform)
                    .At(new Vector3(0f, 0.03f, 0.3f),
                        rot: new Vector3(90f, 0f, 0f),
                        scale: new Vector3(0.03f, 0.15f, 0.03f))
                    .Paint(metal)
                    .NoCollider();

            // 총알이 나오는 자리이자 날아가는 방향입니다. 회전은 0 그대로 둡니다.
            GameObject muzzle = BuildKit.Empty("MuzzlePoint", gun.transform).At(MuzzleLocal);

            return (gun, muzzle.transform);
        }

        // ══════════════════════════════════════════════════════
        //  실습 ② — 총알 틀 만들기
        // ══════════════════════════════════════════════════════

        static GameObject BuildBulletPrefab()
        {
            // 25차시의 명중 판정이 이 이름표를 봅니다. 달기 전에 등록해야 합니다.
            BuildKit.EnsureTag("Bullet");

            Material bright = BuildKit.Mat("Bullet", new Color(0.98f, 0.85f, 0.20f));   // 노란 구슬

            GameObject bullet = BuildKit.Shape("Bullet", PrimitiveType.Sphere)
                                        .At(new Vector3(0f, 1f, 0f), scale: Vector3.one * 0.08f)
                                        .Paint(bright);
            bullet.tag = "Bullet";

            var body = bullet.AddComponent<Rigidbody>();
            body.useGravity = false;   // 안 끄면 총알이 발밑으로 뚝 떨어집니다

            // 🔴 총알이 표적을 뚫고 지나가지 않게 하는 설정입니다.
            //
            // Continuous 로는 부족합니다. 그건 "움직이지 않는 것"(벽·바닥)만 훑어보거든요.
            // 표적에는 Rigidbody 가 있어서, Continuous Dynamic 이라야 표적까지 훑습니다.
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            EditorUtility.SetDirty(body);

            // 총알이 작아서 지나칠 수 있으니 부딪히는 부품만 조금 키웁니다.
            var ball = bullet.GetComponent<SphereCollider>();
            if (ball != null)
            {
                ball.radius = BuildConfig.BulletColliderRadius;
                EditorUtility.SetDirty(ball);
            }

            var projectile = BuildKit.Add<Projectile>(bullet);
            projectile.lifeTime = BuildConfig.BulletLifeTime;
            projectile.targetTag = "Target";
            EditorUtility.SetDirty(projectile);

            return BuildKit.SaveAsPrefab(bullet, "Bullet");
        }

        // ══════════════════════════════════════════════════════
        //  실습 ③ — 쏘아보기
        // ══════════════════════════════════════════════════════

        static void AttachShooting(GameObject gun, Transform muzzle, GameObject bulletPrefab)
        {
            var simpleGun = BuildKit.Add<SimpleGun>(gun);
            simpleGun.bulletPrefab = bulletPrefab;
            simpleGun.muzzlePoint = muzzle;
            simpleGun.bulletSpeed = BuildConfig.BulletSpeed;
            simpleGun.fireRate = BuildConfig.FireRate;
            simpleGun.canFire = true;
            EditorUtility.SetDirty(simpleGun);

            // 총은 총구 방향으로 내보내기만 합니다. 겨누고 쏘라고 알려주는 건 이쪽입니다.
            var shooter = BuildKit.Add<MouseShooter>(gun);
            shooter.aimAtCrosshair = true;
            shooter.zeroDistance = BuildConfig.ZeroDistance;
            shooter.fireButton = MouseShooter.Button.Left;
            shooter.holdToFire = false;
            EditorUtility.SetDirty(shooter);
        }

        static void BuildCrosshair()
        {
            Canvas canvas = BuildKitUi.Root();

            BuildKitUi.Box("Crosshair", canvas.transform,
                           Vector2.zero, new Vector2(8f, 8f), Color.white)
                      .Anchor(new Vector2(0.5f, 0.5f));   // 화면 한가운데
        }

        // ══════════════════════════════════════════════════════
        //  총구 불꽃 — 지금은 만들지 않습니다
        // ══════════════════════════════════════════════════════
        //
        // 교안 실습 ④ 는 기본 도형 파티클로 불꽃을 만드는데, 보기에 좋지 않습니다.
        // 실제 효과 리소스가 들어오면 그때 다시 붙입니다.
        //
        // 지금 비워둬도 아무 문제 없습니다.
        //   · SimpleGun 은 On Fired 에 아무것도 없어도 그냥 쏩니다
        //   · 점검기는 "연결이 0개인 이벤트 칸" 을 지적하지 않습니다
        //   · 27차시가 같은 On Fired 에 총소리를 얹습니다
        //
        // 붙일 때는 Gun 의 자식으로 두고, 자리는 MuzzleLocal 을 쓰면 됩니다.

        // ══════════════════════════════════════════════════════
        //  만든 뒤 스스로 확인
        // ══════════════════════════════════════════════════════

        static void Report()
        {
            List<NoCodeIssue> found = NoCodeCheckRules.CheckAll();

            if (found.Count == 0)
            {
                Debug.Log(
                    $"✅ 24차시 씬을 만들었습니다 — {BuildKit.ScenePath(CompleteScene)}\n" +
                    "점검기도 통과했습니다.\n" +
                    "▶ 를 누르고 ① 발판을 밟아 표적을 띄운 뒤 ② 십자선에 맞춰 마우스 왼쪽 버튼으로 쏴보세요.\n" +
                    "\n" +
                    "※ 총구 불꽃(실습 ④)은 넣지 않았습니다. 실제 효과 리소스가 들어오면 붙입니다.\n" +
                    "  On Fired 가 비어 있어도 총은 그냥 쏩니다.");
                return;
            }

            Debug.LogWarning($"⚠️ 24차시 씬은 만들었지만 점검기가 {found.Count}군데를 짚었습니다.");
            foreach (NoCodeIssue issue in found)
            {
                Debug.LogWarning(issue.Message, issue.Context);
            }
        }
    }
}
