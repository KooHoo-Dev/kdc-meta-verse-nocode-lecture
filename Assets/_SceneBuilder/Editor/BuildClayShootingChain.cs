using System;
using UnityEditor;
using UnityEngine;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 사격장 갈래(22 ~ 27차시)를 차례로 다시 만듭니다.
    ///
    /// <b>왜 필요한가</b> — 뒤 차시가 앞 차시의 결과 위에 쌓이기 때문입니다.
    /// <list type="bullet">
    /// <item>`24_시작` 은 `23_완성` 의 복사본입니다</item>
    /// <item>25차시는 <b>23차시가 만든 접시 틀</b>에 `Hittable` 을 붙입니다</item>
    /// </list>
    ///
    /// 그래서 <b>앞 차시 값을 하나만 바꿔도</b> 그 뒤를 전부 다시 만들어야 합니다.
    /// 예) 표적 크기를 키우면 → 23차시가 틀을 새로 만들고 → 25차시가 다시 `Hittable` 을 붙여야 합니다.
    /// </summary>
    public static class BuildClayShootingChain
    {
        static readonly (string label, Action run)[] Steps =
        {
            ("22차시 — 사격장", Build22ClayWorld.Run),
            ("23차시 — 표적", Build23ClayTarget.Run),
            ("24차시 — 총", Build24ClayGun.Run),
            ("25차시 — 점수", Build25ClayHitScore.Run),
            ("26차시 — 난이도·시간", Build26ClayDifficulty.Run),
            ("27차시 — 배경·소리", Build27ClayPolish.Run),
        };

        [MenuItem("Tools/교안 씬 빌더/사격장 전체 다시 만들기 (22 ~ 27)", false, 20)]
        public static void Rebuild()
        {
            if (!EditorUtility.DisplayDialog(
                    "사격장 전체 다시 만들기",
                    "22 ~ 27차시를 차례로 다시 만듭니다.\n" +
                    "그 차시의 씬과 틀은 모두 새로 만들어집니다.\n\n" +
                    "손으로 고쳐두신 것이 있으면 사라집니다.\n" +
                    "저장 안 된 작업이 있으면 먼저 저장해주세요.",
                    "전부 다시 만들기", "그만두기"))
            {
                return;
            }

            try
            {
                for (int i = 0; i < Steps.Length; i++)
                {
                    (string label, Action run) = Steps[i];

                    EditorUtility.DisplayProgressBar(
                        "사격장 전체 다시 만들기",
                        $"{label} …  ({i + 1} / {Steps.Length})",
                        (float)i / Steps.Length);

                    run();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(
                    "❌ 중간에 멈췄습니다. 아래 오류를 고치고 다시 돌려주세요.\n" +
                    "이미 만들어진 앞 차시 씬은 그대로 남아 있습니다.");
                Debug.LogException(e);
                return;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            Debug.Log(
                $"✅ 사격장 갈래를 전부 다시 만들었습니다 ({Steps.Length}개 차시).\n" +
                "차시마다 점검기를 돌린 결과가 위에 함께 나와 있습니다.");
        }
    }
}
