using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 표적을 일정 간격으로 하늘에 쏘아 올리는 교육용 부품. 23차시에서 사용합니다.
///
/// <c>Target Prefab</c> 에 <b>틀(Prefab)</b> 을 끌어다 놓으면 그걸 찍어냅니다.
/// 15차시에 배운 붕어빵 틀이 여기서 실제로 쓰입니다.
///
/// Interval · Launch Force · Spread Angle 이 <b>난이도를 결정하는 값</b>입니다. (26차시)
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Target Launcher")]
[DisallowMultipleComponent]
public class TargetLauncher : MonoBehaviour
{
    [Header("무엇을, 어디서")]
    [Tooltip("쏘아 올릴 표적 틀(Prefab)을 끌어다 놓습니다.")]
    public GameObject targetPrefab;

    [Tooltip("표적이 나올 자리입니다. 이 물건이 보는 방향으로 날아갑니다.")]
    public Transform launchPoint;

    [Header("난이도 값")]
    [Tooltip("몇 초마다 한 번씩 쏘아 올릴지 정합니다. 작을수록 어려워집니다.")]
    public float interval = 1.5f;

    [Tooltip("얼마나 세게 쏘아 올릴지 정합니다. 클수록 멀리·빠르게 날아갑니다.")]
    public float launchForce = 12f;

    [Tooltip("방향이 얼마나 흩어질지 정합니다(각도). 클수록 예측이 어려워집니다.")]
    public float spreadAngle = 25f;

    [Header("이벤트")]
    [Tooltip("표적을 하나 쏘아 올릴 때마다 같이 실행할 동작을 연결합니다.")]
    public UnityEvent onLaunched;

    bool launching;
    float timer;

    void Update()
    {
        if (!launching) return;

        timer += Time.deltaTime;
        if (timer < interval) return;

        timer = 0f;
        LaunchOne();
    }

    /// <summary>표적을 쏘아 올리기 시작합니다. 게임 시작에 연결해서 씁니다.</summary>
    public void StartLaunching()
    {
        if (targetPrefab == null)
        {
            Debug.LogWarning($"[{name}] Target Prefab 이 비어 있습니다. 표적 틀을 끌어다 놓아주세요.", this);
            return;
        }

        launching = true;
        timer = 0f;
    }

    /// <summary>쏘아 올리기를 멈춥니다. 게임 종료에 연결해서 씁니다.</summary>
    public void StopLaunching()
    {
        launching = false;
    }

    /// <summary>지금 한 개만 쏘아 올립니다. 버튼으로 시험해볼 때 편합니다.</summary>
    public void LaunchOne()
    {
        if (targetPrefab == null) return;

        Transform point = launchPoint != null ? launchPoint : transform;

        GameObject target = Instantiate(targetPrefab, point.position, point.rotation);

        // 방향을 조금씩 흩뜨립니다. 매번 똑같은 데로 날아가면 재미가 없습니다.
        Vector3 dir = Quaternion.Euler(
            Random.Range(-spreadAngle, spreadAngle),
            Random.Range(-spreadAngle, spreadAngle),
            0f) * point.forward;

        var body = target.GetComponent<Rigidbody>();
        if (body != null)
        {
            body.AddForce(dir.normalized * launchForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning(
                $"[{name}] 표적 틀에 Rigidbody 가 없어서 날아가지 않습니다. 틀에 붙여주세요.",
                this);
        }

        onLaunched?.Invoke();
    }
}
