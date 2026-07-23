# **XR Interaction**
XR 상호작용은 Hover, Select, Activate 세 가지 상태로 이루어짐.

| 상태           | 설명                                                                                            |
| ------------ |-----------------------------------------------------------------------------------------------|
| **Hover**    | Interactor(손이나 레이저)가 Interactable 대상 위에 있을 때 진입.전통적인 GUI의 마우스 오버와 유사하며, 시각적 강조 효과를 줄 수 있음.    |
| **Select**   | 사용자 입력(버튼/트리거)을 눌러 대상을 선택할 때.잡기, 레버 조작, 문 열기 준비 등 실제 인터랙션 표시.                                 |
| **Activate** | Select 상태 이후 추가 입력을 통해 행동 실행.플래시라이트 켜기/끄기, 총 발사 등 **상황별 행동** 지정 가능. UnityEvent로 코드 없이 바인딩 가능. |

---

## **1. Interactors, Interactables, Interaction Manager**
| 구성 요소                   | 역할                                       |
| ----------------------- | ---------------------------------------- |
| **Interactor**          | 사용자(손, 컨트롤러 등)의 입력/위치 정보를 처리하여 상호작용 시도   |
| **Interactable**        | 상호작용 대상(오브젝트, 버튼, UI 등)                  |
| **Interaction Manager** | 둘 사이의 상호작용을 조정하고 상태(Hover, Select 등)를 관리 |

### **Interactor**
사용자의 입력/행동을 감지하고 상호작용을 시도하는 주체

| 유형                     | 설명                                |
| ---------------------- | --------------------------------- |
| `XR Ray Interactor`    | 레이저 포인터 기반 상호작용 (UI, 멀리 있는 오브젝트)  |
| `XR Direct Interactor` | 손이나 컨트롤러가 실제로 접촉한 오브젝트를 대상으로 인터랙션 |
| `XR Socket Interactor` | 특정 오브젝트만 수용 가능한 소켓 역할             |
| `XR Simple Interactor` | 수동 상호작용 구현 시 사용 (코드에서 직접 처리)      |

#### **주요 속성**
- Interaction Layer Mask: 어떤 오브젝트와 상호작용할지 필터링
- Select / Activate 이벤트: 입력 신호 감지 및 상호작용 트리거
- Attach Transform: 잡을 때 대상이 붙는 위치

### **Interactable**
사용자가 상호작용할 수 있는 오브젝트

| 유형                       | 설명                           |
| ------------------------ | ---------------------------- |
| `XR Grab Interactable`   | 오브젝트를 잡고, 던지고, 위치 변경         |
| `XR Socket Interactable` | 특정 오브젝트를 받아들이는 대상 (예: 무기 슬롯) |
| `XR Simple Interactable` | 커스텀 인터랙션 (버튼 등)              |
| `XR Base Interactable`   | 모든 상호작용형 오브젝트의 추상 클래스        |

#### **주요 속성**
- Interaction Layer Mask: 어떤 Interactor와 반응할지 필터링
- Rigidbody 필수: Grab 가능하려면 Rigidbody 존재해야 함
- Select Mode: Single / Multiple 선택 가능 여부 설정
- Events
    - OnHoverEntered, OnHoverExited
    - OnSelectEntered, OnSelectExited
    - OnActivated, OnDeactivated

### **Interaction Manager**
Interactor와 Interactable 간의 상호작용 중재자

#### **역할**
- 모든 상호작용 상태(Hover, Select, Activate)를 프레임마다 관리
- 어떤 Interactor가 어떤 Interactable과 상호작용 중인지 추적
- 상태 변경 시 이벤트 호출 및 데이터 정리

#### **사용 방법**
- 일반적으로 씬에 하나만 존재
- 자동으로 Interactor/Interactable을 인식하여 관리
- 여러 Manager를 나눠서 특정 그룹만 분리 관리할 수도 있음

---

## **2. 상호작용의 동작 방식**
```
[XR Controller] Input (Trigger, Grip 등)
      ↓ 
[Interactor] 후보 판단 (Ray Hit, Collider 접촉 등)
      ↓ 
    [Interaction Manager]
      ↙                ↘
[Enter/Exit Hover]   [Enter/Exit Select]
      ↘                ↙
        [Interactable]
      ↓ 반응 (이벤트, 상태변경)
[오브젝트 잡기 / 동작 / UI 클릭 등]
```

---

## **3. Interactor ↔ Interactable**
1. 1:1 : 한 Interactor가 하나의 Interactable만 선택
2. 1\:N : `Multiple Selection` 허용 시 가능 (ex: 다중 UI 선택)
3. 우선순위 : Interactor는 거리, 방향 등 기준으로 가장 가까운 대상 우선 처리
4. 충돌 기반 : Direct Interactor는 Collider 접촉을 통해 후보군 인식
5. 레이 기반 : Ray Interactor는 Raycast로 후보를 인식
6. Layer 기반 필터 : Layer Mask로 어떤 상호작용을 허용할지 제한 가능

