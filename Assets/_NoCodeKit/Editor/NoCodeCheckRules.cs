using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEditorInternal;
using Object = UnityEngine.Object;

namespace NoCodeKit.EditorTools
{
    /// <summary>점검에서 발견한 것 하나.</summary>
    public struct NoCodeIssue
    {
        /// <summary>수강생이 읽을 문장입니다.</summary>
        public string Message;

        /// <summary>콘솔에서 클릭하면 잡힐 대상입니다.</summary>
        public Object Context;
    }

    /// <summary>
    /// 실습 점검 규칙. 수강생이 <b>확실히 잘못한 것만</b> 지적합니다.
    ///
    /// 오탐이 한 번이라도 나오면 수강생은 그 뒤로 이 도구를 믿지 않습니다.
    /// 그래서 "아직 안 한 것" 은 지적하지 않습니다. 그건 차시별 확인이 할 일입니다.
    /// </summary>
    public static class NoCodeCheckRules
    {
        // ── 반드시 채워야 하는 칸 ──────────────────────────────
        // 근거: 부품이 스스로 Debug.LogWarning 을 내는 칸만 넣었습니다.
        //       (▶ 를 누르고 그 상황이 와야 나오는 걸, 미리 알려주는 겁니다)
        static readonly Dictionary<string, string[]> RequiredFields =
            new Dictionary<string, string[]>
            {
                { "SimpleGun",        new[] { "bulletPrefab" } },
                { "TargetLauncher",   new[] { "targetPrefab" } },
                { "FeedbackSpawner",  new[] { "effectPrefab" } },
                { "DifficultyPreset", new[] { "launcher", "timer" } },
            };

        // 칸마다 덧붙일 한 줄 안내입니다.
        static readonly Dictionary<string, string> FieldHint =
            new Dictionary<string, string>
            {
                { "bulletPrefab",  "총알이 안 나갑니다. 재료함의 Bullet 틀을 끌어다 놓아주세요." },
                { "targetPrefab",  "표적이 안 날아옵니다. 재료함의 Target 틀을 끌어다 놓아주세요." },
                { "effectPrefab",  "맞아도 효과가 안 터집니다. 효과 틀을 끌어다 놓아주세요." },
                { "launcher",      "난이도를 바꿔도 표적 쪽이 안 바뀝니다. 발사기를 끌어다 놓아주세요." },
                { "timer",         "난이도를 바꿔도 제한 시간이 안 바뀝니다. 시간 담당을 끌어다 놓아주세요." },
            };

        // ── 이름표(Tag)를 적는 칸 ─────────────────────────────
        static readonly Dictionary<string, string[]> TagFields =
            new Dictionary<string, string[]>
            {
                { "Projectile",        new[] { "targetTag" } },
                { "Hittable",          new[] { "hitterTag" } },
                { "GameStarter",       new[] { "triggerTag" } },
                { "CollisionReporter", new[] { "targetTag" } },
            };

        const string MenuPrefix = "NoCode Kit/";

        // ══════════════════════════════════════════════════════
        //  진입점
        // ══════════════════════════════════════════════════════

        /// <summary>지금 열려 있는 작업대와 재료함의 틀을 모두 점검합니다.</summary>
        public static List<NoCodeIssue> CheckAll()
        {
            var found = new List<NoCodeIssue>();

            foreach (var mb in Object.FindObjectsByType<MonoBehaviour>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                CheckOne(mb, found);
            }

            foreach (var mb in LoadKitComponentsInPrefabs())
            {
                CheckOne(mb, found);
            }

            return found;
        }

        static void CheckOne(MonoBehaviour mb, List<NoCodeIssue> found)
        {
            if (mb == null) return;
            if (!IsKitComponent(mb.GetType())) return;

            CheckRequiredFields(mb, found);
            CheckTagFields(mb, found);
            CheckEvents(mb, found);
        }

        // ══════════════════════════════════════════════════════
        //  1. 비어 있는 칸
        // ══════════════════════════════════════════════════════

