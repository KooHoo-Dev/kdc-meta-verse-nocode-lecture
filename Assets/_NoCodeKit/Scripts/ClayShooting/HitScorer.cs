using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 점수를 세는 교육용 부품. 25차시에서 사용합니다.
///
/// <b>씬에 하나만</b> 둡니다. (예: GameManager 라는 빈 그릇)
///
/// 왜 씬에 하나냐면 — 표적은 <b>틀(Prefab)</b> 이라서 씬 안의 점수판을 끌어다 놓을 수 없기 때문입니다.
/// 그래서 표적 쪽(<see cref="Hittable"/>)이 <b>이 부품을 알아서 찾아옵니다.</b>
/// 수강생이 할 일은 "씬에 하나 놓고 점수판을 연결" 이것뿐입니다.
///
/// <c>On Score Changed</c> 는 <b>현재 점수를 숫자로 넘겨줍니다.</b>
/// 18차시에 배운 <c>Dynamic float</c> 연결로 <c>ValueDisplay.SetValue</c> 에 이어집니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Hit Scorer")]
[DisallowMultipleComponent]
public class HitScorer : MonoBehaviour
{
    [Header("점수 설정")]
    [Tooltip("한 번 맞힐 때마다 오를 점수입니다.")]
    public int pointsPerHit = 10;

    [Header("이벤트")]
    [Tooltip("점수가 바뀔 때마다 같이 실행할 동작을 연결합니다. 현재 점수를 넘겨줍니다.")]
    public UnityEvent<float> onScoreChanged;

    /// <summary>지금까지의 점수입니다.</summary>
    public int Score { get; private set; }

    static HitScorer cached;

    void OnEnable()
    {
        if (cached == null) cached = this;
    }

    void OnDisable()
    {
        if (cached == this) cached = null;
    }

    void Start()
    {
        // 시작할 때 점수판에 0 을 한 번 보내줍니다.
        onScoreChanged?.Invoke(Score);
    }

    /// <summary>Points Per Hit 만큼 점수를 올립니다.</summary>
    public void AddScore()
    {
        Score += pointsPerHit;
        onScoreChanged?.Invoke(Score);
    }

    /// <summary>점수를 0 으로 되돌립니다.</summary>
    public void ResetScore()
    {
        Score = 0;
        onScoreChanged?.Invoke(Score);
    }

    /// <summary>
    /// 씬에 있는 Hit Scorer 를 찾아 돌려줍니다.
    /// 표적 틀에서 씬 오브젝트를 끌어다 놓을 수 없기 때문에 필요한 통로입니다.
    /// </summary>
    public static HitScorer Find()
    {
        if (cached == null)
        {
            cached = FindFirstObjectByType<HitScorer>();
        }
        return cached;
    }
}
