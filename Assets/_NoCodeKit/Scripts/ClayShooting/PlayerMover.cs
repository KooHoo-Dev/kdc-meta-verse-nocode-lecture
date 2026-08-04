using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// 1인칭으로 걸어 다니게 하는 교육용 부품. 22차시에서 사용합니다.
///
/// Rigidbody 로 움직이므로 21차시에 배운 물리·충돌과 그대로 이어집니다.
/// 좌우 회전은 <see cref="MouseLook"/> 이 이 물건을 돌려주므로, 여기서는 이동만 다룹니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Player Mover")]
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMover : MonoBehaviour
{
    [Header("움직임 설정")]
    [Tooltip("걷는 속도입니다. 클수록 빨라집니다.")]
    public float moveSpeed = 5f;

    [Tooltip("점프하는 힘입니다. 0 이면 점프하지 않습니다.")]
    public float jumpForce = 5f;

    [Tooltip("체크를 끄면 움직이지 않습니다. 게임이 끝났을 때 꺼두는 용도입니다.")]
    public bool canMove = true;

    Rigidbody body;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        // 걷다가 넘어지지 않도록 회전을 잠급니다. 안 하면 캡슐이 굴러다닙니다.
        body.freezeRotation = true;
    }

    void Update()
    {
        if (!canMove) return;

        if (JumpPressed() && IsGrounded())
        {
            body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            // 멈추라고 했으면 좌우 속도만 지웁니다. 중력은 그대로 둡니다.
            body.linearVelocity = new Vector3(0f, body.linearVelocity.y, 0f);
            return;
        }

        Vector2 input = ReadMove();
        Vector3 dir = (transform.right * input.x + transform.forward * input.y).normalized;

        Vector3 next = dir * moveSpeed;
        next.y = body.linearVelocity.y;   // 위아래는 물리에 맡깁니다
        body.linearVelocity = next;
    }

    /// <summary>움직일 수 있게 합니다. 게임 시작에 연결해서 씁니다.</summary>
    public void EnableMove()
    {
        canMove = true;
    }

    /// <summary>움직이지 못하게 합니다. 게임 종료에 연결해서 씁니다.</summary>
    public void DisableMove()
    {
        canMove = false;
    }

    // ── 입력 ─────────────────────────────────────────────

    static Vector2 ReadMove()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        if (k == null) return Vector2.zero;

        float x = (k.dKey.isPressed ? 1f : 0f) - (k.aKey.isPressed ? 1f : 0f);
        float y = (k.wKey.isPressed ? 1f : 0f) - (k.sKey.isPressed ? 1f : 0f);
        return new Vector2(x, y);
#else
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#endif
    }

    static bool JumpPressed()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        return k != null && k.spaceKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Space);
#endif
    }

    // 발밑에 뭔가 있을 때만 점프하게 합니다. 공중에서 계속 뛰는 걸 막습니다.
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.2f);
    }
}
