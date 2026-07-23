---
title: 15. 씬 관리 (SceneManagement)
---

# **15. 씬 관리 (SceneManagement)**
> **”게임이라는 책의 페이지를 넘기는 방법”**

메인 메뉴에서 게임 레벨로, 다시 엔딩 크레딧으로. 게임의 흐름을 구성하는 필수 기술, **씬(Scene) 관리** 방법에 대해 알아봅니다.

- **대상**: C# 프로그래밍 입문자, Unity 입문자
- **핵심 키워드**: `Scene`, `SceneManager`, `LoadScene`, `DontDestroyOnLoad`

---

## **씬 관리(Scene Management)?**
Unity에서 **씬(Scene)** 이란 게임의 한 장면을 담는 독립적인 공간입니다. 메인 메뉴, 레벨 1, 보스 스테이지, 상점 등 각기 다른 장면을 별개의 씬 파일(.unity)로 만들어 관리합니다.

- **비유**: 게임을 한 권의 책이라고 생각해 보세요. 각 씬은 책의 '챕터'에 해당합니다. '1장: 프롤로그', '2장: 튜토리얼', '3장: 마지막 결전'처럼 말이죠. **씬 관리**란 바로 이 책의 페이지를 넘겨 하나의 챕터에서 다른 챕터로 이동하는 기술과 같습니다.

- **정의**: 씬 관리는 게임의 흐름에 따라 **필요한 씬을 불러오고(Load), 현재 씬을 내리며(Unload), 씬과 씬 사이의 데이터를 전달하는 모든 과정**을 의미합니다. Unity에서는 `UnityEngine.SceneManagement` 네임스페이스에 포함된 `SceneManager` 클래스를 통해 이 모든 것을 제어합니다.

- **왜 필요한가?**: 만약 게임 전체를 단 하나의 씬으로 만든다면 어떻게 될까요? 로딩 시간은 끔찍하게 길어지고, 수많은 오브젝트가 얽혀 관리가 불가능해질 겁니다. 게임을 여러 개의 작은 씬 단위로 분리하면 **개발 효율성**이 높아지고, 필요한 부분만 메모리에 올리므로 **성능 최적화**에도 유리합니다.

---

## **핵심 요약**
- **`Scene`**: 게임의 레벨, 메뉴 등 독립적인 장면을 구성하는 기본 단위입니다.
- **`UnityEngine.SceneManagement`**: 씬 관리에 필요한 모든 기능(클래스, 메소드)을 담고 있는 핵심 네임스페이스입니다. 스크립트 상단에 `using UnityEngine.SceneManagement;`를 선언해야 합니다.
- **`SceneManager`**: 씬을 불러오고, 현재 씬 정보를 얻는 등 씬과 관련된 모든 작업을 처리하는 **정적(static) 클래스**입니다.
- **Build Settings**: 스크립트를 통해 씬을 불러오려면, 반드시 **`File > Build Settings`** 의 `Scenes In Build` 목록에 해당 씬을 추가해야 합니다.
- **`DontDestroyOnLoad`**: 씬이 전환될 때 특정 게임 오브젝트가 파괴되지 않고 계속 유지되도록 만들어주는 중요한 함수입니다.

---

## **세부 개념**
### **1. Build Settings에 씬 추가하기**
가장 먼저, 그리고 가장 중요한 단계입니다. Unity는 Build Settings에 등록된 씬들만 '존재하는 씬'으로 인식합니다.

1.  상단 메뉴에서 `File > Build Settings...`를 엽니다.
2.  `Scenes In Build` 라고 표시된 큰 상자가 보입니다.
3.  Project 창에서 씬 파일(.unity)들을 이 상자 안으로 끌어다 놓습니다.
4.  등록된 씬들은 오른쪽에 **Build Index**라는 고유 번호(0부터 시작)를 갖게 됩니다. 이 번호나 씬의 이름(문자열)으로 씬을 불러올 수 있습니다.

> **Warning**: 이 과정 없이는 `SceneManager`가 씬을 찾지 못해 오류가 발생합니다!

### **2. 동기적 씬 로딩 (Synchronous Loading)**
가장 간단한 씬 전환 방법으로, 다음 씬 로딩이 완료될 때까지 게임 전체가 잠시 멈춥니다.

