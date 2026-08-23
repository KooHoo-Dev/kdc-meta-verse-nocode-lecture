using System;
using UnityEditor;
using UnityEngine;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 갈래 C(28 ~ 31차시)를 차례로 다시 만듭니다.
    ///
    /// <b>왜 필요한가</b> — 뒤 차시가 앞 차시의 씬 위에 쌓입니다.
    /// <list type="bullet">
    /// <item>29차시는 <b>28차시가 놓은 <c>XR Origin</c></b> 의 눈높이를 맞춥니다</item>
    /// <item>31차시는 <b>28차시가 깐 바닥</b>을 순간이동 목적지로 만듭니다</item>
    /// </list>
    ///
    /// 그래서 <b>28차시를 고치면 그 뒤를 전부 다시 만들어야 합니다.</b>
    /// </summary>
    public static class BuildXrChain
    {
        static readonly (string label, Action run)[] Steps =
        {
            ("28차시 — VR 화면 띄우기", Build28XrSetup.Run),
            ("29차시 — 눈높이", Build29XrOriginLocomotion.Run),
            ("30차시 — 360 배경", Build30Vr360.Run),
            ("31차시 — 잡기 · 순간이동", Build31XrInteraction.Run),
        };

        [MenuItem("Tools/교안 씬 빌더/VR 전체 다시 만들기 (28 ~ 31)", false, 12)]
        public static void Rebuild()
        {
            if (!EditorUtility.DisplayDialog(
                    "VR 전체 다시 만들기",
                    $"28 ~ 31차시를 차례로 다시 만듭니다. ({Steps.Length}개 차시)\n" +
                    "그 차시의 씬은 모두 새로 만들어집니다.\n\n" +
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
                        "VR 전체 다시 만들기",
                        $"{label} …  ({i + 1} / {Steps.Length})",
                        (float)i / Steps.Length);

                    run();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(
                    "❌ 중간에 멈췄습니다. 아래 오류를 고치고 다시 돌려주세요.\n" +
                    "이미 만들어진 앞 차시 씬은 그대로 남아 있습니다.\n\n" +
                    "틀을 못 찾는다는 오류라면 — Package Manager 에서\n" +
                    "XR Interaction Toolkit 의 예제 꾸러미(Starter Assets · XR Interaction Simulator)를\n" +
                    "가져왔는지 확인해주세요. (28차시 실습 ② 2단계)");
                Debug.LogException(e);
                return;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            Debug.Log(
                $"✅ 갈래 C 를 전부 다시 만들었습니다 ({Steps.Length}개 차시).\n" +
                "차시마다 점검기를 돌린 결과가 위에 함께 나와 있습니다.");
        }
    }
}
