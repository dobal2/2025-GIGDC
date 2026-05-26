# 사도천

> 2025 GIGDC(게임 공모전) 출품작
> 감정을 테마로 한 2D 보스 배틀 액션 게임

---

## 프로젝트 개요

| 항목 | 내용 |
|------|------|
| 장르 | 2D 보스 배틀 액션 |
| 엔진 | Unity 6000.3.3f1 |
| 언어 | C# |
| 플랫폼 | PC |
| 개발 인원 | 5명 |
| 담당 (서재민) | 보스 AI 시스템, 일반 몹 시스템 |

---

## 게임 소개

흥분, 좌절, 사랑, 허영 — 4가지 감정을 테마로 한 보스를 순서대로 처치하는 2D 액션 게임입니다.
각 보스는 고유한 2페이즈 패턴을 가지며, 플레이어는 카운터 어택과 무기 전환으로 전투합니다.

---

## 담당 구현 (서재민)

### 보스 AI 시스템 — 4종 보스 전체

`Monster → Boss → Boss_X` 3단 상속 구조로 HP 관리, KnockBack, Counter 로직을 공통화하고
각 보스별 고유 패턴을 하위 클래스에서 구현했습니다.

#### Boss_Excitement
- Phase 1: 피격 시마다 랜덤 위치 텔레포트, 유도 미사일 · 폭탄 투척
- Phase 2: `Mathf.Deg2Rad` + 삼각함수로 버블을 반원 배치 후 일제히 플레이어 방향 이동, GlassWall 3연속 소환

#### Boss_Frustration
- Phase 1: `takeDamageCount`로 피격 횟수 추적 → 5회마다 위치 이동, 손가락 낙하 패턴
- Phase 2: 대시 중 `Physics2D.OverlapBoxAll` 히트박스 판정, Battery 소환 (플레이어가 역이용 가능한 폭발 연쇄 메커닉)

#### Boss_Love
- Phase 1: `col.isTrigger = true`로 잠입 후 경고 이펙트 → 재출현 + 부채꼴 잉크 탄막
- Phase 2: `gravityScale = 0` 부유 전환, HeartBubble 파괴 시 플레이어 무적 쉴드 부여

#### Boss_Vain
- `ExecuteSkill(int)` switch 구조로 Phase별 스킬 풀 분리, `PickNewSkillAvoidingRepeat()`으로 연속 동일 패턴 방지
- Phase 2: `player.rotation.eulerAngles.y`로 플레이어 등 방향 계산 후 기습 텔레포트, Clone 소환

---

### 전투 공통 시스템

#### Monster 기반 클래스
```csharp
// 각도 기반 방향 벡터 계산으로 Rigidbody2D Impulse 적용
void KnockBack(Transform attacker, float force, float angle, float duration)
{
    Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
    rigid.AddForce(dir * force, ForceMode2D.Impulse);
}
```

#### Counter 시스템
- 카운터 성공 시 1.5배 데미지 + 1.5초 스턴
- 스턴 중 스프라이트 회색 처리로 상태 시각 피드백
- 캔버스에 카운터 텍스트 런타임 스폰

#### 보스 HP UI
- 오버레이 캔버스에 Slider를 런타임 생성
- `TakeDamage()` 내에서 직접 동기화 (별도 UI 매니저 없음)
- Phase 전환 시 HP 초기화 및 슬라이더 즉시 갱신

---

### 일반 몹 시스템

| 몹 | 특징 |
|----|------|
| LowMonster_Common_regret | 기본 근접 공격 |
| LowMonster_Common_sad | 기본 원거리 공격 |
| LowMonster_Rare_hate | 돌진 + KnockBack |
| LowMonster_Rare_inferior | 라인 투사체 |
| LowMonster_Rare_interest | 광역 이펙트 |
| LowMonster_Rare_lethargy | 폭발 트리거 |

모든 몹은 `Monster` 기반 클래스를 상속해 동일한 Counter · KnockBack 시스템 적용

---

### 패턴 오브젝트

| 오브젝트 | 역할 |
|----------|------|
| `Bubble.cs` | 5초 후 폭발, Phase 2에서는 보스에게도 데미지 |
| `HeartBubble.cs` | 파괴 시 플레이어 무적 쉴드 부여 |
| `Clone.cs` | Vain 소환체, 1.5초 생성 후 자율 추적 |
| `Battery.cs` | Frustration 소환체, 폭발 시 보스에게 연쇄 데미지 |
| `GlassWall.cs` | 이동 장애물, 처치 가능 |
| `VainProjectile.cs` | 접촉 후 0.2초 뒤 플레이어 추적 전환 |
| `HomingMissile.cs` | Excitement Phase 1 유도 미사일 |

---

## 팀 역할 분담

| 이름 | 담당 |
|------|------|
| 서재민 | 보스 AI 전체 (4종), 일반 몹 시스템 (6종) |
| 체현 | 플레이어 시스템, 무기 (활/창/폭탄), Player State Machine |
| 서일 | 스테이지 매니저, 튜토리얼, 씬 구성, BGM |
| 익현 | VFX 이펙트 전체 (픽셀레이션, 글리치, 파티클, 셰이더) |

---

## 폴더 구조

```
Assets/
├── FIle_Jaemin/         # 보스, 몹, 투사체 (서재민 담당)
│   └── Scripts/
│       ├── Boss/        # Boss 기반 클래스 + 4종 보스 + 패턴 오브젝트
│       ├── Mobs/        # 일반 몹 6종
│       └── Projectiles/ # 투사체
├── File_Hyun/           # 플레이어 시스템 (체현 담당)
├── File_Seoil/          # 스테이지, 튜토리얼 (서일 담당)
├── File_Ikhyun/         # VFX 이펙트 (익현 담당)
└── Public/              # 공용 AudioPlayer 등
```

---

## 빌드 환경

- Unity 6000.3.3f1
- Input System Package
- DOTween
- Universal Render Pipeline (URP)