- **문법 구조**:
  ```csharp
  // using UnityEngine.SceneManagement; // 스크립트 상단에 추가!

  // 이름으로 씬 불러오기
  SceneManager.LoadScene("SceneName");

  // 빌드 인덱스로 씬 불러오기
  SceneManager.LoadScene(1); 
  ```
- **특징 및 시나리오**:
  - 즉시 씬을 전환하며, 로딩이 끝날 때까지 모든 것이 정지됩니다.
  - 로딩 시간이 매우 짧은 간단한 씬(메인 메뉴, 게임 오버 화면 등)으로 전환할 때 적합합니다.
  - **예시 (시작 버튼)**:
    ```csharp
    // MainMenu.cs
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class MainMenu : MonoBehaviour
    {
        public void OnStartButtonClick()
        {
            // "Level1"이라는 이름의 씬을 불러온다.
            SceneManager.LoadScene("Level1");
        }
    }
    ```

### **3. 비동기적 씬 로딩 (Asynchronous Loading)**
로딩 중에도 게임이 멈추지 않는, '로딩 화면' 구현에 필수적인 방법입니다.

- **문법 구조**:
  ```csharp
  // 코루틴 안에서 사용해야 함
  IEnumerator LoadSceneAsyncProcess(string sceneName)
  {
      AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

      // 씬 로딩이 완료될 때까지 대기
      while (!asyncLoad.isDone)
      {
          Debug.Log("Loading progress: " + asyncLoad.progress);
          yield return null;
      }
  }
  ```
- **특징 및 시나리오**:
  - `LoadSceneAsync`는 `AsyncOperation`이라는 객체를 반환하며, 이 객체를 통해 로딩 진행 상황을 확인할 수 있습니다.
  - 로딩이 백그라운드에서 진행되므로, 로딩 바(Progress Bar)를 보여주거나 애니메이션을 재생하는 등 사용자 경험을 향상시킬 수 있습니다.
  - **`asyncLoad.progress`**: 로딩 진행률을 0.0 ~ 1.0 사이의 값으로 반환합니다. (정확히는 0.9에서 멈춤)
  - **`asyncLoad.allowSceneActivation`**: 이 값을 `false`로 설정하면, 로딩이 99% 완료되어도(progress가 0.9) 씬이 자동으로 활성화되지 않습니다. "Press any key to continue" 같은 프롬프트를 구현할 때 유용합니다.
  - **예시 (로딩 바 구현)**:
    ```csharp
    // LoadingManager.cs
    public class LoadingManager : MonoBehaviour
    {
        public Slider loadingBar;

        void Start()
        {
            StartCoroutine(LoadLevelAsync("Level1"));
        }

        IEnumerator LoadLevelAsync(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false; // 로딩 후 바로 넘어가지 않게

            while (!asyncLoad.isDone)
            {
                // progress는 0.9에서 멈추므로, 0.9를 1로 변환해준다.
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                loadingBar.value = progress;

                // 로딩이 거의 완료되면
                if (asyncLoad.progress >= 0.9f)
                {
                    // "아무 키나 누르세요" 텍스트를 띄우고...
                    if (Input.anyKeyDown)
                    {
                        asyncLoad.allowSceneActivation = true; // 씬 활성화
                    }
                }
                yield return null;
            }
        }
    }
    ```

### **4. 씬 간 데이터 유지: `DontDestroyOnLoad`와 싱글톤**
기본적으로 씬이 전환되면 현재 씬의 모든 게임 오브젝트는 파괴됩니다. 하지만 게임 매니저, 사운드 매니저처럼 게임 내내 유지되어야 할 오브젝트도 있습니다.

- **`DontDestroyOnLoad(GameObject)`**:
  - 이 함수에 전달된 게임 오브젝트는 씬이 바뀌어도 파괴되지 않고 다음 씬으로 그대로 옮겨집니다.
- **싱글톤(Singleton) 패턴**:
  - `DontDestroyOnLoad`를 사용할 때 흔히 발생하는 문제는, 이전 씬으로 돌아왔을 때 관리자 오브젝트가 중복으로 생성되는 것입니다.
  - 싱글톤 패턴은 특정 클래스의 인스턴스가 **프로그램 내에 단 하나만 존재하도록 보장**하는 디자인 패턴입니다. `static` 변수를 사용해 자신의 유일한 인스턴스를 저장하고, 두 번째 인스턴스가 생성되려 하면 스스로를 파괴하도록 만들어 이 문제를 해결합니다.

