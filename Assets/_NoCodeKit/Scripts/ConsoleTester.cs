using UnityEngine;

/// <summary>
/// Console 창에 메시지를 찍어보는 교육용 부품. 13차시에서 사용합니다.
///
/// 빨간 오류 줄을 '일부러' 내보게 해서, 오류 메시지에 대한 공포를 미리 없애는 것이 목적입니다.
/// 로그에 this 를 함께 넘기므로 Console 의 줄을 더블클릭하면 해당 물건이 선택됩니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Console Tester")]
[DisallowMultipleComponent]
public class ConsoleTester : MonoBehaviour
{
    /// <summary>인스펙터 드롭다운에 Info / Warning / Error 로 표시됩니다.</summary>
    public enum MessageKind
    {
        Info,
        Warning,
        Error,
    }

    [Header("메시지 설정")]
    [Tooltip("Console 에 찍을 내용입니다.")]
    public string message = "안녕하세요";

    [Tooltip("어떤 색으로 찍을지 고릅니다. Info 하양 / Warning 노랑 / Error 빨강")]
    public MessageKind logType = MessageKind.Info;

    [Tooltip("체크해두면 ▶ 를 누를 때 자동으로 한 번 찍습니다. 버튼을 아직 안 배운 13차시용입니다.")]
    public bool sendOnStart = true;

    void Start()
    {
        if (sendOnStart)
        {
            SendLog();
        }
    }

    /// <summary>위에 적어둔 내용을 골라둔 종류로 Console 에 찍습니다.</summary>
    public void SendLog()
    {
        switch (logType)
        {
            case MessageKind.Warning:
                Debug.LogWarning(message, this);
                break;

            case MessageKind.Error:
                Debug.LogError(message, this);
                break;

            default:
                Debug.Log(message, this);
                break;
        }
    }
}
