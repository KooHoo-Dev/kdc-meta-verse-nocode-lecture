using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 제한 시간을 재는 교육용 부품. 26차시에서 사용합니다.
///
/// <c>On Tick</c> 은 <b>남은 시간을 숫자로 넘겨줍니다.</b>
/// 18차시에 배운 <c>Dynamic float</c> 연결로 <c>ValueDisplay.SetValue</c> 에 이어집니다.
///
/// 1초 단위로만 알려주므로, 화면 글자가 매 순간 요동치지 않습니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Round Timer")]
[DisallowMultipleComponent]
public class RoundTimer : MonoBehaviour
{
    [Header("시간 설정")]
    [Tooltip("한 판이 몇 초인지 정합니다. 난이도를 정하는 값 중 하나입니다.")]
    public float duration = 60f;

    [Tooltip("체크하면 ▶ 를 누르자마자 시작합니다. 보통은 꺼두고 게임 시작에 연결합니다.")]
    public bool autoStart = false;

    [Header("이벤트")]
    [Tooltip("남은 시간이 1초 줄어들 때마다 실행합니다. 남은 시간을 넘겨줍니다.")]
    public UnityEvent<float> onTick;

    [Tooltip("시간이 다 됐을 때 같이 실행할 동작을 연결합니다.")]
    public UnityEvent onFinished;

    /// <summary>남은 시간(초)입니다.</summary>
    public float Remaining { get; private set; }

    bool running;
    int lastWhole = -1;

    void Start()
    {
        Remaining = duration;
        Report(true);

        if (autoStart) StartTimer();
    }

    void Update()
    {
        if (!running) return;

        Remaining -= Time.deltaTime;

        if (Remaining <= 0f)
        {
            Remaining = 0f;
            running = false;
            Report(true);
            onFinished?.Invoke();
            return;
        }

        Report(false);
    }

    /// <summary>시간을 재기 시작합니다. 게임 시작에 연결해서 씁니다.</summary>
    public void StartTimer()
    {
        running = true;
    }

    /// <summary>시간 재기를 멈춥니다.</summary>
    public void StopTimer()
    {
        running = false;
    }

    /// <summary>남은 시간을 처음으로 되돌립니다.</summary>
    public void ResetTimer()
    {
        running = false;
        Remaining = duration;
        Report(true);
    }

    // 1초 단위로 바뀔 때만 알립니다. 매 순간 알리면 글자가 요동칩니다.
    void Report(bool force)
    {
        int whole = Mathf.CeilToInt(Remaining);
        if (!force && whole == lastWhole) return;

        lastWhole = whole;
        onTick?.Invoke(whole);
    }
}
