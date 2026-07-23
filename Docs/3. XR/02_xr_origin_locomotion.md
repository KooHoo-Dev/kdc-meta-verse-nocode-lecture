# **XR Origin**
## **개요**
XR Origin은 Unity의 XR Interaction Toolkit에서 사용자의 **머리(HMD)** 와 **손(컨트롤러)** 의 위치를 월드 좌표계에 정확히 매핑하기 위한 **기준 좌표 체계(Rig)** .
XR 콘텐츠 제작 시, 플레이어의 기준이 되는 **"카메라 리그(Center Eye + 컨트롤러 구조)"** 를 의미.

> Camera Rig, XR Rig 등으로도 불림

- XR 장비(HMD + 컨트롤러)의 추적 위치를 Unity 씬 공간에 배치
- 이동, 회전 시 사용자 전체 좌표계를 함께 이동시킬 수 있도록 함
- 좌우 손, 머리의 상대 위치 보정을 자동화
- Unity 월드 공간에서 플레이어 기준 좌표계(Player Space) 역할 수행

## **구성 요소**
```
XR Origin (GameObject)
├── Camera Offset (Transform) ← 사용자 기준점 (Origin)
│   ├── Main Camera ← HMD (머리 추적)
│   ├── LeftHand Controller ← 왼손 컨트롤러
│   └── RightHand Controller ← 오른손 컨트롤러
```

| 구성 요소                    | 설명                                  |
| ------------------------ | ----------------------------------- |
| **XR Origin (root)**     | 플레이어 전체 Rig의 기준 오브젝트                |
| **Camera Offset**        | 사용자 눈높이 기준 위치 조정                    |
| **Main Camera**          | HMD에 해당. 실제 XR 콘텐츠에서 보는 시점          |
| **LeftHand Controller**  | 왼손 컨트롤러 (Ray Interactor, Grab 등 부착) |
| **RightHand Controller** | 오른손 컨트롤러 (위와 동일)                    |

### **Tracking Origin Mode**
- Device: 디바이스가 기준 (앉은 상태 기준)
- Floor: 바닥이 기준 (서 있는 상태 기준, Room-scale)
> 일반적으로 VR 게임에서는 Floor 모드가 추천.


## **XR Origin의 주요 기능**
1. HMD & Controller 추적
- Main Camera는 HMD의 위치와 회전을 자동으로 반영
- Left/Right Hand GameObject는 컨트롤러 좌표를 추적
- Input System과 연결 시, 실제 컨트롤러 데이터를 실시간으로 반영

2. 사용자 위치/방향 제어
- XR Origin 오브젝트를 이동시키면 사용자 전체 위치를 이동시킴
- Camera Offset은 머리 기준의 Offset 조정 (앉은 높이, 바닥 기준 보정 등)

3. 텔레포트 이동 (MoveCameraToWorldLocation)
- 실제 플레이어의 위치를 특정 위치로 순간이동시킬 때 사용
- 텔레포트 시스템과 연계 가능
```csharp
xrOrigin.MoveCameraToWorldLocation(targetPosition);
```

4. 손/레이 인터랙션 기준점 제공
- 손 오브젝트(Interactor)가 위치할 기준을 제공
- XR Controller (Action-based) 구성 시, 이 손 오브젝트에 부착됨

## **XR Origin에서 좌표계 개념**
| 좌표 기준                    | 의미                                          |
| ------------------------ | ------------------------------------------- |
| World Space              | Unity의 전역 공간                                |
| Player Space (XR Origin) | 사용자 기준 좌표계. XR Origin이 움직이면 사용자가 이동한 것처럼 보임 |
| Device Space             | 실제 XR 기기의 좌표계. SDK에서 직접 접근하지 않음             |

