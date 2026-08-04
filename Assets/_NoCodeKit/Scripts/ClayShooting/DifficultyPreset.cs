using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 난이도 세 단계를 버튼으로 갈아 끼우는 교육용 부품. 26차시에서 사용합니다.
///
/// 하는 일은 단순합니다. <b>미리 적어둔 값들을 Target Launcher 와 Round Timer 에 옮겨 담는 것</b>뿐이에요.
/// 수강생이 Inspector 에서 직접 바꾸던 값을, 버튼 하나로 한꺼번에 바꿔주는 겁니다.
///
/// <c>On Difficulty Changed</c> 는 <b>난이도 이름을 글자로 넘겨줍니다.</b>
/// TextMeshPro 의 <c>text</c> 에 <c>Dynamic string</c> 으로 연결하면 "난이도: 보통" 처럼 표시됩니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Difficulty Preset")]
[DisallowMultipleComponent]
public class DifficultyPreset : MonoBehaviour
{
    /// <summary>난이도 한 단계에 들어가는 값 묶음입니다.</summary>
    [System.Serializable]
    public class Level
    {
        [Tooltip("화면에 보여줄 이름입니다. 예: 쉬움")]
        public string label = "보통";

        [Tooltip("몇 초마다 표적을 쏘아 올릴지 (작을수록 어려움)")]
        public float interval = 1.5f;

        [Tooltip("표적을 얼마나 세게 쏘아 올릴지 (클수록 어려움)")]
        public float launchForce = 12f;

        [Tooltip("표적 방향이 얼마나 흩어질지 (클수록 어려움)")]
        public float spreadAngle = 25f;

        [Tooltip("한 판의 제한 시간(초) (짧을수록 어려움)")]
        public float duration = 60f;
    }

    [Header("값을 바꿔줄 대상")]
    [Tooltip("난이도 값을 옮겨 담을 Target Launcher 를 끌어다 놓습니다.")]
    public TargetLauncher launcher;

    [Tooltip("제한 시간을 옮겨 담을 Round Timer 를 끌어다 놓습니다.")]
    public RoundTimer timer;

    [Header("난이도 세 단계")]
    public Level easy = new Level { label = "쉬움", interval = 2.5f, launchForce = 8f, spreadAngle = 10f, duration = 90f };
    public Level normal = new Level { label = "보통", interval = 1.5f, launchForce = 12f, spreadAngle = 25f, duration = 60f };
    public Level hard = new Level { label = "어려움", interval = 0.8f, launchForce = 16f, spreadAngle = 40f, duration = 45f };

    [Header("시작할 때")]
    [Tooltip("체크하면 ▶ 를 누를 때 '보통' 난이도를 미리 적용합니다.")]
    public bool applyNormalOnStart = true;

    [Header("이벤트")]
    [Tooltip("난이도가 바뀔 때마다 실행합니다. 난이도 이름을 글자로 넘겨줍니다.")]
    public UnityEvent<string> onDifficultyChanged;

    void Start()
    {
        if (applyNormalOnStart)
        {
            ApplyNormal();
        }
    }

    /// <summary>쉬움으로 바꿉니다. 버튼에 연결해서 씁니다.</summary>
    public void ApplyEasy()
    {
        Apply(easy);
    }

    /// <summary>보통으로 바꿉니다. 버튼에 연결해서 씁니다.</summary>
    public void ApplyNormal()
    {
        Apply(normal);
    }

    /// <summary>어려움으로 바꿉니다. 버튼에 연결해서 씁니다.</summary>
    public void ApplyHard()
    {
        Apply(hard);
    }

    // ── 내부 ─────────────────────────────────────────────

    void Apply(Level level)
    {
        if (level == null) return;

        if (launcher != null)
        {
            launcher.interval = level.interval;
            launcher.launchForce = level.launchForce;
            launcher.spreadAngle = level.spreadAngle;
        }
        else
        {
            Debug.LogWarning($"[{name}] Launcher 가 비어 있어서 표적 값이 안 바뀝니다.", this);
        }

        if (timer != null)
        {
            timer.duration = level.duration;
            timer.ResetTimer();   // 바뀐 시간이 화면에 바로 보이게 합니다
        }
        else
        {
            Debug.LogWarning($"[{name}] Timer 가 비어 있어서 제한 시간이 안 바뀝니다.", this);
        }

        onDifficultyChanged?.Invoke(level.label);
    }
}
