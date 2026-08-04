using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 일정 시간이 지나면 스스로 사라지는 교육용 부품. 23차시에서 사용합니다.
///
/// 못 맞힌 표적을 치우는 역할입니다. 이게 없으면 표적이 무한히 쌓입니다.
/// </summary>
[AddComponentMenu("NoCode Kit/Clay Shooting/Target Life")]
[DisallowMultipleComponent]
public class TargetLife : MonoBehaviour
{
    [Header("수명")]
    [Tooltip("몇 초 뒤에 사라질지 정합니다.")]
    public float lifeTime = 5f;

    [Header("이벤트")]
    [Tooltip("수명이 다해 사라지는 순간 같이 실행할 동작을 연결합니다. (못 맞혔을 때)")]
    public UnityEvent onExpired;

    float timer;
    bool finished;

    void Update()
    {
        if (finished) return;

        timer += Time.deltaTime;
        if (timer < lifeTime) return;

        finished = true;
        onExpired?.Invoke();
        Destroy(gameObject);
    }
}
