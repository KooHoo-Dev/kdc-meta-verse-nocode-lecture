using System;
using UnityEditor;
using UnityEngine;

namespace NoCodeKit.SceneBuilder
{
    /// <summary>
    /// 갈래 A(13 ~ 21차시)를 차례로 다시 만듭니다.
    ///
    /// <b>왜 필요한가</b> — 갈래 A는 <b>아홉 차시가 한 씬에 계속 쌓입니다.</b>
    /// <list type="bullet">
    /// <item>`15_시작` 은 `14_완성` 의 복사본입니다</item>
    /// <item>15차시는 <b>14차시가 만든 자동차</b>를 틀로 바꿉니다</item>
    /// <item>19차시는 <b>14차시 자동차</b>와 <b>17 · 18차시 화면</b>을 함께 씁니다</item>
    /// <item>20차시는 <b>19차시가 만든 조작 패널 틀</b>을 다시 씁니다</item>
    /// </list>
    ///
    /// 그래서 <b>앞 차시를 하나만 고쳐도</b> 그 뒤를 전부 다시 만들어야 합니다.
    /// 예) 자동차 바퀴 회전을 고치면 → 15차시가 틀을 새로 만들고 → 19 · 20차시가 다시 연결해야 합니다.
    ///
    /// <b>갈래 A 아홉 차시가 전부 들어 있습니다.</b>
    /// </summary>
    public static class BuildBasicsChain
    {
        static readonly (string label, Action run)[] Steps =
        {
            ("13차시 — 에디터 · 재료 · Console", Build13UnityEditor.Run),
            ("14차시 — 자동차", Build14GameObject.Run),
            ("15차시 — 틀과 다섯 대", Build15DataPrefab.Run),
            ("16차시 — 갈아 끼우기", Build16Renderer.Run),
            ("17차시 — 안 깨지는 화면 틀", Build17UguiCanvas.Run),
            ("18차시 — 버튼과 슬라이더", Build18UguiButtonSlider.Run),
            ("19차시 — 조작 패널", Build19UguiEventSystem.Run),
            ("20차시 — 버튼으로 움직이기", Build20Transform.Run),
            ("21차시 — 떨어뜨리고 부딪히기", Build21RigidbodyCollider.Run),
        };

        [MenuItem("Tools/교안 씬 빌더/기초 전체 다시 만들기 (13 ~ 21)", false, 11)]
        public static void Rebuild()
        {
            if (!EditorUtility.DisplayDialog(
                    "기초 전체 다시 만들기",
                    $"13 ~ 21차시를 차례로 다시 만듭니다. ({Steps.Length}개 차시)\n" +
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
                        "기초 전체 다시 만들기",
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
                $"✅ 갈래 A 를 전부 다시 만들었습니다 ({Steps.Length}개 차시).\n" +
                "차시마다 점검기를 돌린 결과가 위에 함께 나와 있습니다.");
        }
    }
}
