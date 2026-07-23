# **Physics**
## **유니티 물리 시스템의 핵심 클래스**
### **1) Rigidbody**
- 물리 기반의 움직임을 처리하는 가장 중요한 컴포넌트
- 중력 적용, 힘 적용, 충돌 감지 등의 역할 수행

### **📌 주요 프로퍼티**
| 프로퍼티 | 설명 |
| --- | --- |
| `mass` | 물체의 질량을 설정 |
| `drag` | 공기 저항 (선속도 감속) |
| `angularDrag` | 회전 저항 (각속도 감속) |
| `useGravity` | 중력 적용 여부 |
| `isKinematic` | `true`일 경우, 물리 엔진의 영향을 받지 않음 |
| `constraints` | 위치 및 회전 축을 고정 가능 |

### **📌 주요 함수**
1. `AddForce(Vector3 force, ForceMode mode = ForceMode.Force)` : 힘을 가하여 움직임을 유도
2. `AddTorque(Vector3 torque, ForceMode mode = ForceMode.Force)` : 회전력을 가하여 회전 유도
3. `MovePosition(Vector3 position)` : 물체를 특정 위치로 이동 (Kinematic 사용)
4. `MoveRotation(Quaternion rotation)` : 물체를 특정 회전으로 변경 (Kinematic 사용)
5. `Sleep()` : Rigidbody의 움직임을 멈춤 (비활성화)

### **PhysicsMaterial**
- 충돌 시 물리적 특성을 결정하는 재질

### **📌 주요 프로퍼티**
| 프로퍼티 | 설명 |
| --- | --- |
| `dynamicFriction` | 이동 중 마찰 계수 |
| `staticFriction` | 정지 시 마찰 계수 |
| `bounciness` | 충돌 반발력 |
| `frictionCombine` | 마찰 적용 방식 (`Average`, `Multiply`, `Minimum`, `Maximum`) |

Physics 

## **기본적인 충돌 감지 함수**
유니티에서 충돌 감지는 `Raycast`, `SphereCast`, `BoxCast` 등의 **캐스팅 함수**를 사용하여 수행됩니다.

### **🔹 Raycast (광선 충돌 감지)**
```csharp
bool Physics.Raycast(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

```

- 특정 방향으로 **광선(Ray)** 을 쏴서 충돌 여부를 확인
- `out RaycastHit hit`을 통해 충돌 정보 저장
- **레이어 마스크(layerMask)** 를 활용하여 특정 레이어와만 충돌 가능

**📌 사용 예시**

```csharp
RaycastHit hit;
if (Physics.Raycast(transform.position, transform.forward, out hit, 100f, LayerMask.GetMask("Enemy")))
{
    Debug.Log("충돌한 오브젝트: " + hit.collider.name);
}

```

---

### **🔹 SphereCast (구 형태의 충돌 감지)**
```csharp
bool Physics.SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hit, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

```

- `Raycast`와 유사하지만, **광선이 아닌 구(Sphere)** 를 쏴서 충돌 감지

**📌 사용 예시**

```csharp
if (Physics.SphereCast(transform.position, 1f, transform.forward, out hit, 50f))
{
    Debug.Log("구 형태로 감지된 오브젝트: " + hit.collider.name);
}

```

---

### **🔹 BoxCast (박스 형태의 충돌 감지)**
```csharp
bool Physics.BoxCast(Vector3 center, Vector3 halfExtents, Vector3 direction, out RaycastHit hit, Quaternion orientation, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

```

- 박스 모양으로 충돌 감지 (예: 벽을 통과하는 플레이어 감지)

**📌 사용 예시**

```csharp
if (Physics.BoxCast(transform.position, Vector3.one * 0.5f, transform.forward, out hit, Quaternion.identity, 10f))
{
    Debug.Log("박스 캐스트 충돌: " + hit.collider.name);
}

```

---

### **🔹 CapsuleCast (캡슐 형태의 충돌 감지)**
```csharp
bool Physics.CapsuleCast(Vector3 point1, Vector3 point2, float radius, Vector3 direction, out RaycastHit hit, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

```

- **CapsuleCollider** 형태로 충돌 감지 (플레이어 감지 등에 활용)

---

## **2. 영역 감지 (Overlap)**
- `Overlap` 함수들은 특정 위치에서 충돌 가능한 오브젝트들을 검색하는 함수

### **🔹 OverlapSphere (구 영역 내 모든 충돌 오브젝트 검색)**
```csharp
Collider[] Physics.OverlapSphere(Vector3 position, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

```

- **지정한 반경 내에 있는 모든 콜라이더 검색**
- NPC, 적 AI 감지 등에 사용

**📌 사용 예시**