- **예시 (싱글톤 게임 매니저)**:
  ```csharp
  // GameManager.cs
  public class GameManager : MonoBehaviour
  {
      // 1. 자기 자신 타입의 static 변수 선언 (유일한 인스턴스를 담을 변수)
      public static GameManager instance = null;

      public int score = 0;

      void Awake()
      {
          // 2. instance가 아직 비어있다면(최초의 인스턴스라면)
          if (instance == null)
          {
              // instance에 자기 자신을 할당
              instance = this;
              // 씬이 전환되어도 이 오브젝트를 파괴하지 말라고 명령
              DontDestroyOnLoad(this.gameObject);
          }
          // 3. instance에 이미 다른 GameManager가 할당되어 있다면(중복 생성되었다면)
          else
          {
              // 새로 생긴 자기 자신을 파괴하여 유일성을 보장
              Destroy(this.gameObject);
          }
      }
  }
  ```

---

## **주요 `SceneManager` 관련 기능 표**
| 기능 | 설명 | 대표 활용 시나리오 |
| :--- | :--- | :--- |
| **`SceneManager.LoadScene()`** | 동기적으로 씬을 로드합니다. 로딩 중 게임이 멈춥니다. | 간단한 메뉴, 게임 오버 씬으로 즉시 이동할 때 |
| **`SceneManager.LoadSceneAsync()`** | 비동기적으로 씬을 로드합니다. 로딩 중 게임이 멈추지 않습니다. | 로딩 화면을 보여주며 무거운 레벨 씬을 불러올 때 |
| **`SceneManager.GetActiveScene()`** | 현재 활성화된 `Scene` 객체를 반환합니다. (`.name`이나 `.buildIndex` 속성 접근 가능) | 현재 씬의 이름이나 빌드 인덱스를 확인하여 분기 처리할 때 |
| **`SceneManager.sceneCountInBuildSettings`**| 빌드 세팅에 등록된 씬의 총 개수를 반환합니다. | 마지막 레벨인지 확인하여 엔딩 씬으로 분기할 때 |
| **`DontDestroyOnLoad(Object)`** | `target` 오브젝트가 다음 씬으로 전환될 때 파괴되지 않도록 합니다. | 게임 전체의 상태를 관리하는 `GameManager`, `SoundManager` |
| **`AsyncOperation.progress`** | 비동기 로딩 진행률을 0.0 ~ 0.9 사이 값으로 반환합니다. | 로딩 바(Progress Bar)의 UI를 업데이트할 때 |
| **`AsyncOperation.allowSceneActivation`**| `true`로 설정하면 로딩 완료 시 씬을 자동 활성화합니다. | 로딩 완료 후 유저의 입력을 기다렸다가 씬을 전환하고 싶을 때 |

---

## **실습 문제**
1.  **[기초] 간단한 씬 전환**:
    - `MainMenu` 씬과 `Level1` 씬, 두 개를 만드세요.
    - `File > Build Settings`에 두 씬을 모두 등록하세요 (`MainMenu`가 0번 인덱스).
    - `MainMenu` 씬에 UI 버튼을 만들고, 이 버튼을 클릭하면 `Level1`으로 넘어가도록 `SceneManager.LoadScene("Level1");`을 호출하는 스크립트를 작성하여 연결하세요.

2.  **[기초] 현재 씬 재시작**:
    - `Level1` 씬에 'R' 키를 누르면 현재 씬이 처음부터 다시 시작되도록 만들어보세요.
    - 힌트: `SceneManager.GetActiveScene().buildIndex`를 사용하면 현재 씬의 빌드 인덱스를 얻을 수 있습니다.

3.  **[응용] 로딩 바 구현**:
    - `LoadingScene`이라는 씬을 새로 만드세요. 이 씬에는 `Slider` UI가 하나 있습니다.
    - `MainMenu`에서 `Start` 버튼을 누르면 `LoadingScene`으로 먼저 이동합니다.
    - `LoadingScene`이 시작되면, `LoadSceneAsync`를 사용해 `Level1`을 비동기적으로 로드하고, 로딩 진행률(`asyncOperation.progress`)에 따라 `Slider`의 `value`가 채워지도록 코루틴을 작성하세요.