        static void CheckRequiredFields(MonoBehaviour mb, List<NoCodeIssue> found)
        {
            if (!RequiredFields.TryGetValue(mb.GetType().Name, out string[] names)) return;

            foreach (string fieldName in names)
            {
                FieldInfo f = mb.GetType().GetField(fieldName,
                    BindingFlags.Public | BindingFlags.Instance);
                if (f == null) continue;

                var value = f.GetValue(mb) as Object;
                if (value != null) continue;

                string label = ObjectNames.NicifyVariableName(fieldName);
                string hint = FieldHint.TryGetValue(fieldName, out string h) ? " " + h : "";

                found.Add(new NoCodeIssue
                {
                    Message = $"[{Where(mb)}] {label} 칸이 비어 있습니다.{hint}",
                    Context = mb,
                });
            }
        }

        // ══════════════════════════════════════════════════════
        //  2. 이름표(Tag)
        // ══════════════════════════════════════════════════════

        static void CheckTagFields(MonoBehaviour mb, List<NoCodeIssue> found)
        {
            if (!TagFields.TryGetValue(mb.GetType().Name, out string[] names)) return;

            foreach (string fieldName in names)
            {
                FieldInfo f = mb.GetType().GetField(fieldName,
                    BindingFlags.Public | BindingFlags.Instance);
                if (f == null) continue;

                string tag = f.GetValue(mb) as string;
                if (string.IsNullOrEmpty(tag)) continue;      // 비워두는 건 허용된 사용법입니다
                if (Array.IndexOf(InternalEditorUtility.tags, tag) >= 0) continue;

                string label = ObjectNames.NicifyVariableName(fieldName);
                found.Add(new NoCodeIssue
                {
                    Message =
                        $"[{Where(mb)}] {label} 에 적힌 이름표 '{tag}' 가 아직 만들어지지 않았습니다. " +
                        "Edit → Project Settings → Tags and Layers 에서 먼저 추가해주세요.",
                    Context = mb,
                });
            }
        }

        // ══════════════════════════════════════════════════════
        //  3. 이벤트 칸 연결
        // ══════════════════════════════════════════════════════

        static void CheckEvents(MonoBehaviour mb, List<NoCodeIssue> found)
        {
            var so = new SerializedObject(mb);

            foreach (FieldInfo f in mb.GetType().GetFields(
                         BindingFlags.Public | BindingFlags.Instance))
            {
                if (!typeof(UnityEventBase).IsAssignableFrom(f.FieldType)) continue;

                SerializedProperty calls =
                    so.FindProperty(f.Name + ".m_PersistentCalls.m_Calls");
                if (calls == null || !calls.isArray) continue;

                // 값을 넘겨주는 칸인지, 넘겨주는 값이 무엇인지 알아냅니다.
                Type valueType = PassedValueType(f.FieldType);
                string eventName = ObjectNames.NicifyVariableName(f.Name);

                for (int i = 0; i < calls.arraySize; i++)
                {
                    SerializedProperty call = calls.GetArrayElementAtIndex(i);
                    var target = call.FindPropertyRelative("m_Target").objectReferenceValue;
                    string method = call.FindPropertyRelative("m_MethodName").stringValue;
                    int mode = call.FindPropertyRelative("m_Mode").enumValueIndex;

                    string at = $"[{Where(mb)}] {eventName} 의 {i + 1}번째 줄";

                    if (target == null)
                    {
                        found.Add(new NoCodeIssue
                        {
                            Message = at + " 이 끊어져 있습니다. " +
                                      "연결해둔 것을 지웠거나 이름을 바꾸면 이렇게 됩니다. 다시 끌어다 놓아주세요.",
                            Context = mb,
                        });
                        continue;
                    }

                    if (string.IsNullOrEmpty(method))
                    {
                        found.Add(new NoCodeIssue
                        {
                            Message = at + " 에 할 일이 안 골라져 있습니다. 오른쪽 목록에서 동작을 골라주세요.",
                            Context = mb,
                        });
                        continue;
                    }

                    // EventDefined(0) = 목록 위쪽 Dynamic. 그 밖은 미리 적어둔 고정값입니다.
                    //
                    // 다만 <b>위쪽에 고를 것이 있었을 때만</b> 지적합니다.
                    // 예) 잡았을 때 · 놓았을 때 같은 칸에 인자 없는 동작을 걸면
                    //     애초에 위쪽에 안 나오므로 아래쪽인 게 정상입니다.
                    if (valueType != null && mode != 0 && CanTakeValue(target, method, valueType))
                    {
                        found.Add(new NoCodeIssue
                        {
                            Message = at +
                                $" ({method}) 이 **고정값** 으로 걸려 있습니다. " +
                                "바뀌는 값이 전달되지 않아서 화면이 그대로입니다. " +
                                "목록에서 **위쪽 Dynamic** 쪽을 다시 골라주세요. " +
                                "(고정값을 일부러 쓰신 거라면 그냥 두셔도 됩니다)",
                            Context = mb,
                        });
                    }
                }
            }
        }

