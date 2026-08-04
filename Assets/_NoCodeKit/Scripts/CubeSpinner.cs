using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 물건을 돌리는 교육용 부품. 14차시(자동차)와 19차시(버튼 조작)에서 사용합니다.
///
/// 인스펙터 표시명이 교안의 "조절할 수 있는 값 / 버튼에 연결할 수 있는 동작" 표와
/// 글자까지 1:1로 대응해야 합니다. 항목명을 바꾸면 교안도 함께 고쳐주세요.
/// </summary>
[AddComponentMenu("NoCode Kit/Cube Spinner")]
[DisallowMultipleComponent]
public class CubeSpinner : MonoBehaviour
{
    [Header("회전 설정")]
    [Tooltip("회전 속도입니다. 클수록 빨라지고, 음수를 넣으면 반대 방향으로 돕니다.")]
    public float rotationSpeed = 90f;

    [Tooltip("체크를 끄면 회전이 멈춥니다.")]
    public bool isSpinning = true;

    [Header("색 설정")]
    [Tooltip("Change Color 를 실행할 때 적용할 색입니다. 고른다고 바로 바뀌지는 않습니다.")]
    public Color targetColor = Color.red;

    [Header("이벤트")]
    [Tooltip("회전이 멈추는 순간 같이 실행할 동작을 연결합니다.")]
    public UnityEvent onStopped;

    // 멈추는 '순간'을 한 번만 알리기 위해 직전 상태를 기억해둡니다.
    bool wasSpinning;

    void OnEnable()
    {
        wasSpinning = isSpinning;
    }

    void Update()
    {
        if (isSpinning)
        {
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }

        // 버튼으로 멈추든 인스펙터 체크를 끄든, 멈추는 순간에 똑같이 반응합니다.
        if (wasSpinning && !isSpinning)
        {
            onStopped?.Invoke();
        }

        wasSpinning = isSpinning;
    }

    /// <summary>돌기 시작합니다. 버튼의 On Click 에 연결해서 씁니다.</summary>
    public void StartSpin()
    {
        isSpinning = true;
    }

    /// <summary>멈춥니다. 멈추는 순간 On Stopped 에 연결된 동작이 같이 실행됩니다.</summary>
    public void StopSpin()
    {
        isSpinning = false;
    }

    /// <summary>Target Color 에 골라둔 색으로 바꿉니다.</summary>
    public void ChangeColor()
    {
        // 자기 자신에 보이는 부품이 없으면 자식에서 찾습니다.
        // (14차시처럼 빈 그릇에 붙인 경우를 위해)
        var target = GetComponentInChildren<Renderer>();

        if (target == null)
        {
            // 빨간 오류로 멈춰 세우지 않고, 무엇을 확인하면 되는지만 알려줍니다.
            Debug.LogWarning(
                $"[{name}] 색을 바꿀 대상이 없습니다. 보이는 부품(Mesh Renderer)이 붙어 있는지 확인해주세요.",
                this);
            return;
        }

        target.material.color = targetColor;
    }
}
