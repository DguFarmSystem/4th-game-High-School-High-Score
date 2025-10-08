# 야바위 게임 (Restaurant Find Stage)

## 📋 개요
음식이 담긴 접시를 찾는 야바위 게임입니다. Wanted 포스터에 표시된 음식이 들어있는 접시를 찾으면 클리어!

## 🎮 게임 플로우

1. **게임 시작**: 카운트다운 아직 시작 안 함
2. **음식 공개**: 3개의 접시에 음식이 보임 (뚜껑 열림)
   - 4가지 음식(에그플레이트, 오므라이스, 파르페, 스시) 중 랜덤으로 2가지 선택
   - Wanted 포스터에 찾아야 할 음식 표시
3. **뚜껑 닫기**: 모든 접시의 뚜껑이 닫힘
4. **접시 섞기**: 야바위 게임처럼 접시들이 섞임 (애니메이션)
5. **타이머 시작**: 섞기가 끝나면 카운트다운 시작
6. **플레이어 선택**:
   - 터치로 접시 선택
   - 선택된 접시는 `Find_Plate_Selected` 스프라이트로 변경 (0.7초)
7. **뚜껑 열기**: 선택한 접시의 뚜껑이 열림
8. **결과 판정**:
   - ✅ Wanted 음식과 동일 → **클리어 성공**
   - ❌ 다른 음식 → **클리어 실패**

## 📂 파일 구조

```
Assets/Scripts/Stage/Stages/RestaurantFindStage/
├── RestaurantFindStage.cs    # 메인 게임 로직
├── PlateController.cs         # 개별 접시 관리
└── README.md                  # 이 문서
```

## 🎨 필요한 스프라이트

### 접시 스프라이트
- `Find_Plate_Default` - 닫힌 접시 (기본)
- `Find_Plate_Selected` - 선택된 접시 (테두리 강조)
- `Find_Plate_Eggplate` - 에그플레이트가 보이는 접시
- `Find_Plate_Domelid` - 도멜리드가 보이는 접시 (선택 사항)
- `Find_Plate_Omurice` - 오므라이스가 보이는 접시
- `Find_Plate_Parfait` - 파르페가 보이는 접시
- `Find_Plate_Sushi` - 스시가 보이는 접시

### 음식 스프라이트 (Wanted 포스터용)
- `Find_Wanted_EggPlate` - 에그플레이트
- `Find_Wanted_Omurice` - 오므라이스
- `Find_Wanted_Parfait` - 파르페
- `Find_Wanted_Sushi` - 스시

## 🔧 Unity 설정 방법

### 1. 씬 설정

1. 빈 GameObject 생성: `RestaurantFindStage`
2. `RestaurantFindStage.cs` 스크립트 추가

### 2. 접시 오브젝트 생성 (3개)

각 접시마다:

```
Plate1 (GameObject)
├── PlateSprite (SpriteRenderer) - 접시 이미지
├── FoodSprite (SpriteRenderer) - 음식 이미지 (처음엔 비활성화)
└── Collider2D - 터치 감지용 (BoxCollider2D 또는 CircleCollider2D)
```

**중요**: 각 접시에 `PlateController.cs` 스크립트 추가

### 3. RestaurantFindStage Inspector 설정

#### Game Settings
- **Plate Count**: 3 (접시 개수)
- **Timer Time**: 제한 시간 (초) - StageNormal에서 상속

#### Plate References
- **Plates**: PlateController 배열 (3개)
  - Plate1, Plate2, Plate3 할당

#### Food Sprites
- **Egg Plate Sprite**: Find_Wanted_EggPlate 할당
- **Omurice Sprite**: Find_Wanted_Omurice 할당
- **Parfait Sprite**: Find_Wanted_Parfait 할당
- **Sushi Sprite**: Find_Wanted_Sushi 할당

#### UI References
- **Wanted Poster Food**: Wanted 포스터의 SpriteRenderer 할당

#### Animation Settings
- **Show Food Duration**: 음식 보여주는 시간 (기본: 2초)
- **Lid Close Duration**: 뚜껑 닫는/여는 시간 (기본: 1초)
- **Shuffle Animator**: 섞기 애니메이션 Animator 할당
- **Shuffle Duration**: 섞는 시간 (기본: 3초)

