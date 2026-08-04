using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 부딪힘을 감지해서 알려주는 교육용 부품. 21차시에서 사용합니다.
///
/// 부딪히는 부품(Collider)이 붙어 있어야 동작합니다.
/// Is Trigger 가 켜져 있든 꺼져 있든 양쪽 모두 감지합니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Collision Reporter")]
[DisallowMultipleComponent]
public class CollisionReporter : MonoBehaviour
{
    [Header("무엇에 반응할지")]
    [Tooltip("여기에 적은 이름표(Tag)를 단 것에만 반응합니다. 비워두면 무엇에든 반응합니다.")]
    public string targetTag = "";

    [Tooltip("체크해두면 부딪힐 때마다 Console 에 한 줄씩 남깁니다.")]
    public bool reportToConsole = true;

    [Header("이벤트")]
    [Tooltip("부딪히는 순간 같이 실행할 동작을 연결합니다.")]
    public UnityEvent onHit;

    [Tooltip("떨어지는 순간 같이 실행할 동작을 연결합니다.")]
    public UnityEvent onLeave;

    void Reset()
    {
        // 부딪히는 부품이 없으면 아무 일도 일어나지 않으므로 미리 알려줍니다.
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning(
                $"[{name}] 부딪히는 부품(Collider)이 없습니다. Add Component 로 먼저 붙여주세요.",
                this);
        }
    }

    // ── 밀어내는 충돌 ─────────────────────────────────────

    void OnCollisionEnter(Collision other) { Enter(other.gameObject); }
    void OnCollisionExit(Collision other) { Leave(other.gameObject); }

    // ── 통과하는 충돌 (Is Trigger) ────────────────────────

    void OnTriggerEnter(Collider other) { Enter(other.gameObject); }
    void OnTriggerExit(Collider other) { Leave(other.gameObject); }

    // ── 내부 ─────────────────────────────────────────────

    void Enter(GameObject other)
    {
        if (!Matches(other)) return;

        if (reportToConsole)
        {
            Debug.Log($"[{name}] 부딪혔습니다 → {other.name}", this);
        }
        onHit?.Invoke();
    }

    void Leave(GameObject other)
    {
        if (!Matches(other)) return;

        if (reportToConsole)
        {
            Debug.Log($"[{name}] 떨어졌습니다 → {other.name}", this);
        }
        onLeave?.Invoke();
    }

    bool Matches(GameObject other)
    {
        // 이름표를 비워두면 무엇에든 반응합니다.
        if (string.IsNullOrWhiteSpace(targetTag)) return true;
        return other.CompareTag(targetTag);
    }
}
