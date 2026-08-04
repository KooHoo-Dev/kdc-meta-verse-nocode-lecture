using UnityEngine;

/// <summary>
/// 숫자를 글자로 바꿔서 화면에 보여주는 교육용 부품. 18차시에서 사용합니다.
///
/// 슬라이더의 On Value Changed 에 SetValue 를 연결하면
/// 슬라이더를 움직일 때마다 글자가 따라 바뀝니다.
/// 글자 부품(TextMeshPro 또는 Text)이 붙어 있는 물건에 같이 붙여주세요.
/// </summary>
[AddComponentMenu("NoCode Kit/Value Display")]
[DisallowMultipleComponent]
public class ValueDisplay : MonoBehaviour
{
    [Header("표시 방법")]
    [Tooltip("숫자 앞에 붙일 글자입니다. 예: \"점수: \"")]
    public string prefix = "점수: ";

    [Tooltip("숫자 뒤에 붙일 글자입니다. 예: \" 점\"")]
    public string suffix = "";

    [Tooltip("소수점 아래를 몇 자리까지 보여줄지 정합니다. 0이면 정수로만 보입니다.")]
    [Range(0, 3)]
    public int decimals = 0;

    // TextMeshPro 와 옛 Text 를 모두 받아줍니다. 어느 쪽을 쓰셔도 동작합니다.
    TMPro.TMP_Text tmpText;
    UnityEngine.UI.Text uiText;
    bool searched;

    /// <summary>숫자를 받아 글자로 바꿔 보여줍니다. 슬라이더에 연결해서 씁니다.</summary>
    public void SetValue(float value)
    {
        Find();

        string body = value.ToString("F" + decimals);
        string result = prefix + body + suffix;

        if (tmpText != null)
        {
            tmpText.text = result;
        }
        else if (uiText != null)
        {
            uiText.text = result;
        }
        else
        {
            Debug.LogWarning(
                $"[{name}] 글자를 보여줄 대상이 없습니다. 글자 부품이 붙어 있는 물건에 같이 붙여주세요.",
                this);
        }
    }

    void Find()
    {
        if (searched) return;
        searched = true;

        tmpText = GetComponentInChildren<TMPro.TMP_Text>();
        if (tmpText == null)
        {
            uiText = GetComponentInChildren<UnityEngine.UI.Text>();
        }
    }
}
