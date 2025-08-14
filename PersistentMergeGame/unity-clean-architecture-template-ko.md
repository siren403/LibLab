# Unity Clean Architecture 템플릿

## 아키텍처 개요

이 템플릿은 Unity 프로젝트를 위한 Clean Architecture 구현체를 제공하며, Domain-Driven Design 원칙을 따릅니다. 도메인에 구애받지 않으며 유지보수 가능하고 테스트 가능하며 확장 가능한 아키텍처가 필요한 모든 Unity 프로젝트에 적용할 수 있습니다.

## 레이어 구조

### 1. **{ProjectName}.Contracts** (순수 인터페이스 레이어)
```
Assets/App.{ProjectName}/
└── {ProjectName}.Contracts/
    ├── Assembly Definition (.asmdef)
    │   └── noEngineReferences: true
    │   └── Dependencies: None
    └── 인터페이스 파일들 (I*.cs)
```

**책임:**
- 순수 인터페이스와 계약 정의
- 의존성 역전을 위한 추상화 제공
- 완전히 Unity에 무관하게 유지
- 다른 모든 레이어의 기반 역할

**핵심 원칙:**
- Unity 엔진 참조 없음
- 외부 의존성 없음
- 순수 C# 인터페이스만 포함
- 도메인에 구애받지 않는 추상화

### 2. **{ProjectName}.Core** (도메인/비즈니스 로직 레이어)
```
Assets/App.{ProjectName}/
└── {ProjectName}.Core/
    ├── Assembly Definition (.asmdef)
    │   └── Dependencies: {ProjectName}.Contracts, VContainer, UniTask, etc.
    ├── Application/
    │   ├── Commands/          # CQRS 커맨드
    │   └── Queries/           # CQRS 쿼리
    ├── Internal/
    │   ├── Entities/          # 도메인 엔티티
    │   ├── Handlers/          # 커맨드/쿼리 핸들러
    │   ├── Repositories/      # 리포지토리 인터페이스
    │   ├── Managers/          # 도메인 매니저
    │   └── Services/          # 도메인 서비스
    ├── ValueObjects/          # 값 객체
    ├── Enums/                 # 도메인 열거형
    └── Extensions/            # DI 등록
```

**책임:**
- 모든 비즈니스 로직과 도메인 규칙 포함
- 도메인 엔티티와 값 객체 정의
- CQRS 패턴을 통한 커맨드와 쿼리 구현
- 데이터 접근을 위한 리포지토리 인터페이스 제공
- 도메인 특화 연산 관리

**핵심 원칙:**
- 외부 레이어에 대한 의존성 없음
- 비즈니스 로직을 포함한 풍부한 도메인 모델
- 인터페이스를 통한 의존성 역전
- 프레임워크에 무관한 비즈니스 규칙

### 3. **{ProjectName}.Infrastructure** (데이터 접근 레이어)
```
Assets/App.{ProjectName}/
└── {ProjectName}.Infrastructure/
    ├── Assembly Definition (.asmdef)
    │   └── Dependencies: {ProjectName}.Core, External Data Libraries
    ├── Internal/
    │   ├── Repositories/      # 리포지토리 구현체
    │   └── Services/          # 외부 서비스 구현체
    ├── Models/                # 데이터 전송 객체
    ├── Data/                  # 에셋 기반 데이터 구조
    └── Extensions/            # DI 등록
```

**책임:**
- 데이터 영속성과 검색 구현
- 리포지토리 인터페이스의 구체적 구현 제공
- 외부 서비스 통합 처리
- 데이터 직렬화와 역직렬화 관리

**핵심 원칙:**
- Core 레이어에 정의된 인터페이스 구현
- 모든 외부 데이터 관련 문제 처리
- 데이터 저장소에 대한 추상화 제공
- 데이터 매핑과 변환 관리