```csharp
Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f, LayerMask.GetMask("Enemy"));
foreach (var collider in hitColliders)
{
    Debug.Log("감지된 적: " + collider.name);
}

```

---

### **🔹 OverlapBox (박스 영역 내 모든 충돌 오브젝트 검색)**
```csharp
Collider[] Physics.OverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

```

- 박스 형태로 충돌 감지 (건물 내부 등 특정 영역 내 객체 찾기)

---

### **🔹 OverlapCapsule (캡슐 영역 내 모든 충돌 오브젝트 검색)**
```csharp
Collider[] Physics.OverlapCapsule(Vector3 point1, Vector3 point2, float radius, int layerMask, QueryTriggerInteraction queryTriggerInteraction);

```

- 특정 높이와 반지름을 가진 **캡슐 형태의 감지 영역**을 사용

---

## **3. Rigidbody 관련 물리 연산**
### **🔹 ComputePenetration (두 콜라이더의 침투 깊이 계산)**
```csharp
bool Physics.ComputePenetration(Collider colliderA, Vector3 positionA, Quaternion rotationA, Collider colliderB, Vector3 positionB, Quaternion rotationB, out Vector3 direction, out float distance);

```

- **두 개의 콜라이더가 겹칠 경우**, 겹치는 정도(침투 깊이)와 방향을 계산

---

### **🔹 ClosestPoint (특정 지점에서 가장 가까운 콜라이더 위치 찾기)**
```csharp
Vector3 Physics.ClosestPoint(Vector3 point, Collider collider, Vector3 position, Quaternion rotation);

```

- **지정한 지점에서 특정 콜라이더와의 가장 가까운 점을 반환**
- AI 경로 계산 등에 활용

---

## **4. 충돌 무시 및 Layer 관리**
### **🔹 IgnoreCollision (두 개의 콜라이더 간 충돌 무시)**
```csharp
void Physics.IgnoreCollision(Collider collider1, Collider collider2, bool ignore);

```

- 특정 두 오브젝트 간 충돌을 무시할 수 있음

**📌 사용 예시**

```csharp
Physics.IgnoreCollision(playerCollider, enemyCollider, true);

```

---

### **🔹 IgnoreLayerCollision (특정 레이어 간 충돌 무시)**
```csharp
void Physics.IgnoreLayerCollision(int layer1, int layer2, bool ignore);

```

- 게임 내 특정 레이어 간의 충돌을 비활성화 (예: 플레이어와 트리 충돌 비활성화)

**📌 사용 예시**

```csharp
Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Tree"), true);

```

---

## **5. 물리 시뮬레이션 설정 및 시간 제어**
### **🔹 Simulate (물리 연산 수동 실행)**
```csharp
void Physics.Simulate(float deltaTime);

```

- `Auto Simulation`이 비활성화된 경우 **물리 시뮬레이션을 수동으로 업데이트**
- `FixedUpdate()` 대신 직접 호출할 수 있음

---

### **🔹 AutoSimulation 설정**
```csharp
Physics.autoSimulation = false;  // 물리 연산을 수동으로 실행하도록 설정

```

---

## **6. 유니티 Physics 엔진 관련 설정**
### **🔹 Gravity (중력 설정)**
```csharp
Physics.gravity = new Vector3(0, -9.81f, 0);

```

- 전역적으로 물리 엔진의 중력 값을 설정 가능

---

### **🔹 QueriesHitTriggers 설정**
```csharp
Physics.queriesHitTriggers = true;

```

- `true`: `Raycast`, `Overlap` 함수가 `isTrigger`가 활성화된 오브젝트도 감지
- `false`: `isTrigger`가 활성화된 오브젝트 무시

---

!!! note "🔗 함께 보기"
    - **C# 기초 문법**: [11. 참조 (Reference)](../../C%23 Programming/Part 1 - 기초 문법/11_c%23_reference.md) — `Physics.Raycast(..., out RaycastHit hit)`의 `out` 파라미터로 결과를 돌려받는 방식을 다룹니다 (C# 챕터의 `TryParse(out ...)`와 같은 패턴).

## **📌 정리 및 결론**
유니티의 `Physics` 클래스는 단순한 충돌 감지에서부터 물리 시뮬레이션 제어, 최적화까지 다양한 기능을 제공합니다.

**게임 개발에서 효율적으로 활용하려면**:

1. **Raycast, Overlap, Cast 함수를 적절히 선택하여 사용**
2. **필요한 경우 `IgnoreCollision`, `IgnoreLayerCollision`으로 최적화**
3. **물리 연산을 최적화하기 위해 `autoSimulation`과 `Simulate()` 활용**
4. **충돌을 감지할 때 Layer Mask를 사용하여 불필요한 연산 최소화**