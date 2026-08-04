using UnityEngine;

/// <summary>
/// 소리를 재생하는 교육용 부품. 27차시에서 사용합니다.
///
/// ⚠️ <b>만들기 전에 확인하세요.</b>
/// <c>AudioSource.Play()</c> 가 버튼 목록에 그대로 나온다면 이 부품은 필요 없습니다.
/// 그때는 <c>AudioSource</c> 를 붙이고 이벤트에 직접 연결하는 편이 정직합니다.
///
/// 이 부품의 존재 이유는 <b>Random Pitch</b> 하나입니다.
/// 총소리가 매번 똑같으면 금방 어색해지는데, 높낮이를 조금씩 흔들면 훨씬 자연스럽습니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Audio Player")]
[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [Header("소리 설정")]
    [Tooltip("재생할 소리 파일을 끌어다 놓습니다. 비워두면 AudioSource 에 지정된 것을 씁니다.")]
    public AudioClip clip;

    [Tooltip("소리 크기입니다. 0 이면 안 들리고 1 이면 최대입니다.")]
    [Range(0f, 1f)]
    public float volume = 1f;

    [Tooltip("소리의 높낮이를 얼마나 흔들지 정합니다. 0 이면 매번 똑같은 소리가 납니다.")]
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;

    AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
    }

    /// <summary>소리를 한 번 재생합니다. 이벤트에 연결해서 씁니다.</summary>
    public void Play()
    {
        AudioClip target = clip != null ? clip : source.clip;

        if (target == null)
        {
            Debug.LogWarning($"[{name}] 재생할 소리가 지정되지 않았습니다.", this);
            return;
        }

        source.pitch = 1f + Random.Range(-randomPitch, randomPitch);
        source.PlayOneShot(target, volume);
    }

    /// <summary>재생 중인 소리를 멈춥니다.</summary>
    public void Stop()
    {
        source.Stop();
    }
}
