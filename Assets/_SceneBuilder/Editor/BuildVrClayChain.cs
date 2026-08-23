using System;
using UnityEditor;
using UnityEngine;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 갈래 D(32 · 33차시)를 차례로 다시 만듭니다.
    ///
    /// <b>이 갈래는 갈래 B 위에 얹힙니다.</b>
    /// 32차시가 <c>27_완성</c> 을 복제해서 시작하므로,
    /// <b>사격장을 고쳤으면 여기도 다시 돌려야</b> 합니다.
    ///
    /// 순서 — <c>사격장 전체 다시 만들기(22 ~ 27)</c> → <b>이것</b>.
    /// </summary>
    public static class BuildVrClayChain
    {
        static readonly (string label, Action run)[] Steps =
        {
            ("32차시 — VR로 총을 잡고 쏘기", Build32VrClayShooting.Run),
            ("33차시 — 사격장이 여러 개인 게임월드", Build33VrClayWorld.Run),
        };

        [MenuItem("Tools/교안 씬 빌더/VR 사격 전체 다시 만들기 (32 ~ 33)", false, 13)]
        public static void Rebuild()
        {
            if (!EditorUtility.DisplayDialog(
                    "VR 사격 전체 다시 만들기",
                    $"32 ~ 33차시를 차례로 다시 만듭니다. ({Steps.Length}개 차시)\n" +
                    "27_완성 씬이 먼저 있어야 합니다.\n\n" +
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
                        "VR 사격 전체 다시 만들기",
                        $"{label} …  ({i + 1} / {Steps.Length})",
                        (float)i / Steps.Length);

                    run();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(
                    "❌ 중간에 멈췄습니다. 아래 오류를 고치고 다시 돌려주세요.\n\n" +
                    "«27_완성 을 못 찾았다» 는 오류라면 —\n" +
                    "Tools ▸ 교안 씬 빌더 ▸ 사격장 전체 다시 만들기 (22 ~ 27) 을 먼저 돌려주세요.\n\n" +
                    "«틀을 못 찾았다» 는 오류라면 —\n" +
                    "XR Interaction Toolkit 의 예제 꾸러미를 가져왔는지 확인해주세요. (28차시 실습 ②)");
                Debug.LogException(e);
                return;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            Debug.Log(
                $"✅ 갈래 D 를 전부 다시 만들었습니다 ({Steps.Length}개 차시).\n" +
                "**이걸로 11 ~ 33차시 전 과정의 씬이 갖춰졌습니다.**");
        }
    }
}
