using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 발판을 밟으면 게임을 시작시키는 교육용 부품. 23차시에서 사용합니다.
///
/// <b>Is Trigger 가 켜진 Collider</b> 가 붙은 물건에 붙입니다.
/// 21차시 실습 ④(통과하면서 감지하기)의 회수 지점입니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Game Starter")]
[DisallowMultipleComponent]
public class GameStarter : MonoBehaviour
{
    [Header("시작 조건")]
    [Tooltip("이 이름표를 단 것이 들어오면 시작합니다. 비워두면 무엇이든 들어오면 시작합니다.")]
    public string triggerTag = "Player";

    [Tooltip("체크하면 한 번만 시작합니다. 껐다 켜려면 Reset Game 을 쓰세요.")]
    public bool startOnce = true;

    [Header("이벤트")]
    [Tooltip("게임이 시작될 때 같이 실행할 동작을 연결합니다.")]
    public UnityEvent onGameStart;

    [Tooltip("게임이 끝날 때 같이 실행할 동작을 연결합니다.")]
    public UnityEvent onGameEnd;

    bool started;

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning(
                $"[{name}] 부딪히는 부품(Collider)이 없습니다. 붙이고 Is Trigger 를 켜주세요.",
                this);
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning(
                $"[{name}] Collider 의 Is Trigger 가 꺼져 있습니다. 켜야 밟고 지나갈 수 있습니다.",
                this);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!Matches(other.gameObject)) return;
        if (startOnce && started) return;

        StartGame();
    }

    /// <summary>게임을 시작합니다. 버튼에 연결해서 쓸 수도 있습니다.</summary>
    public void StartGame()
    {
        started = true;
        onGameStart?.Invoke();
    }

    /// <summary>게임을 끝냅니다. Round Timer 의 On Finished 에 연결해서 씁니다.</summary>
    public void EndGame()
    {
        onGameEnd?.Invoke();
    }

    /// <summary>다시 시작할 수 있는 상태로 되돌립니다.</summary>
    public void ResetGame()
    {
        started = false;
    }

    bool Matches(GameObject other)
    {
        if (string.IsNullOrWhiteSpace(triggerTag)) return true;
        return other.CompareTag(triggerTag);
    }
}
