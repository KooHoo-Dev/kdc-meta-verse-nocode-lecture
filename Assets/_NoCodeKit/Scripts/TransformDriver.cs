using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 버튼으로 물건의 위치·회전·크기를 실시간으로 바꿔보는 교육용 부품. 20차시에서 사용합니다.
///
/// X·Y·Z 가 각각 어느 방향인지를 '눌러서 확인'하게 하는 것이 목적이라,
/// 동작 이름에 축 이름을 그대로 남겨두었습니다. (MoveXPlus / MoveXMinus …)
/// </summary>
[AddComponentMenu("NoCode Kit/Transform Driver")]
[DisallowMultipleComponent]
public class TransformDriver : MonoBehaviour
{
    [Header("한 번에 얼마나 바꿀지")]
    [Tooltip("이동 버튼을 한 번 누를 때 움직일 거리입니다.")]
    public float moveStep = 0.5f;

    [Tooltip("회전 버튼을 한 번 누를 때 돌아갈 각도입니다.")]
    public float rotateStep = 15f;

    [Tooltip("크기 버튼을 한 번 누를 때 곱해질 배율입니다. 1.2 면 20% 씩 커집니다.")]
    public float scaleStep = 1.2f;

    [Header("이벤트")]
    [Tooltip("위치·회전·크기가 바뀔 때마다 같이 실행할 동작을 연결합니다.")]
    public UnityEvent onChanged;

    // 처음 상태를 기억해뒀다가 Reset All 에서 되돌립니다.
    Vector3 startPosition;
    Quaternion startRotation;
    Vector3 startScale;

    void Awake()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
        startScale = transform.localScale;
    }

    // ── 이동 ─────────────────────────────────────────────

    /// <summary>X 방향(빨강, 좌우)으로 + 만큼 옮깁니다.</summary>
    public void MoveXPlus() { Move(Vector3.right); }

    /// <summary>X 방향(빨강, 좌우)으로 − 만큼 옮깁니다.</summary>
    public void MoveXMinus() { Move(Vector3.left); }

    /// <summary>Y 방향(초록, 위아래)으로 + 만큼 옮깁니다.</summary>
    public void MoveYPlus() { Move(Vector3.up); }

    /// <summary>Y 방향(초록, 위아래)으로 − 만큼 옮깁니다.</summary>
    public void MoveYMinus() { Move(Vector3.down); }

    /// <summary>Z 방향(파랑, 앞뒤)으로 + 만큼 옮깁니다.</summary>
    public void MoveZPlus() { Move(Vector3.forward); }

    /// <summary>Z 방향(파랑, 앞뒤)으로 − 만큼 옮깁니다.</summary>
    public void MoveZMinus() { Move(Vector3.back); }

    // ── 회전 ─────────────────────────────────────────────

    /// <summary>Y 축을 기준으로 + 방향으로 돌립니다.</summary>
    public void RotateYPlus() { Rotate(rotateStep); }

    /// <summary>Y 축을 기준으로 − 방향으로 돌립니다.</summary>
    public void RotateYMinus() { Rotate(-rotateStep); }

    // ── 크기 ─────────────────────────────────────────────

    /// <summary>커집니다.</summary>
    public void ScaleUp() { Scale(scaleStep); }

    /// <summary>작아집니다.</summary>
    public void ScaleDown()
    {
        // 0으로 나누는 일이 없도록 막아둡니다.
        Scale(Mathf.Approximately(scaleStep, 0f) ? 1f : 1f / scaleStep);
    }

    // ── 되돌리기 ──────────────────────────────────────────

    /// <summary>처음 상태로 한 번에 되돌립니다.</summary>
    public void ResetAll()
    {
        transform.localPosition = startPosition;
        transform.localRotation = startRotation;
        transform.localScale = startScale;
        onChanged?.Invoke();
    }

    // ── 내부 ─────────────────────────────────────────────

    void Move(Vector3 dir)
    {
        transform.localPosition += dir * moveStep;
        onChanged?.Invoke();
    }

    void Rotate(float angle)
    {
        transform.Rotate(0f, angle, 0f, Space.Self);
        onChanged?.Invoke();
    }

    void Scale(float factor)
    {
        transform.localScale *= factor;
        onChanged?.Invoke();
    }
}