#### Test Code (디버깅용)
- **Green Sphere**: 클리어 성공 시 표시할 오브젝트
- **Red Sphere**: 클리어 실패 시 표시할 오브젝트

### 4. PlateController Inspector 설정

각 접시마다:

#### Sprite References
- **Plate Renderer**: 접시 SpriteRenderer 할당
- **Food Renderer**: 음식 SpriteRenderer 할당

#### Plate Sprites
- **Default Plate Sprite**: Find_Plate_Default 할당
- **Selected Plate Sprite**: Find_Plate_Selected 할당

## 🎬 애니메이션 설정 (섞기)

### 방법 1: Animator 사용

1. 3개의 접시를 부모 오브젝트로 묶기 (예: `PlatesContainer`)
2. Animator 컴포넌트 추가
3. Animation Clip 생성: `Shuffle`
4. 애니메이션에서 각 접시의 Transform Position을 키프레임으로 설정
   - 접시1과 접시2 위치 바꾸기
   - 접시2와 접시3 위치 바꾸기
   - 여러 번 반복
5. Animator Controller에 Trigger 파라미터 생성: `Shuffle`
6. Idle → Shuffle 트랜지션 설정
7. RestaurantFindStage의 `Shuffle Animator`에 Animator 할당

### 방법 2: 스크립트로 직접 구현

애니메이터 없이 코드로 섞기를 구현하려면 `RestaurantFindStage.cs`의 `ShufflePlates()` 메서드를 수정하세요.

## 💡 주요 클래스 설명

### RestaurantFindStage.cs

#### 주요 메서드
- `GameSequence()`: 전체 게임 흐름 관리 코루틴
- `ShowFood()`: 4가지 음식 중 랜덤으로 2가지 선택하여 보여주기
- `CloseLids()`: 모든 접시 뚜껑 닫기
- `ShufflePlates()`: 섞기 애니메이션 실행
- `OnPlateSelected()`: 플레이어가 접시 선택 시 호출됨 (PlateController에서 호출)
- `HandlePlateSelection()`: 선택 처리 및 결과 판정
- `GetRandomTwoFoods()`: 4가지 중 랜덤으로 2가지 음식 선택

#### 게임 상태 (GameState)
```csharp
ShowingFood       // 음식 보여주는 중
ClosingLid        // 뚜껑 닫는 중
Shuffling         // 섞는 중
WaitingSelection  // 선택 대기 중
Revealing         // 결과 공개 중
Ended             // 게임 종료
```

### PlateController.cs

#### 주요 메서드
- `SetFood()`: 음식 설정 및 정답 여부 설정
- `ShowFood()`: 음식 보여주기 (뚜껑 열림 상태)
- `CloseLid()`: 뚜껑 닫기 (음식 숨김)
- `OpenLid()`: 뚜껑 열기 (음식 보임)
- `ShowSelected()`: 선택 상태 표시 (Selected 스프라이트)
- `OnMouseDown()`: 터치/클릭 감지

#### 스프라이트 자동 로드
`SetOpenPlateSprite()` 메서드는 음식 이름에 따라 자동으로 적절한 접시 스프라이트를 찾습니다.
- 예: `Wanted_EggPlate` → `Find_Plate_Eggplate`

## � 게임 시퀀스 타이밍

```
[게임 시작]
    ↓
[음식 보여주기] ← 2초
 - 4가지 중 2가지 랜덤 선택
 - Wanted 포스터 표시
    ↓
[뚜껑 닫기] ← 1초
    ↓
[접시 섞기] ← 3초 (애니메이션)
    ↓
[타이머 시작] → 플레이어 선택 가능
    ↓
[접시 선택]
    ↓
[선택 표시] ← 0.7초
    ↓
[뚜껑 열기] ← 1초
    ↓
[결과 판정]
 - 정답: 클리어
 - 오답: 실패
```

## 🐛 디버깅 팁

### 1. 접시를 클릭해도 반응 없음
- [ ] 각 접시에 `Collider2D`가 있는지 확인
- [ ] `PlateController.cs` 스크립트가 추가되었는지 확인
- [ ] `RestaurantFindStage`가 씬에 있는지 확인
- [ ] 카메라에 `Physics2DRaycaster` 컴포넌트가 있는지 확인 (UI용)