### 4. **{ProjectName}.Api** (프레젠테이션 레이어)
```
Assets/App.{ProjectName}/
└── {ProjectName}.Api/
    ├── Assembly Definition (.asmdef)
    │   └── Dependencies: All other layers
    ├── Controllers/           # 게임 컨트롤러
    ├── Inputs/               # 입력 처리 시스템
    ├── UI/                   # 사용자 인터페이스 컴포넌트
    └── Extensions/           # 전체 시스템 DI 등록
```

**책임:**
- 사용자 입력과 상호작용 처리
- 시스템 작업 오케스트레이션
- UI와 프레젠테이션 로직 관리
- 모든 레이어를 통합된 시스템으로 통합

**핵심 원칙:**
- 모든 다른 레이어에 의존
- 레이어 간 조정 역할
- Unity 특화 통합 처리
- 애플리케이션 생명주기 관리

## 의존성 흐름

```
{ProjectName}.Api → {ProjectName}.Infrastructure → {ProjectName}.Core → {ProjectName}.Contracts
```

**의존성 규칙:**
- 각 레이어는 아래 레이어에만 의존 가능
- 순환 의존성 금지
- Core 레이어는 외부 레이어에 절대 의존하지 않음
- Contracts 레이어는 의존성 없음

## Assembly Definition 템플릿

### Contracts 레이어 (.asmdef)
```json
{
    "name": "{ProjectName}.Contracts",
    "references": [],
    "noEngineReferences": true,
    "precompiledReferences": [],
    "defineConstraints": [],
    "optionalUnityReferences": [],
    "versionDefines": []
}
```

### Core 레이어 (.asmdef)
```json
{
    "name": "{ProjectName}.Core",
    "references": [
        "{ProjectName}.Contracts",
        "VContainer",
        "UniTask",
        "VitalRouter",
        "GameKit.Dependencies"
    ],
    "precompiledReferences": [],
    "defineConstraints": [],
    "optionalUnityReferences": [],
    "versionDefines": []
}
```

### Infrastructure 레이어 (.asmdef)
```json
{
    "name": "{ProjectName}.Infrastructure",
    "references": [
        "{ProjectName}.Core",
        "Unity.Addressables",
        "External.Data.Libraries"
    ],
    "precompiledReferences": [],
    "defineConstraints": [],
    "optionalUnityReferences": [],
    "versionDefines": []
}
```

### Api 레이어 (.asmdef)
```json
{
    "name": "{ProjectName}.Api",
    "references": [
        "{ProjectName}.Contracts",
        "{ProjectName}.Core",
        "{ProjectName}.Infrastructure",
        "Unity.InputSystem",
        "R3.Unity"
    ],
    "precompiledReferences": [],
    "defineConstraints": [],
    "optionalUnityReferences": [],
    "versionDefines": []
}
```

## 일반적인 패턴

### 1. **CQRS 패턴**
```csharp
// Core/Application/Commands/
public record SomeCommand(/* 매개변수 */);

// Core/Internal/Handlers/
public class SomeCommandHandler : ICommandHandler<SomeCommand>
{
    public async UniTask Handle(SomeCommand command) { /* 구현 */ }
}
```

### 2. **리포지토리 패턴**
```csharp
// Core/Internal/Repositories/
public interface ISomeRepository
{
    UniTask<SomeEntity> GetAsync(EntityId id);
    UniTask SaveAsync(SomeEntity entity);
}

// Infrastructure/Internal/Repositories/
public class SomeRepository : ISomeRepository
{
    public async UniTask<SomeEntity> GetAsync(EntityId id) { /* 구현 */ }
    public async UniTask SaveAsync(SomeEntity entity) { /* 구현 */ }
}
```

### 3. **도메인 엔티티 패턴**
```csharp
// Core/Internal/Entities/
public class SomeEntity
{
    public EntityId Id { get; private set; }
    
    public void DoSomething() { /* 비즈니스 로직 */ }
    private void ValidateState() { /* 검증 로직 */ }
}
```

