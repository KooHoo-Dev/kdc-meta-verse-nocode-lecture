using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// 마우스로 둘러보게 하는 교육용 부품. 22차시에서 사용합니다.
///
/// <b>카메라에 붙입니다.</b> 좌우는 부모(Player)를 돌리고, 위아래는 자기(Camera)를 돌립니다.
/// 이게 1인칭의 표준 구조입니다.
///
/// 커서를 잠그면 화면의 버튼을 누를 수 없으므로 <c>Esc</c> 로 풀 수 있게 해두었습니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Mouse Look")]
[DisallowMultipleComponent]
public class MouseLook : MonoBehaviour
{
    [Header("둘러보기 설정")]
    [Tooltip("마우스 감도입니다. 클수록 조금만 움직여도 많이 돕니다.")]
    public float sensitivity = 0.15f;

    [Tooltip("체크하면 위아래가 뒤집힙니다.")]
    public bool invertY = false;

    [Tooltip("아래로 얼마나 내려다볼 수 있는지 (각도)")]
    public float minAngle = -80f;

    [Tooltip("위로 얼마나 올려다볼 수 있는지 (각도)")]
    public float maxAngle = 80f;

    [Tooltip("체크하면 커서가 화면 가운데에 잠깁니다. Esc 로 풀 수 있습니다.")]
    public bool lockCursor = true;

    float pitch;          // 위아래 각도
    Transform playerBody; // 좌우로 돌릴 대상 = 부모

    void Awake()
    {
        playerBody = transform.parent;
        if (playerBody == null)
        {
            Debug.LogWarning(
                $"[{name}] 이 부품은 카메라에 붙이고, 그 카메라를 플레이어의 자식으로 두어야 합니다.",
                this);
        }
        pitch = transform.localEulerAngles.x;
    }

    void OnEnable()
    {
        ApplyCursor(lockCursor);
    }

    void OnDisable()
    {
        ApplyCursor(false);
    }

    void Update()
    {
        // Esc 를 누르면 커서가 풀립니다. 화면의 버튼을 누를 때 필요합니다.
        if (EscapePressed())
        {
            UnlockCursor();
        }

        // 커서가 풀려 있으면 둘러보지 않습니다. 안 그러면 버튼을 누르다가 시점이 돌아갑니다.
        if (Cursor.lockState != CursorLockMode.Locked) return;

        Vector2 delta = ReadLook() * sensitivity;

        // 좌우 — 부모를 돌립니다
        if (playerBody != null)
        {
            playerBody.Rotate(0f, delta.x, 0f, Space.Self);
        }

        // 위아래 — 자기를 돌립니다. 고개가 넘어가지 않게 각도를 제한합니다.
        pitch += invertY ? delta.y : -delta.y;
        pitch = Mathf.Clamp(pitch, minAngle, maxAngle);
        transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }

    /// <summary>커서를 화면 가운데에 잠급니다. 게임 시작에 연결해서 씁니다.</summary>
    public void LockCursor()
    {
        lockCursor = true;
        ApplyCursor(true);
    }

    /// <summary>커서를 풀어 화면의 버튼을 누를 수 있게 합니다.</summary>
    public void UnlockCursor()
    {
        lockCursor = false;
        ApplyCursor(false);
    }

    // ── 내부 ─────────────────────────────────────────────

    static void ApplyCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    static Vector2 ReadLook()
    {
#if ENABLE_INPUT_SYSTEM
        var m = Mouse.current;
        return m == null ? Vector2.zero : m.delta.ReadValue();
#else
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#endif
    }

    static bool EscapePressed()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        return k != null && k.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }
}