## **팁 및 흔한 실수**
- XR Origin을 이동시키면 플레이어가 이동한 것으로 간주됨.
- Camera Offset은 바닥 보정용 → 바닥 기준 위치 조절
- 플레이어 회전 시, XR Origin을 Y축 회전시키면 됨
- 카메라가 바닥에 파묻히는 문제: Tracking Origin Mode가 Device인 경우, 바닥 기준이 맞지 않아 발생
- 컨트롤러가 보이지 않음: XR Controller에 Action 설정이 빠졌거나 Input Action Asset이 연결되지 않은 경우
- 손 위치가 어긋남: Camera Offset이 너무 크거나, XR Rig 전체 위치가 잘못된 경우

---

---

# **XR Locomotion System**
## **개요**
Unity의 XR Interaction Toolkit에서 제공하는 Locomotion 시스템은 사용자(플레이어)가 XR 공간 내에서 자연스럽게 
이동할 수 있도록 지원하는 기능. 이 시스템은 텔레포트(Teleportation), 연속 이동(Continuous Movement), 
회전(Continuous/ Snap Turn) 등을 포함.

| 이동 방식                           | 설명                     |
| ------------------------------- | ---------------------- |
| **Teleportation**               | 특정 지점으로 순간이동           |
| **Continuous Move**             | 아날로그 스틱 기반 부드러운 이동     |
| **Continuous Turn / Snap Turn** | 방향 전환: 연속 회전 or 계단식 회전 |

## **구조**
```
XR Origin
├── Locomotion System (필수)
│ ├── Teleportation Provider (선택)
│ ├── Continuous Move Provider (선택)
│ └── Continuous Turn Provider / Snap Turn Provider (선택)   

```
> LocomotionSystem은 XR Origin에 단 1개만 존재해야 함

## **주요 기능**
### **Teleportation System**
순간이동 방식, 사용자는 레이 포인터를 쏴서 목적지를 정한 뒤 지정 위치로 즉시 이동.

#### **구성**
| 컴포넌트                     | 설명                             |
| ------------------------ | ------------------------------ |
| `Teleportation Provider` | 이동 요청을 받아 XR Origin을 해당 위치로 이동 |
| `Teleportation Area`     | 바닥 등에 배치되어 해당 영역에 텔레포트 가능      |
| `Teleportation Anchor`   | 특정 지점으로 고정된 이동만 허용             |
| `XR Ray Interactor`      | 컨트롤러의 포인터 역할                   |
| `XR Controller`          | 입력 이벤트 수신 (ex. Select pressed) |

#### **동작 방식**
1. 사용자가 Select 버튼 누름
2. XR Ray Interactor가 활성화됨
3. Ray가 Teleportation Area/Anchor와 충돌
4. 버튼을 떼면 지정된 위치로 이동 (TeleportRequest 생성)
5. Teleportation Provider가 요청을 받아 XR Origin 이동


### **Continuous Move Provider**
스틱 이동 기반 부드러운 이동

| 컴포넌트                                      | 설명                                      |
| ----------------------------------------- | --------------------------------------- |
| `Continuous Move Provider (Action-based)` | 아날로그 입력을 기반으로 이동                        |
| `XR Controller (Action-based)`            | 이동 입력 액션을 읽음                            |
| Input Action Asset                        | `Move` Input Action (Vector2 타입) 바인딩 필요 |

1. `Move Speed` : 이동 속도
2. `Gravity Application Mode` : 중력 적용 방식 (즉시, 누적 등)
3. `Forward Source` : 이동 기준 방향 (HMD, 컨트롤러 등 선택 가능)

#### **동작 방식**
1. Input System의 Vector2 값을 통해 입력 방향 계산
2. XR Origin을 해당 방향으로 매 프레임 Translate
3. 이동 속도, 중력, 지면 검출 여부 등을 커스터마이징 가능

### **Continuous Turn / Snap Turn**
사용자 좌표계(XR Origin)의 Y축 회전을 제어. 회전 기준은 일반적으로 HMD 기준.

#### **Continuous Turn (Provider)**
- 부드러운 Y축 회전 (회전 속도 지정 가능)
- Input Action의 Vector2 입력값을 사용 (Turn Input Action)

#### **Snap Turn Provider**
- 일정 각도로 빠르게 회전 (ex. 45도)
- 멀미를 줄이기 위한 방식
- 바로 방향 전환됨