4.  **[응용] 싱글톤으로 점수 유지하기**:
    - 위에서 배운 싱글톤 패턴을 사용해 `GameManager` 스크립트를 만드세요. `GameManager`는 `public int score;` 변수를 가집니다.
    - `MainMenu` 씬에서 '점수 100점 추가' 버튼을 만들어 `GameManager.instance.score += 100;`을 실행하게 하세요.
    - `Level1` 씬으로 전환한 뒤, `Level1`에 있는 다른 스크립트의 `Start` 함수에서 `Debug.Log(GameManager.instance.score);`를 호출하여 `MainMenu`에서 올린 점수가 그대로 유지되는지 확인해보세요.

??? success "정답 및 해설"

    **1번**:
    ```csharp
    using UnityEngine.SceneManagement;   // 필수!

    public class MenuController : MonoBehaviour
    {
        public void OnStartButtonClicked()   // Button의 OnClick()에 연결
        {
            SceneManager.LoadScene("Level1");
        }
    }
    ```
    Build Settings에 씬을 등록하지 않으면 `Scene 'Level1' couldn't be loaded` 에러가 납니다 —
    씬 전환 문제의 90%가 이 등록 누락입니다.

    **2번**:
    ```csharp
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    ```
    이름 대신 **빌드 인덱스**로 로드하면 어느 씬에 붙여도 동작하는 범용 재시작 코드가 됩니다.

    **3번**:
    ```csharp
    public Slider progressBar;

    void Start()
    {
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("Level1");

        while (!op.isDone)
        {
            // progress는 0.9에서 멈춤 — 0.9가 "로드 완료", 나머지 0.1은 씬 활성화 단계
            progressBar.value = Mathf.Clamp01(op.progress / 0.9f);
            yield return null;
        }
    }
    ```
    `op.progress / 0.9f`로 나눠야 로딩 바가 100%까지 차오릅니다. 그대로 쓰면 90%에서 멈춘 것처럼 보임.

    **4번**:
    ```csharp
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public int score;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);   // 씬이 바뀌어도 파괴되지 않음
            }
            else
            {
                Destroy(gameObject);   // 이미 존재하면 중복 생성 방지
            }
        }
    }
    ```
    점수가 유지되는 이유 두 가지: `DontDestroyOnLoad`로 **오브젝트가 살아남고**,
    `static instance`로 **어느 씬의 어떤 스크립트든 같은 GameManager에 접근**하기 때문입니다.


---

## **주의사항**
- **Build Settings는 필수입니다**: 아무리 강조해도 지나치지 않습니다. 스크립트로 씬을 불러오려면 반드시 `File > Build Settings`에 씬이 등록되어 있어야 합니다. 가장 흔하게 겪는 실수입니다.
- **`LoadSceneAsync`의 `progress`**: `progress` 값은 씬 데이터를 메모리에 올리는 과정만 포함하며, 0.9에서 멈추는 것처럼 보입니다. 나머지 0.1은 로드된 씬을 활성화(Awake, Start 함수 호출 등)하는 과정이며, 이 과정이 짧은 순간에 일어나 1.0으로 바뀝니다.
- **`DontDestroyOnLoad`의 함정**: 이 함수로 지정된 오브젝트는 루트(Root)에 위치하게 됩니다. 또한, 싱글톤 방어 코드가 없다면 씬을 되돌아올 때마다 오브젝트가 복제되어 심각한 버그를 유발할 수 있습니다.
- **씬 전환 시 이벤트 구독 해제**: `OnEnable`에서 구독한 이벤트를 `OnDisable`에서 해제하는 습관은 매우 중요합니다. 해제하지 않으면, 파괴된 오브젝트의 함수를 호출하려다 `MissingReferenceException` 오류가 발생할 수 있습니다.

---

## **더 알아보기**
- **`LoadSceneMode.Additive`**: 기존 씬을 그대로 둔 채, 그 위에 새로운 씬을 추가로 불러오는 방식입니다. UI 씬을 게임 씬 위에 항상 띄워두거나, 거대한 오픈 월드 맵을 여러 구역(씬)으로 나누어 플레이어 주변의 씬들만 동적으로 로드/언로드하는 스트리밍 기법에 사용됩니다.
- **Addressable Asset System**: Unity의 최신 에셋 관리 시스템입니다. 씬을 포함한 모든 게임 에셋을 '주소'를 통해 관리하며, 원격 서버에서 다운로드하거나 필요할 때만 로컬에서 불러올 수 있습니다. 이를 통해 앱 초기 빌드 용량을 획기적으로 줄이고, 앱 스토어 업데이트 없이 콘텐츠를 패치할 수 있습니다.