### 4. **값 객체 패턴**
```csharp
// Core/ValueObjects/
public readonly record struct SomeValue(int Value)
{
    public static implicit operator int(SomeValue value) => value.Value;
    public static implicit operator SomeValue(int value) => new(value);
}
```

### 5. **의존성 주입 등록**
```csharp
// 각 레이어의 Extensions/
public static class SomeLayerExtensions
{
    public static void RegisterSomeLayer(this ContainerBuilder builder)
    {
        builder.Register<ISomeService, SomeService>(Lifetime.Singleton);
        // ... 기타 등록
    }
}
```

## 테스트 구조

### 테스트 어셈블리 구성
```
Assets/App.{ProjectName}/
├── {ProjectName}.Contracts.Tests/
├── {ProjectName}.Core.Tests/
├── {ProjectName}.Infrastructure.Tests/
└── {ProjectName}.Api.Tests/
```

### 테스트 의존성
- 각 테스트 어셈블리는 해당 레이어에 의존
- 테스트 어셈블리는 동일한 의존성 계층 구조를 따름
- Unity 특화 테스트를 위한 Unity Test Framework 통합
- 순수 C# 단위 테스트를 위한 NUnit

## 주요 장점

1. **테스트 가능성**: 각 레이어를 독립적으로 테스트 가능
2. **유지보수성**: 명확한 관심사 분리
3. **확장성**: 기존 코드에 영향 없이 새 기능 추가 용이
4. **유연성**: UI와 데이터 접근에 독립적인 비즈니스 로직
5. **Unity 통합**: 적절한 레이어에서 Unity 기능의 올바른 사용

## 구현 가이드라인

1. **Contracts부터 시작**: 인터페이스를 먼저 정의
2. **Core 로직 구축**: 외부 의존성 없이 비즈니스 규칙 구현
3. **Infrastructure 추가**: 데이터 접근과 외부 서비스 구현
4. **API 레이어 생성**: Unity 특화 코드로 모든 것을 연결
5. **테스트 작성**: 각 레이어가 철저히 테스트되도록 보장
6. **DI 구성**: 전체 시스템에 대한 의존성 주입 설정

## 피해야 할 일반적인 함정

1. **순환 의존성**: 항상 의존성 방향을 존중
2. **빈혈 도메인 모델**: 비즈니스 로직을 도메인 엔티티에 배치
3. **추상화 누수**: 인터페이스를 통해 구현 세부사항 노출 금지
4. **God Object**: 클래스를 단일 책임에 집중
5. **Unity 결합**: Unity 특화 코드는 API 레이어에만 유지

이 템플릿은 Clean Architecture 원칙을 유지하면서 시간이 지남에 따라 성장하고 발전할 수 있는 유지보수 가능하고 테스트 가능한 Unity 애플리케이션을 구축하기 위한 견고한 기반을 제공합니다.

## AI 프롬프트 활용 가이드

새 프로젝트를 생성할 때 다음과 같이 활용하세요:

### 프롬프트 예시
```
Unity 프로젝트에서 Clean Architecture를 구현해주세요. 
프로젝트명: [ProjectName]
도메인: [도메인 설명]

다음 레이어 구조를 따라주세요:
1. {ProjectName}.Contracts - 순수 인터페이스 레이어
2. {ProjectName}.Core - 도메인/비즈니스 로직 레이어  
3. {ProjectName}.Infrastructure - 데이터 접근 레이어
4. {ProjectName}.Api - 프레젠테이션 레이어

각 레이어의 Assembly Definition, 폴더 구조, 기본 클래스들을 생성하고 
CQRS, Repository 패턴, 의존성 주입을 적용해주세요.
```

이 템플릿 문서와 함께 프롬프트를 제공하면 AI가 일관된 Clean Architecture 구조를 생성할 수 있습니다.