        // ══════════════════════════════════════════════════════
        //  도우미
        // ══════════════════════════════════════════════════════

        /// <summary>
        /// 이 이벤트 칸이 <b>값을 넘겨주는 칸</b>이면 그 값의 종류를, 아니면 <c>null</c> 을 돌려줍니다.
        ///
        /// <c>UnityEvent&lt;float&gt;</c> 처럼 바로 쓰는 것도 있고,
        /// XRI 의 <c>SelectEnterEvent</c> 처럼 <b>이름을 붙여 물려받은 것</b>도 있어서 위로 훑어봅니다.
        /// </summary>
        static Type PassedValueType(Type eventType)
        {
            for (Type t = eventType; t != null; t = t.BaseType)
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(UnityEvent<>))
                {
                    return t.GetGenericArguments()[0];
                }
            }
            return null;
        }

        /// <summary>
        /// 그 동작이 <b>넘겨주는 값을 받을 수 있는지</b> 봅니다.
        ///
        /// 받을 수 있으면 목록 위쪽(Dynamic)에도 나왔다는 뜻이라, 아래쪽을 고른 건 실수입니다.
        /// 못 받으면 애초에 위쪽에 안 나오므로 아래쪽인 게 정상입니다.
        /// </summary>
        static bool CanTakeValue(Object target, string methodName, Type valueType)
        {
            if (target == null || string.IsNullOrEmpty(methodName)) return false;

            return target.GetType().GetMethod(
                       methodName,
                       BindingFlags.Public | BindingFlags.Instance,
                       null,
                       new[] { valueType },
                       null) != null;
        }

        /// <summary>NoCode Kit 부품인지. 22개 전부 AddComponentMenu 를 갖고 있습니다.</summary>
        public static bool IsKitComponent(Type t)
        {
            var a = (AddComponentMenu)Attribute.GetCustomAttribute(t, typeof(AddComponentMenu));
            return a != null
                   && !string.IsNullOrEmpty(a.componentMenu)
                   && a.componentMenu.StartsWith(MenuPrefix, StringComparison.Ordinal);
        }

        /// <summary>메시지 앞에 붙일 자리 이름입니다. 틀 안이면 틀 이름이 나옵니다.</summary>
        static string Where(MonoBehaviour mb)
        {
            return mb.gameObject.name;
        }

        /// <summary>재료함의 틀(Prefab) 안에 있는 부품도 봅니다. 표적·총알이 여기 있습니다.</summary>
        static IEnumerable<MonoBehaviour> LoadKitComponentsInPrefabs()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                // 유니티가 딸려 준 예제까지 볼 필요는 없습니다.
                if (path.StartsWith("Assets/Samples", StringComparison.Ordinal)) continue;
                if (path.StartsWith("Assets/TutorialInfo", StringComparison.Ordinal)) continue;

                var root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (root == null) continue;

                foreach (var mb in root.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (mb != null && IsKitComponent(mb.GetType()))
                        yield return mb;
                }
            }
        }
    }
}
