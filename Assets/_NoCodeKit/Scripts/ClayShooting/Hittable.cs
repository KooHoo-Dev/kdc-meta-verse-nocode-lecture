using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 맞으면 반응하는 교육용 부품. 25차시에서 사용합니다. <b>표적 틀(Prefab)에 붙입니다.</b>
///
/// 맞으면 두 가지가 일어납니다.
/// 1. <c>On Hit</c> 에 연결해둔 동작이 실행됩니다 (터지는 효과 · 소리)
/// 2. 씬의 <see cref="HitScorer"/> 를 <b>알아서 찾아가</b> 점수를 올립니다
///
/// 2번을 부품 안에 숨긴 이유는, 틀에서 씬 안의 점수판을 끌어다 놓을 수 없기 때문입니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Hittable")]
[DisallowMultipleComponent]
public class Hittable : MonoBehaviour
{
    [Header("무엇에 맞았을 때")]
    [Tooltip("이 이름표를 단 것에 맞았을 때만 반응합니다. 비워두면 무엇에든 반응합니다.")]
    public string hitterTag = "Bullet";

    [Tooltip("체크하면 맞은 뒤 사라집니다.")]
    public bool destroyOnHit = true;

    [Tooltip("체크를 끄면 점수가 오르지 않습니다. 점수 없는 장식용 표적에 쓰세요.")]
    public bool addScore = true;

    [Header("이벤트")]
    [Tooltip("맞는 순간 같이 실행할 동작을 연결합니다.")]
    public UnityEvent onHit;

    bool hit;

    void Reset()
    {
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"[{name}] 부딪히는 부품(Collider)이 없습니다. 붙여주세요.", this);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        TryHit(other.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        TryHit(other.gameObject);
    }

    void TryHit(GameObject other)
    {
        if (hit) return;                 // 한 번만 반응합니다
        if (!Matches(other)) return;

        hit = true;

        // 연결해둔 동작을 먼저 실행합니다. (터지는 효과가 사라지기 전에 나와야 합니다)
        onHit?.Invoke();

        if (addScore)
        {
            HitScorer scorer = HitScorer.Find();
            if (scorer != null)
            {
                scorer.AddScore();
            }
            else
            {
                Debug.LogWarning(
                    $"[{name}] 씬에 Hit Scorer 가 없어서 점수가 오르지 않습니다. " +
                    "빈 그릇을 하나 만들어 Hit Scorer 를 붙여주세요.",
                    this);
            }
        }

        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    bool Matches(GameObject other)
    {
        if (string.IsNullOrWhiteSpace(hitterTag)) return true;
        return other.CompareTag(hitterTag);
    }
}