### 2. 음식이 제대로 표시되지 않음
- [ ] Food Sprites (4가지)가 모두 할당되었는지 확인
- [ ] PlateRenderer와 FoodRenderer가 연결되었는지 확인
- [ ] FoodRenderer의 Sorting Layer가 PlateRenderer보다 위에 있는지 확인

### 3. 섞기 애니메이션이 실행 안 됨
- [ ] Shuffle Animator가 할당되었는지 확인
- [ ] Animator에 "Shuffle" Trigger 파라미터가 있는지 확인
- [ ] Animation Clip이 제대로 설정되었는지 확인

### 4. 타이머가 섞기 중에 시작됨
- [ ] `OnStageStart()`에서 타이머를 시작하지 않도록 오버라이드됨
- [ ] `GameSequence()`에서 섞기 완료 후 `StartGameTimer()` 호출 확인

### 5. Wanted 포스터에 음식이 안 보임
- [ ] `wantedPosterFood` SpriteRenderer가 할당되었는지 확인
- [ ] SpriteRenderer가 활성화되어 있는지 확인

## 📝 Unity 설정 체크리스트

- [ ] RestaurantFindStage 오브젝트 생성 및 스크립트 추가
- [ ] 접시 3개 생성 (각각 PlateController 추가)
- [ ] 각 접시에 Collider2D 추가
- [ ] 4가지 음식 스프라이트 할당
- [ ] 접시 스프라이트 (Default, Selected) 할당
- [ ] Wanted 포스터 UI 설정
- [ ] 섞기 애니메이션 생성 및 Animator 할당
- [ ] 테스트 실행 및 디버깅

## 🎯 향후 개선 사항

- [ ] 사운드 효과 추가
  - 뚜껑 여는 소리
  - 섞는 소리
  - 선택 소리
  - 정답/오답 사운드
- [ ] 파티클 이펙트
  - 정답 시 반짝이는 효과
  - 오답 시 실패 효과
- [ ] 난이도 조절
  - 섞는 속도 증가
  - 접시 개수 증가 (4개, 5개)
- [ ] 더 복잡한 섞기 패턴

## 📞 문의

문제가 발생하거나 질문이 있으면 개발팀에 문의해주세요!


각 그릇마다 다음 구조로 생성:

```
DishObject (GameObject)
├── DishSprite (SpriteRenderer) - 그릇 이미지
├── FoodSprite (SpriteRenderer) - 음식 이미지
├── LidSprite (SpriteRenderer) - 뚜껑 이미지
├── BorderSprite (SpriteRenderer, Optional) - 선택 테두리
└── Collider2D - 클릭/터치 감지용
```

**중요**: 각 그릇 오브젝트에 `DishController.cs` 스크립트 추가

### 3. ShellGameStage Inspector 설정

#### Stage Settings
- **Stage Level**: 난이도 (1~4)
  - Lv1: 그릇 2개
  - Lv2-3: 그릇 3개
  - Lv4: 그릇 4개
- **Timer Time**: 제한 시간 (초)

#### Dish References
- **Dishes**: DishController 배열 (최대 4개)
  - 생성한 그릇 오브젝트들을 순서대로 할당
- **Dishes Container**: 그릇들의 부모 오브젝트 (Optional)

#### Food Settings
- **Food Sprites**: 다양한 음식 스프라이트 배열
- **Wanted Food Sprite**: 찾아야 할 음식 (Wanted 포스터에 표시될 음식)

#### UI References
- **Wanted Poster UI**: Wanted 포스터 UI 오브젝트

#### Animation Settings
- **Lid Close Delay**: 뚜껑 닫히는/열리는 애니메이션 시간 (기본: 1.5초)
- **Shuffle Duration**: 그릇 섞는 총 시간 (기본: 3초)
- **Selection Feedback Duration**: 선택 테두리 표시 시간 (기본: 0.7초)

#### Test Code (디버깅용)
- **Green Sphere**: 클리어 성공 시 활성화될 오브젝트
- **Red Sphere**: 클리어 실패 시 활성화될 오브젝트

### 4. DishController Inspector 설정

각 그릇 오브젝트마다 설정:

