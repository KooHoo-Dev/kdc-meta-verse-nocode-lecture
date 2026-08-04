using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// 마우스를 누르면 총알을 쏘는 교육용 부품. 24차시에서 사용합니다.
///
/// 총알은 <b>실제로 날아가는 물건</b>입니다. 눈에 안 보이는 즉시 판정 방식은 쓰지 않습니다.
/// 그래야 21차시에 배운 Collider · Rigidbody 가 그대로 이어집니다.
///
/// 조준은 <b>화면 한가운데(십자선)</b> 기준입니다. 총이 화면 구석에 있어도 십자선으로 나갑니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Simple Gun")]
[DisallowMultipleComponent]
public class SimpleGun : MonoBehaviour
{
    [Header("무엇을, 어디서")]
    [Tooltip("쏘아 보낼 총알 틀(Prefab)을 끌어다 놓습니다.")]
    public GameObject bulletPrefab;

    [Tooltip("총알이 나올 자리입니다. 총구 끝에 빈 그릇을 하나 두고 끌어다 놓으세요.")]
    public Transform muzzlePoint;

    [Header("총 설정")]
    [Tooltip("총알이 날아가는 속도입니다. 너무 빠르면 눈에 안 보입니다.")]
    public float bulletSpeed = 40f;

    [Tooltip("한 발 쏘고 다음 발까지 기다릴 시간(초)입니다. 연타를 막아줍니다.")]
    public float fireRate = 0.3f;

    [Tooltip("체크를 끄면 쏘지 못합니다. 게임이 끝났을 때 꺼두는 용도입니다.")]
    public bool canFire = true;

    [Header("이벤트")]
    [Tooltip("총을 쏠 때마다 같이 실행할 동작을 연결합니다. (총구 불꽃 · 총소리)")]
    public UnityEvent onFired;

    float cooldown;

    void Update()
    {
        if (cooldown > 0f) cooldown -= Time.deltaTime;

        if (!canFire) return;
        if (!FirePressed()) return;

        Fire();
    }

    /// <summary>한 발 쏩니다. 버튼에 연결해서 쓸 수도 있습니다.</summary>
    public void Fire()
    {
        if (!canFire) return;
        if (cooldown > 0f) return;

        if (bulletPrefab == null)
        {
            Debug.LogWarning($"[{name}] Bullet Prefab 이 비어 있습니다. 총알 틀을 끌어다 놓아주세요.", this);
            return;
        }

        Transform point = muzzlePoint != null ? muzzlePoint : transform;
        Vector3 dir = GetAimDirection(point.position);

        GameObject bullet = Instantiate(bulletPrefab, point.position, Quaternion.LookRotation(dir));

        var body = bullet.GetComponent<Rigidbody>();
        if (body != null)
        {
            body.linearVelocity = dir * bulletSpeed;
        }
        else
        {
            Debug.LogWarning(
                $"[{name}] 총알 틀에 Rigidbody 가 없어서 날아가지 않습니다. 틀에 붙여주세요.",
                this);
        }

        cooldown = fireRate;
        onFired?.Invoke();
    }

    /// <summary>쏠 수 있게 합니다.</summary>
    public void EnableFire()
    {
        canFire = true;
    }

    /// <summary>못 쏘게 합니다. 게임 종료에 연결해서 씁니다.</summary>
    public void DisableFire()
    {
        canFire = false;
    }

    // ── 내부 ─────────────────────────────────────────────

    // 총이 화면 구석에 있어도 십자선(화면 한가운데)으로 나가게 방향을 맞춥니다.
    Vector3 GetAimDirection(Vector3 from)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            return (muzzlePoint != null ? muzzlePoint : transform).forward;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Vector3 aim = Physics.Raycast(ray, out RaycastHit hit, 300f)
            ? hit.point
            : ray.GetPoint(300f);

        Vector3 dir = aim - from;
        return dir.sqrMagnitude < 0.0001f ? cam.transform.forward : dir.normalized;
    }

    static bool FirePressed()
    {
#if ENABLE_INPUT_SYSTEM
        var m = Mouse.current;
        return m != null && m.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }
}
