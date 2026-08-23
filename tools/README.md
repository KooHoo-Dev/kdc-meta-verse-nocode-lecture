# tools — 강의 준비용 (수강생 배포 대상 아님)

이 폴더는 **강사가 Unity 에디터에서 작업할 때 쓰는 재료**입니다.
`Assets/` 밖에 있으므로 `.unitypackage` 에 포함되지 않습니다.

---

## `korean_charset_2350.txt`

TMP 한글 폰트 에셋을 만들 때 **Character Set → Custom Characters** 에 넣는 문자 목록입니다.

| 구성 | 개수 |
| :--- | ---: |
| KS X 1001 완성형 한글 | 2350 |
| ASCII 출력 가능 문자 | 95 |
| 자주 쓰는 기호 (`·—…“”→★○□△` 등) | 25 |
| **합계** | **2470** |

### 왜 2350자인가

한글 음절은 전부 합치면 11172자입니다. 그걸 다 구우면 아틀라스가 4096×4096 여러 장이 되어 실용적이지 않습니다.
**KS X 1001 완성형 2350자**는 현대 한국어 문서를 사실상 다 덮으면서 아틀라스 한 장에 들어갑니다.

---

## 폰트 에셋 만드는 절차 (U-2)

> ⚠️ **12차시 강의자료(`.unitypackage`)에 들어가야 합니다.**
>
> **14차시 실습 ④에서 버튼 글자를 한글("멈춤")로 씁니다.** 폰트가 없으면 그때부터 네모(☐)가 나옵니다.
> 원래 18차시 마감이었지만 **버튼이 14차시로 앞당겨지면서 마감도 앞당겨졌습니다.**

1. 자유 라이선스 한글 폰트(`.ttf`)를 `Assets/_NoCodeKit/Fonts/` 에 넣습니다.
   - **재배포 가능한 라이선스인지 반드시 확인.** 수강생에게 `.unitypackage` 로 나갑니다.
   - 본고딕(Noto Sans KR) 계열 권장 — SIL OFL 이라 재배포에 문제가 없습니다.
2. `Window` → `TextMeshPro` → `Font Asset Creator`
3. 설정:

    | 항목 | 값 |
    | :--- | :--- |
    | Source Font File | 1번에서 넣은 `.ttf` |
    | Sampling Point Size | `Auto Sizing` |
    | Padding | `5` |
    | Packing Method | `Fast` (최종본은 `Optimum`) |
    | Atlas Resolution | `2048 × 2048` |
    | Character Set | **`Custom Characters`** |
    | Custom Character List | `korean_charset_2350.txt` 내용을 **전부 복사해 붙여넣기** |
    | Render Mode | `SDFAA` |
    | **Atlas Population Mode** | **`Dynamic`** ← 아래 참고 |

4. `Generate Font Atlas` → `Save as...` → `Assets/_NoCodeKit/Fonts/` 에 저장
5. `Project Settings` → `TextMesh Pro` → **`Default Font Asset`** 을 방금 만든 것으로 지정

> 📝 **왜 Dynamic 인가**
>
> `Static` 으로 구우면 목록에 없는 글자는 **네모(☐)로 나옵니다.**
> 노코드 수강생이 버튼에 아무 글자나 입력하다가 네모를 보면 **"내가 뭘 잘못했다"고 느낍니다.**
>
> `Dynamic` 은 목록에 없는 글자를 **실행 중에 알아서 추가**합니다.
> 2350자를 미리 구워두는 것은 **자주 쓰는 글자를 빠르게 처리하기 위한 것**이고,
> Dynamic 은 **그 밖의 글자에 대한 안전망**입니다. 둘을 같이 씁니다.

### 확인 방법

씬에 `TextMeshPro - Text` 를 하나 놓고 아래를 입력해 **네모가 없는지** 봅니다.

```
한글 잘 나옵니까? 12345 ABC 점수: 0 / 시작 · 재시작 → ★
```

---

## 남은 준비물 (U-3 이후)

`plans/02_유니티_프로젝트_작업계획.md` 참고.

- [ ] `Assets/_NoCodeKit/` 를 `.unitypackage` 로 내보내기 (v1: 컴포넌트 2개 + 폰트)
- [ ] 13 · 14차시용 씬 2개
- [ ] 차시별 완성 상태 씬 (진도 복구용)
