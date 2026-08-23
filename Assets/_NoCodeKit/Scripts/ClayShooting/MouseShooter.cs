using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

/// <summary>
/// 마우스로 총을 겨누고 쏘게 하는 교육용 부품. 24차시에서 사용합니다.
///
/// <see cref="SimpleGun"/> 과 <b>같은 그릇에</b> 붙입니다. 하는 일은 두 가지뿐입니다.
/// <list type="number">
/// <item>총을 <b>화면 한가운데(십자선) 쪽으로 돌려놓습니다</b></item>
/// <item>마우스를 누르면 <see cref="SimpleGun.Fire"/> 를 불러줍니다</item>
/// </list>
///
/// 총알을 실제로 내보내는 일은 <see cref="SimpleGun"/> 이 합니다.
/// 그래서 <b>이 부품을 떼어내면 총은 남고 마우스 조작만 사라집니다.</b>
/// 32차시에서 VR 로 옮길 때 이 부품을 떼고, 대신 손이 총을 겨눕니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Mouse Shooter")]
[DisallowMultipleComponent]
[RequireComponent(typeof(SimpleGun))]
public class MouseShooter : MonoBehaviour
{
    /// <summary>어느 마우스 버튼으로 쏠지.</summary>
    public enum Button
    {
        Left,
        Right,
        Middle,
    }

    [Header("겨누기")]
    [Tooltip("체크하면 총이 화면 한가운데(십자선) 쪽을 향하게 돌아갑니다.\n" +
             "체크를 끄면 총이 놓인 방향 그대로 나갑니다.")]
    public bool aimAtCrosshair = true;

    [Tooltip("몇 미터 앞에서 총알이 십자선과 만날지 정합니다. 사격장 표적은 25m 쯤에 있습니다. " +
             "이 거리보다 가깝거나 멀면 총알이 십자선에서 조금씩 벗어납니다.")]
    public float zeroDistance = 25f;

    [Header("쏘기")]
    [Tooltip("어느 마우스 버튼으로 쏠지 목록에서 고릅니다.")]
    public Button fireButton = Button.Left;

    [Tooltip("체크하면 누르고 있는 동안 계속 쏩니다.\n" +
             "얼마나 빨리 나가는지는 총의 Fire Rate 가 정합니다.")]
    public bool holdToFire = false;

    SimpleGun gun;

    void Awake()
    {
        gun = GetComponent<SimpleGun>();
    }

    void Update()
    {
        if (aimAtCrosshair) Aim();
        if (FirePressed()) gun.Fire();
    }

    // ── 내부 ─────────────────────────────────────────────

    // 총이 화면 구석에 있어도 십자선 쪽을 향하도록 총을 돌려놓습니다.
    void Aim()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // 십자선(화면 한가운데) 쪽으로 Zero Distance 만큼 나간 자리입니다.
        // 총알은 딱 여기서 십자선과 만납니다.
        Vector3 aim = cam.transform.position + cam.transform.forward * zeroDistance;

        Vector3 from = gun.muzzlePoint != null ? gun.muzzlePoint.position : transform.position;
        Vector3 dir = aim - from;
        if (dir.sqrMagnitude < 0.0001f) return;

        transform.rotation = Quaternion.LookRotation(dir);
    }

    bool FirePressed()
    {
#if ENABLE_INPUT_SYSTEM
        Mouse m = Mouse.current;
        if (m == null) return false;

        ButtonControl b = fireButton switch
        {
            Button.Right => m.rightButton,
            Button.Middle => m.middleButton,
            _ => m.leftButton,
        };
        return holdToFire ? b.isPressed : b.wasPressedThisFrame;
#else
        int index = (int)fireButton;   // 0 = 왼쪽, 1 = 오른쪽, 2 = 가운데
        return holdToFire ? Input.GetMouseButton(index) : Input.GetMouseButtonDown(index);
#endif
    }
}
