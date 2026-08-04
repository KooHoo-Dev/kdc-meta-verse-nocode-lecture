using UnityEngine;

/// <summary>
/// 총구 불꽃을 한 번 터뜨리는 교육용 부품. 24차시에서 사용합니다.
///
/// ⚠️ <b>만들기 전에 확인하세요.</b>
/// <c>ParticleSystem.Play()</c> 가 버튼 목록에 그대로 나온다면 이 부품은 필요 없습니다.
/// 그때는 <c>SimpleGun</c> 의 <c>On Fired</c> 에 파티클을 직접 연결하는 편이 정직합니다.
///
/// 이 부품의 존재 이유는 <b>불빛까지 같이 껐다 켜는 것</b> 하나뿐입니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Muzzle Flash Player")]
[DisallowMultipleComponent]
public class MuzzleFlashPlayer : MonoBehaviour
{
    [Header("무엇을 터뜨릴지")]
    [Tooltip("총구에서 터질 흩날리는 효과를 끌어다 놓습니다.")]
    public ParticleSystem particle;

    [Tooltip("총구가 번쩍일 때 켤 불빛입니다. 없어도 됩니다.")]
    public Light flashLight;

    [Tooltip("불빛이 켜져 있을 시간(초)입니다.")]
    public float duration = 0.05f;

    float lightTimer;

    void Awake()
    {
        if (flashLight != null) flashLight.enabled = false;
    }

    void Update()
    {
        if (lightTimer <= 0f) return;

        lightTimer -= Time.deltaTime;
        if (lightTimer <= 0f && flashLight != null)
        {
            flashLight.enabled = false;
        }
    }

    /// <summary>총구 불꽃을 한 번 터뜨립니다. Simple Gun 의 On Fired 에 연결해서 씁니다.</summary>
    public void Play()
    {
        if (particle != null)
        {
            particle.Clear(true);
            particle.Play(true);
        }

        if (flashLight != null)
        {
            flashLight.enabled = true;
            lightTimer = duration;
        }

        if (particle == null && flashLight == null)
        {
            Debug.LogWarning($"[{name}] 터뜨릴 효과도 불빛도 지정되지 않았습니다.", this);
        }
    }
}
