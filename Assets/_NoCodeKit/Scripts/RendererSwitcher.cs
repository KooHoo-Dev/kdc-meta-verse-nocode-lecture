using UnityEngine;

/// <summary>
/// 세 가지 '보이는 방식'을 갈아 끼워보는 교육용 부품. 16차시 · 19차시에서 사용합니다.
///
/// 16차시에는 아직 버튼을 배우기 전이라 <see cref="showMode"/> 목록으로 고르게 합니다.
/// 19차시에서 같은 부품의 ShowMesh / ShowSprite / PlayParticle 을 버튼에 연결합니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Renderer Switcher")]
[DisallowMultipleComponent]
[ExecuteAlways]
public class RendererSwitcher : MonoBehaviour
{
    /// <summary>인스펙터 드롭다운에 Mesh / Sprite / Particle / None 으로 표시됩니다.</summary>
    public enum ShowMode
    {
        Mesh,
        Sprite,
        Particle,
        None,
    }

    [Header("지금 보여줄 것")]
    [Tooltip("무엇을 보여줄지 목록에서 고릅니다. ▶ 를 누르지 않아도 바로 바뀝니다.")]
    public ShowMode showMode = ShowMode.Mesh;

    [Header("갈아 끼울 물건 세 개")]
    [Tooltip("입체 모양으로 보여줄 물건을 끌어다 놓습니다.")]
    public GameObject meshObject;

    [Tooltip("납작한 그림으로 보여줄 물건을 끌어다 놓습니다.")]
    public GameObject spriteObject;

    [Tooltip("흩날리는 효과로 보여줄 물건을 끌어다 놓습니다.")]
    public GameObject particleObject;

    ShowMode applied;
    bool hasApplied;

    void OnEnable()
    {
        Apply(showMode);
    }

    void Update()
    {
        // 인스펙터 목록에서 고른 것을 바로 반영합니다.
        if (!hasApplied || applied != showMode)
        {
            Apply(showMode);
        }
    }

    // ── 버튼에 연결할 동작 (19차시) ───────────────────────

    /// <summary>입체 모양만 보이게 합니다.</summary>
    public void ShowMesh()
    {
        showMode = ShowMode.Mesh;
        Apply(showMode);
    }

    /// <summary>납작한 그림만 보이게 합니다.</summary>
    public void ShowSprite()
    {
        showMode = ShowMode.Sprite;
        Apply(showMode);
    }

    /// <summary>흩날리는 효과를 보이게 하고 처음부터 다시 재생합니다.</summary>
    public void PlayParticle()
    {
        showMode = ShowMode.Particle;
        Apply(showMode);
        Replay();
    }

    /// <summary>셋 다 안 보이게 합니다.</summary>
    public void HideAll()
    {
        showMode = ShowMode.None;
        Apply(showMode);
    }

    // ── 내부 ─────────────────────────────────────────────

    void Apply(ShowMode mode)
    {
        applied = mode;
        hasApplied = true;

        SetActive(meshObject, mode == ShowMode.Mesh);
        SetActive(spriteObject, mode == ShowMode.Sprite);
        SetActive(particleObject, mode == ShowMode.Particle);

        if (meshObject == null && spriteObject == null && particleObject == null)
        {
            Debug.LogWarning(
                $"[{name}] 갈아 끼울 물건이 하나도 지정되지 않았습니다. 세 칸에 물건을 끌어다 놓아주세요.",
                this);
        }
    }

    void Replay()
    {
        if (particleObject == null) return;

        var ps = particleObject.GetComponentInChildren<ParticleSystem>();
        if (ps == null)
        {
            Debug.LogWarning(
                $"[{name}] Particle Object 에 흩날리는 부품(Particle System)이 없습니다.",
                this);
            return;
        }

        ps.Clear(true);
        ps.Play(true);
    }

    static void SetActive(GameObject go, bool on)
    {
        if (go != null && go.activeSelf != on)
        {
            go.SetActive(on);
        }
    }
}
