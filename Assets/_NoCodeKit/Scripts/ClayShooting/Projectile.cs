using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 총알에 붙이는 교육용 부품. 24차시에서 사용합니다.
///
/// 날아가는 것은 Rigidbody 가 맡고(총이 속도를 넣어줍니다), 이 부품은
/// <b>무엇에 맞았는지 판단하고 스스로 사라지는 일</b>만 합니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Projectile")]
[DisallowMultipleComponent]
public class Projectile : MonoBehaviour
{
    [Header("총알 설정")]
    [Tooltip("몇 초 뒤에 사라질지 정합니다. 빗나간 총알이 쌓이지 않게 해줍니다.")]
    public float lifeTime = 3f;

    [Tooltip("이 이름표를 단 것에 맞았을 때만 On Hit 이 실행됩니다. 비워두면 무엇에든 반응합니다.")]
    public string targetTag = "Target";

    [Header("이벤트")]
    [Tooltip("표적에 맞는 순간 같이 실행할 동작을 연결합니다.")]
    public UnityEvent onHit;

    [Tooltip("아무것도 못 맞히고 사라질 때 같이 실행할 동작을 연결합니다.")]
    public UnityEvent onMiss;

    float timer;
    bool done;

    void Reset()
    {
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"[{name}] 부딪히는 부품(Collider)이 없습니다. 붙여주세요.", this);
        }
    }

    void Update()
    {
        if (done) return;

        timer += Time.deltaTime;
        if (timer < lifeTime) return;

        done = true;
        onMiss?.Invoke();
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        Touch(other.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Touch(other.gameObject);
    }

    void Touch(GameObject other)
    {
        if (done) return;

        // 표적이 아닌 것(바닥·벽)에 맞으면 조용히 사라집니다.
        if (!Matches(other))
        {
            done = true;
            Destroy(gameObject);
            return;
        }

        done = true;
        onHit?.Invoke();
        Destroy(gameObject);
    }

    bool Matches(GameObject other)
    {
        if (string.IsNullOrWhiteSpace(targetTag)) return true;
        return other.CompareTag(targetTag);
    }
}
