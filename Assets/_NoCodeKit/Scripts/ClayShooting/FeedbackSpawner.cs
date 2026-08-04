using UnityEngine;

/// <summary>
/// 그 자리에 효과를 하나 만들어내는 교육용 부품. 25차시에서 사용합니다.
///
/// 표적이 맞아서 사라질 때, <b>터지는 효과만 남기는</b> 용도입니다.
/// 만들어낸 효과는 이 물건의 자식으로 넣지 않습니다. 부모가 사라지면 같이 사라지니까요.
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Feedback Spawner")]
[DisallowMultipleComponent]
public class FeedbackSpawner : MonoBehaviour
{
    [Header("무엇을 만들어낼지")]
    [Tooltip("만들어낼 효과 틀(Prefab)을 끌어다 놓습니다.")]
    public GameObject effectPrefab;

    [Tooltip("이 물건에서 얼마나 떨어진 자리에 만들지 정합니다.")]
    public Vector3 spawnOffset = Vector3.zero;

    /// <summary>효과를 하나 만들어냅니다. On Hit 에 연결해서 씁니다.</summary>
    public void Spawn()
    {
        if (effectPrefab == null)
        {
            Debug.LogWarning($"[{name}] Effect Prefab 이 비어 있습니다. 효과 틀을 끌어다 놓아주세요.", this);
            return;
        }

        // 부모를 지정하지 않습니다. 이 물건이 사라져도 효과는 남아야 하니까요.
        Instantiate(effectPrefab, transform.position + spawnOffset, Quaternion.identity);
    }
}
