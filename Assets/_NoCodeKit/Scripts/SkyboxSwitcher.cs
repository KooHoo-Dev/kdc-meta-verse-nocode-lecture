using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 360 배경(하늘)을 버튼으로 갈아 끼우는 교육용 부품. 30차시에서 사용합니다.
///
/// 하는 일은 단순합니다. <b>미리 등록해둔 하늘 재료를 하나씩 꺼내 씌우는 것</b>뿐이에요.
/// 27차시에 Lighting 창에서 손으로 갈아 끼우던 것을, 버튼 하나로 바꿔주는 겁니다.
///
/// <c>On Skybox Changed</c> 는 <b>배경 이름을 글자로 넘겨줍니다.</b>
/// TextMeshPro 의 <c>text</c> 에 <c>Dynamic string</c> 으로 연결하면 "지금 배경: 노을" 처럼 표시됩니다.
/// 26차시의 Difficulty Preset 과 같은 방식입니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Skybox Switcher")]
[DisallowMultipleComponent]
public class SkyboxSwitcher : MonoBehaviour
{
    /// <summary>배경 하나에 들어가는 묶음입니다.</summary>
    [System.Serializable]
    public class Sky
    {
        [Tooltip("화면에 보여줄 이름입니다. 비워두면 재료 이름을 그대로 씁니다. 예: 노을")]
        public string label = "";

        [Tooltip("씌울 하늘 재료(Material)를 끌어다 놓습니다.")]
        public Material material;
    }

    [Header("갈아 끼울 배경들")]
    [Tooltip("배경을 원하는 만큼 등록합니다. + 를 눌러 칸을 늘리세요.")]
    public Sky[] skyboxes = new Sky[0];

    [Header("시작할 때")]
    [Tooltip("▶ 를 누를 때 몇 번째 배경으로 시작할지 정합니다. 맨 처음이 0 입니다.")]
    public int startIndex = 0;

    [Header("이벤트")]
    [Tooltip("배경이 바뀔 때마다 실행합니다. 배경 이름을 글자로 넘겨줍니다.")]
    public UnityEvent<string> onSkyboxChanged;

    /// <summary>지금 보여주고 있는 배경 번호입니다.</summary>
    public int Current { get; private set; }

    void Start()
    {
        if (!HasAny()) return;

        Apply(startIndex);
    }

    // ── 버튼에 연결할 동작 ────────────────────────────────

    /// <summary>다음 배경으로 넘어갑니다. 마지막이면 처음으로 돌아옵니다.</summary>
    public void Next()
    {
        if (!HasAny()) return;

        Apply(Current + 1);
    }

    /// <summary>이전 배경으로 돌아갑니다. 처음이면 마지막으로 넘어갑니다.</summary>
    public void Previous()
    {
        if (!HasAny()) return;

        Apply(Current - 1);
    }

    /// <summary>번호로 바로 고릅니다. 맨 처음이 0 입니다.</summary>
    public void Apply(int index)
    {
        if (!HasAny()) return;

        int count = skyboxes.Length;

        // 목록 밖의 번호가 들어와도 목록 안으로 감아 넣습니다.
        Current = ((index % count) + count) % count;

        Sky sky = skyboxes[Current];
        if (sky == null || sky.material == null)
        {
            Debug.LogWarning(
                $"[{name}] {Current} 번 칸에 하늘 재료가 비어 있습니다. 재료를 끌어다 놓아주세요.",
                this);
            return;
        }

        RenderSettings.skybox = sky.material;

        // 하늘이 바뀌면 주변 밝기도 같이 바뀌어야 자연스럽습니다.
        DynamicGI.UpdateEnvironment();

        onSkyboxChanged?.Invoke(NameOf(sky));
    }

    // ── 내부 ─────────────────────────────────────────────

    bool HasAny()
    {
        if (skyboxes != null && skyboxes.Length > 0) return true;

        Debug.LogWarning(
            $"[{name}] 갈아 끼울 배경이 하나도 등록되지 않았습니다. Skyboxes 목록에 하늘 재료를 넣어주세요.",
            this);
        return false;
    }

    static string NameOf(Sky sky)
    {
        if (!string.IsNullOrWhiteSpace(sky.label)) return sky.label;

        return sky.material != null ? sky.material.name : "";
    }
}
