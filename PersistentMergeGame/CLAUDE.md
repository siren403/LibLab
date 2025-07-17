# 프로젝트 컨텍스트 개요

## 요구사항 문서
- [requirements.md](requirements.md)에 모든 주요 요구사항 정의

## 설계 문서
- [design.md](design.md)에서 아키텍처, 구조, 핵심 결정 사항 정리

## Tasks 문서
- [tasks.md](tasks.md)에 현재/예정 작업 내역 관리

---

# 머지 게임 시스템 프로젝트 컨텍스트

## 🏗️ 프로젝트 개요
Unity 기반 2D 머지 게임 시스템 개발 프로젝트로, Clean Architecture와 Domain-Driven Design을 적용한 확장 가능한 게임 시스템입니다.

## 📋 핵심 문서
1. **requirements.md** - 시스템 요구사항 명세서
2. **design.md** - 시스템 설계 문서
3. **tasks.md** - 작업 목록 및 개발 계획
4. **implementation-notes.md** - 구현 세부사항 및 트러블슈팅

## 🔧 기술 스택
- **엔진**: Unity 6000.0.49f1
- **언어**: C# 12
- **아키텍처**: Clean Architecture + DDD
- **데이터베이스**: PostgreSQL (Supabase)
- **의존성 주입**: VContainer
- **비동기**: UniTask
- **테스트**: Unity Test Framework + NUnit

## 🏛️ 아키텍처 구조
```
Assets/App.MergeGame/
├── MergeGame.Contracts/     # 순수 인터페이스
├── MergeGame.Core/          # 도메인 로직
├── MergeGame.Infrastructure/ # 데이터 접근
└── MergeGame.Api/           # 프레젠테이션
```

## 🎮 핵심 기능
- **블록 이동 시스템**: 터치/클릭 기반 블록 조작
- **블록 합성 시스템**: 동일 레벨 블록 합성으로 상위 블록 생성
- **게임 세션 관리**: 게임 시작/종료 및 상태 관리
- **실시간 데이터 동기화**: Supabase를 통한 클라우드 저장

## 📈 현재 개발 상태
- **아키텍처**: Clean Architecture 기반 구조 완성
- **도메인 모델**: Board, Block, GameSession 엔티티 구현
- **데이터베이스**: Supabase PostgreSQL 연동
- **테스트**: 단위/통합 테스트 구조 완비

## 🚀 다음 단계
Phase 1 (기반 안정화) 작업 진행 중:
1. 의존성 주입 최적화
2. 블록 이동 로직 최적화
3. 합성 규칙 엔진 확장
4. 캐싱 시스템 구현
5. 입력 시스템 개선

## 💡 개발 가이드라인
- **코드 품질**: 80% 이상 테스트 커버리지 유지
- **성능**: 60FPS 안정 유지, 메모리 사용량 512MB 이내
- **보안**: 클라이언트-서버 통신 암호화, 데이터 무결성 보장
- **확장성**: 모듈화된 구조로 새로운 기능 추가 용이

## 📖 참고 자료
상세한 내용은 각 문서를 참조하세요:
- 기능 및 기술 요구사항 → **requirements.md**
- 시스템 아키텍처 및 설계 → **design.md**
- 작업 계획 및 우선순위 → **tasks.md**
- 구현 세부사항 및 트러블슈팅 → **implementation-notes.md**