#### Visual Components
- **Dish Renderer**: 그릇 SpriteRenderer
- **Food Renderer**: 음식 SpriteRenderer
- **Lid Renderer**: 뚜껑 SpriteRenderer
- **Border Renderer**: 테두리 SpriteRenderer (Optional)

#### Sprites
- **Normal Dish Sprite**: 일반 그릇 스프라이트
- **Selected Dish Sprite**: 선택된 그릇 스프라이트 (테두리 있는 버전)

#### Animation Settings
- **Lid Animation Duration**: 뚜껑 애니메이션 시간 (기본: 0.5초)
- **Lid Open Y Offset**: 뚜껑이 열릴 때 Y축 이동 거리 (기본: 2)

## 🎨 아트 에셋 준비물

### 필수 스프라이트
1. **그릇 이미지** (Dish)
   - 일반 버전
   - 선택된 버전 (테두리 있는)
2. **뚜껑 이미지** (Lid)
3. **음식 이미지들** (Food)
   - 최소 3~4가지 다른 음식
4. **Wanted 포스터 UI**

### 레이어 순서 (Sorting Order)
- 음식: 0
- 그릇: 1
- 뚜껑: 2
- 테두리: 3

## 💡 주요 기능 설명

### ShellGameStage.cs

#### 주요 메서드
- `OnStageStart()`: 스테이지 시작 (타이머는 섞기 후 시작)
- `GameSequence()`: 전체 게임 흐름 관리 코루틴
- `ShowDishContents()`: 그릇 내용물 보여주기
- `CloseLids()`: 뚜껑 닫기
- `ShuffleDishes()`: 그릇 섞기
- `SwapDishes()`: 두 그릇 위치 교환
- `OnDishSelected()`: 플레이어가 그릇 선택 시 호출
- `HandleDishSelection()`: 선택 처리 및 결과 판정
- `RevealCorrectDish()`: 정답 그릇 공개 (실패/타임아웃 시)

#### 난이도별 설정
```csharp
Lv1:  2개 그릇, 섞기 3회
Lv2-3: 3개 그릇, 섞기 6~9회
Lv4:  4개 그릇, 섞기 12회
```

### DishController.cs

#### 주요 메서드
- `SetFood()`: 음식 설정 및 정답 여부 설정
- `ShowContents()`: 내용물 보여주기
- `CloseLid()`: 뚜껑 닫기 애니메이션
- `OpenLid()`: 뚜껑 열기 애니메이션
- `ShowSelectionBorder()`: 선택 테두리 표시
- `OnMouseDown()`: 클릭/터치 감지

## 🔄 게임 시퀀스 타이밍

```
[게임 시작]
    ↓
[내용물 보여주기] ← 2초
    ↓
[뚜껑 닫기] ← 1.5초 (설정 가능)
    ↓
[그릇 섞기] ← 3초 (설정 가능)
    ↓
[타이머 시작] → 플레이어 선택 가능
    ↓
[그릇 선택]
    ↓
[테두리 표시] ← 0.7초
    ↓
[뚜껑 열기] ← 1.5초
    ↓
[결과 판정]
```

## 🐛 디버깅 팁

1. **그릇이 섞이지 않음**
   - `Dishes` 배열이 제대로 할당되었는지 확인
   - Stage Level이 올바르게 설정되었는지 확인

2. **뚜껑 애니메이션이 작동하지 않음**
   - `DishController`의 `Lid Renderer`가 할당되었는지 확인
   - Lid의 Pivot이 중앙 하단에 있는지 확인

3. **클릭이 감지되지 않음**
   - 각 그릇에 `Collider2D`가 있는지 확인
   - 카메라에 `Physics2DRaycaster` 컴포넌트가 있는지 확인

4. **타이머가 너무 일찍 시작됨**
   - `OnStageStart()`에서 타이머가 시작되지 않도록 수정됨
   - `GameSequence()` 코루틴에서 섞기 후에 타이머 시작

## 🎯 향후 개선 사항

- [ ] 그릇 섞기 애니메이션 더 부드럽게 (곡선 경로)
- [ ] 사운드 효과 추가
- [ ] 파티클 이펙트 추가 (선택, 정답, 오답)
- [ ] 난이도별 섞는 속도 조절
- [ ] Wanted 포스터 애니메이션

## 📞 문의

문제가 발생하거나 질문이 있으시면 개발팀에 문의해주세요.
