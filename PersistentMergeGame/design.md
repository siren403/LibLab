# 머지 게임 시스템 설계 문서

## 1. 시스템 개요

### 1.1 아키텍처 원칙
- **Clean Architecture**: 계층 간 명확한 분리와 의존성 역전
- **Domain-Driven Design**: 비즈니스 도메인 중심 설계
- **CQRS (Command Query Responsibility Segregation)**: 명령과 조회 분리
- **Event-Driven Architecture**: 이벤트 기반 시스템 통신
- **Dependency Injection**: 느슨한 결합과 테스트 용이성

### 1.2 기술 스택
```
Frontend: Unity 6000.0.49f1 + C# 12
Backend: Supabase (PostgreSQL)
Architecture: Clean Architecture + DDD
DI Container: VContainer
Async: UniTask
Testing: Unity Test Framework + NUnit
ORM: Supabase SDK
```

## 2. 시스템 아키텍처

### 2.1 계층 구조
```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐│
│  │   Unity UI      │  │ Input Handlers  │  │ Game Controller ││
│  │  Components     │  │                 │  │                 ││
│  └─────────────────┘  └─────────────────┘  └─────────────────┘│
└─────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                        │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐│
│  │   Commands      │  │    Queries      │  │    Handlers     ││
│  │                 │  │                 │  │                 ││
│  └─────────────────┘  └─────────────────┘  └─────────────────┘│
└─────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────┐
│                     Domain Layer                            │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐│
│  │    Entities     │  │  Value Objects  │  │  Domain Services││
│  │                 │  │                 │  │                 ││
│  └─────────────────┘  └─────────────────┘  └─────────────────┘│
└─────────────────────────────────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────┐
│                  Infrastructure Layer                       │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐│
│  │  Repositories   │  │   Database      │  │ External APIs   ││
│  │                 │  │                 │  │                 ││
│  └─────────────────┘  └─────────────────┘  └─────────────────┘│
└─────────────────────────────────────────────────────────────┘
```

### 2.2 모듈 의존성
```
MergeGame.Contracts (순수 인터페이스)
    ↑
MergeGame.Core (도메인 로직)
    ↑
MergeGame.Infrastructure (데이터 접근)
    ↑
MergeGame.Api (프레젠테이션)
```

## 3. 도메인 설계

### 3.1 도메인 모델

#### 3.1.1 Board (집합 루트)
**핵심 책임**:
- 2D 그리드 셀 관리 및 상태 추적
- 블록 이동 및 합성 비즈니스 로직
- 방향 기반 빈 셀 탐색 기능
- 보드 상태 무결성 보장

**주요 메서드**:
- `MoveBlock`: 블록 이동 처리
- `MergeBlock`: 블록 합성 처리  
- `FindNearestEmptyCell`: 기본 빈 셀 탐색
- `FindNearestEmptyCellFromDirection`: 방향 기반 빈 셀 탐색 (신규 추가)

#### 3.1.2 BoardCell (엔티티)
**핵심 책임**:
- 개별 보드 셀의 상태 관리
- 블록 배치 및 제거 로직
- 셀 상태 전환 규칙 적용

**주요 기능**:
- `PlaceBlock`: 블록 배치 및 상태 변경
- `RemoveBlock`: 블록 제거 및 상태 초기화
- `CanPlaceBlock`: 블록 배치 가능성 검증
- `CanMergeTo`: 다른 셀과의 합성 가능성 검증

**상태 전환 다이어그램**:
```
Empty ←→ Occupied
  ↓         ↑
Untouchable → Mergeable → Movable
```

#### 3.1.3 GameSession (집합 루트)
**핵심 책임**:
- 게임 세션 생명주기 관리
- 세션 상태 추적 및 전환
- 세션 기록 및 통계 유지

**주요 기능**:
- `Start`, `Pause`, `Resume`, `End`: 세션 상태 제어
- 세션 시간 추적 및 기록
- 세션 데이터 무결성 보장

### 3.2 값 객체 (Value Objects)

#### 3.2.1 Position
**목적**: 보드 상의 2D 좌표를 나타내는 불변 값 객체
**특징**:
- Unity.Mathematics의 `int2` 기반 구현
- 좌표 유효성 검증 기능
- 거리 계산 및 비교 연산 지원

#### 3.2.2 BlockId  
**목적**: 블록의 고유 식별자를 나타내는 값 객체
**특징**:
- `Ulid` 기반 고유 식별자
- 빈 블록 상태 표현 지원
- 타입 안전성 보장

#### 3.2.3 BoardSize
**목적**: 보드의 크기 정보를 캡슐화하는 값 객체
**특징**:
- 너비와 높이 정보 보유
- 총 셀 수 계산 기능
- 유효성 검증 로직 포함

#### 3.2.4 Direction
**목적**: 방향 벡터를 나타내는 불변 값 객체 (방향 기반 이동 시스템용)
**특징**:
- Unity.Mathematics의 `float2` 기반 구현
- 생성자에서 자동 정규화 처리
- 다양한 방향 생성 메서드 제공
- 기본 방향 상수 (Up, Down, Left, Right)
- 방향 유효성 검증 기능

### 3.3 도메인 서비스

#### 3.3.1 MergeRuleService
**목적**: 블록 합성 규칙을 관리하고 적용하는 도메인 서비스
**책임**:
- 블록 간 합성 가능성 검증
- 합성 규칙 적용 및 결과 생성
- 동적 합성 규칙 관리

#### 3.3.2 BlockSpawnerService  
**목적**: 블록 생성 로직을 담당하는 도메인 서비스
**책임**:
- 블록 타입별 생성 로직
- 초기 블록 배치 전략
- 랜덤 블록 생성 규칙

## 4. 애플리케이션 계층 설계

### 4.1 명령 (Commands)

#### 4.1.1 MoveBlockCommand
**목적**: 보드 내 블록 이동 요청
**입력**: 세션ID, 시작 위치, 목표 위치
**출력**: 이동 결과 (성공/실패, 최종 위치)

#### 4.1.2 MergeBlockCommand
**목적**: 블록 합성 요청
**입력**: 세션ID, 소스 블록 위치, 대상 블록 위치
**출력**: 합성 결과 (새로운 블록 정보, 업데이트된 셀 상태)

#### 4.1.3 MoveBlockFromDirectionCommand
**목적**: 방향 기반 블록 이동 요청 (보드 바깥 드래그용)
**입력**: 세션ID, 시작 위치, 방향 벡터
**출력**: 최종 배치 위치

#### 4.1.4 CreateGameSessionCommand
**목적**: 새로운 게임 세션 생성
**입력**: 보드 레이아웃ID (선택), 게임 설정 (선택)
**출력**: 생성된 세션ID

### 4.2 명령 핸들러 (Command Handlers)

#### 4.2.1 MoveBlockCommandHandler
**처리 과정**:
1. 세션 및 보드 조회
2. 도메인 로직 실행 (Board.MoveBlock)
3. 결과 저장 (Repository)
4. 도메인 이벤트 발행

**의존성**: BoardRepository, GameSessionRepository, DomainEventPublisher

#### 4.2.2 MoveBlockFromDirectionHandler
**처리 과정**:
1. 게임 매니저를 통한 보드 조회
2. 방향 기반 빈 셀 탐색 (FindNearestEmptyCellFromDirection)
3. 실제 블록 이동 수행
4. 최종 위치 반환

**의존성**: GameManager

#### 4.2.3 핸들러 설계 원칙
- **단일 책임**: 각 핸들러는 하나의 명령만 처리
- **트랜잭션 관리**: 필요시 데이터 일관성 보장
- **에러 처리**: FastResult 패턴으로 안전한 에러 전파
- **이벤트 발행**: 도메인 이벤트를 통한 사이드 이펙트 처리

### 4.3 조회 (Queries)

#### 4.3.1 GetBoardStateQuery
**목적**: 현재 보드 상태 조회
**입력**: 세션ID
**출력**: 보드 상태 (크기, 셀 배열, 마지막 업데이트 시간)

#### 4.3.2 GetGameSessionQuery
**목적**: 게임 세션 정보 조회
**입력**: 세션ID
**출력**: 세션 상태, 생성 시간, 종료 시간 등

### 4.4 조회 핸들러 (Query Handlers)

#### 4.4.1 GetBoardStateQueryHandler
**처리 과정**:
1. 세션 정보로 보드ID 조회
2. 보드 상태 데이터 수집
3. 읽기 전용 결과 객체 생성

**특징**:
- 읽기 전용 데이터 반환
- 캐시 활용으로 성능 최적화
- 도메인 객체를 DTO로 변환

#### 4.4.2 CQRS 분리 원칙
- **명령**: 상태 변경, 결과 최소화
- **조회**: 상태 조회, 최적화된 데이터 구조
- **책임 분리**: 읽기와 쓰기 모델 독립적 설계

## 5. 인프라스트럭처 계층 설계

### 5.1 Repository 구현

#### 5.1.1 BoardRepository
```csharp
internal class BoardRepository : IBoardRepository
{
    private readonly ISupabaseClient _client;
    private readonly IMemoryCache _cache;
    private readonly ILogger<BoardRepository> _logger;
    
    public async Task<Board> GetByIdAsync(Ulid id, CancellationToken ct = default)
    {
        // 1. 캐시 확인
        if (_cache.TryGetValue(id, out Board? cachedBoard))
            return cachedBoard;
        
        // 2. 데이터베이스 조회
        var boardData = await _client
            .From<BoardEntity>()
            .Where(x => x.Id == id)
            .Single();
        
        // 3. 도메인 객체 변환
        var board = MapToDomain(boardData);
        
        // 4. 캐시 저장
        _cache.Set(id, board, TimeSpan.FromMinutes(10));
        
        return board;
    }
    
    public async Task SaveAsync(Board board, CancellationToken ct = default)
    {
        var entity = MapToEntity(board);
        
        await _client
            .From<BoardEntity>()
            .Upsert(entity);
        
        _cache.Set(board.Id, board, TimeSpan.FromMinutes(10));
    }
}
```

#### 5.1.2 BlockRepository
```csharp
internal class BlockRepository : IBlockRepository
{
    private readonly ISupabaseClient _client;
    private readonly IMemoryCache _cache;
    
    public async Task<IEnumerable<Block>> GetByBoardIdAsync(Ulid boardId, CancellationToken ct = default)
    {
        var cacheKey = $"blocks_{boardId}";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<Block>? cachedBlocks))
            return cachedBlocks;
        
        var blockEntities = await _client
            .From<BlockEntity>()
            .Where(x => x.BoardId == boardId)
            .Get();
        
        var blocks = blockEntities.Select(MapToDomain).ToList();
        _cache.Set(cacheKey, blocks, TimeSpan.FromMinutes(5));
        
        return blocks;
    }
}
```

### 5.2 데이터베이스 스키마 설계

#### 5.2.1 Boards 테이블
```sql
CREATE TABLE boards (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    width INTEGER NOT NULL,
    height INTEGER NOT NULL,
    layout_id UUID,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX idx_boards_layout_id ON boards(layout_id);
```

#### 5.2.2 Board_Cells 테이블
```sql
CREATE TABLE board_cells (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    board_id UUID NOT NULL REFERENCES boards(id) ON DELETE CASCADE,
    position_x INTEGER NOT NULL,
    position_y INTEGER NOT NULL,
    block_id UUID,
    state VARCHAR(20) NOT NULL DEFAULT 'Empty',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE UNIQUE INDEX idx_board_cells_position ON board_cells(board_id, position_x, position_y);
CREATE INDEX idx_board_cells_block_id ON board_cells(block_id);
```

#### 5.2.3 Blocks 테이블
```sql
CREATE TABLE blocks (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    type VARCHAR(20) NOT NULL,
    level INTEGER NOT NULL DEFAULT 1,
    group_id UUID,
    data JSONB,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

CREATE INDEX idx_blocks_type ON blocks(type);
CREATE INDEX idx_blocks_group_id ON blocks(group_id);
```

#### 5.2.4 Game_Sessions 테이블
```sql
CREATE TABLE game_sessions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    board_id UUID NOT NULL REFERENCES boards(id),
    state VARCHAR(20) NOT NULL DEFAULT 'Created',
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    ended_at TIMESTAMP WITH TIME ZONE,
    data JSONB
);

CREATE INDEX idx_game_sessions_board_id ON game_sessions(board_id);
CREATE INDEX idx_game_sessions_state ON game_sessions(state);
```

### 5.3 외부 서비스 통합

#### 5.3.1 Supabase 클라이언트
```csharp
internal class SupabaseService
{
    private readonly Supabase.Client _client;
    
    public async Task<T> ExecuteAsync<T>(Func<Supabase.Client, Task<T>> operation)
    {
        try
        {
            return await operation(_client);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Supabase operation failed");
            throw;
        }
    }
}
```

#### 5.3.2 실시간 업데이트
```csharp
internal class RealtimeService
{
    private readonly Supabase.Client _client;
    
    public async Task SubscribeToBoardUpdates(Ulid boardId, Action<BoardUpdatedEvent> callback)
    {
        await _client.Realtime
            .From<BoardEntity>()
            .Where(x => x.Id == boardId)
            .Subscribe(ChannelEventType.All, (payload) =>
            {
                var updatedBoard = payload.Model;
                callback(new BoardUpdatedEvent(updatedBoard));
            });
    }
}
```

## 6. 프레젠테이션 계층 설계

### 6.1 게임 컨트롤러

#### 6.1.1 GameController
```csharp
public partial class GameController
{
    private readonly IMediator _mediator;
    
    public GameController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    // 기존 블록 이동 (보드 내부)
    public async UniTask<FastResult<MoveBlockResponse>> MoveBlock(
        Ulid sessionId,
        MoveBlockRequest request,
        CancellationToken ct = default)
    
    // 방향 기반 블록 이동 (보드 바깥 드래그)
    public async UniTask<FastResult<MovedResponse>> MoveBlockFromDirection(
        Ulid sessionId,
        Vector2Int fromPosition,
        Vector2 dragStartWorldPosition,
        Vector2 dragEndWorldPosition,
        CancellationToken ct = default)
    {
        // 드래그 방향 계산
        var direction = Direction.FromCoordinates(
            dragStartWorldPosition.x, dragStartWorldPosition.y,
            dragEndWorldPosition.x, dragEndWorldPosition.y
        );

        // 방향 기반 이동 커맨드 실행
        var command = new MoveBlockFromDirectionCommand
        {
            SessionId = sessionId,
            FromPosition = new Position(fromPosition.x, fromPosition.y),
            Direction = direction
        };

        var result = await _mediator.ExecuteMoveBlockFromDirection(command, ct);
        
        if (result.IsError(out FastResult<MovedResponse> fail))
            return fail;

        var newPosition = result.Value;
        return FastResult<MovedResponse>.Ok(new MovedResponse(new Vector2Int(newPosition.X, newPosition.Y)));
    }
}
```

### 6.2 입력 처리

#### 6.2.1 InputPointerHandler
```csharp
public class InputPointerHandler : MonoBehaviour
{
    private GameController _gameController;
    private Camera _camera;
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandlePointerDown(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            HandlePointerUp(Input.mousePosition);
        }
    }
    
    private async void HandlePointerDown(Vector2 screenPosition)
    {
        var worldPosition = _camera.ScreenToWorldPoint(screenPosition);
        var gridPosition = WorldToGridPosition(worldPosition);
        
        if (IsValidGridPosition(gridPosition))
        {
            await _gameController.SelectBlock(gridPosition);
        }
    }
}
```

### 6.3 UI 시스템

#### 6.3.1 BoardView
```csharp
public class BoardView : MonoBehaviour
{
    [SerializeField] private BoardCellView _cellPrefab;
    [SerializeField] private Transform _boardContainer;
    
    private Dictionary<Position, BoardCellView> _cellViews = new();
    
    public async UniTask InitializeBoard(BoardStateResult boardState)
    {
        for (int x = 0; x < boardState.Size.Width; x++)
        {
            for (int y = 0; y < boardState.Size.Height; y++)
            {
                var position = new Position(x, y);
                var cellView = Instantiate(_cellPrefab, _boardContainer);
                
                cellView.Initialize(position, boardState.Cells[x, y]);
                _cellViews[position] = cellView;
            }
        }
    }
    
    public async UniTask UpdateCell(Position position, BoardCellData cellData)
    {
        if (_cellViews.TryGetValue(position, out var cellView))
        {
            await cellView.UpdateState(cellData);
        }
    }
}
```

#### 6.3.2 BlockView
```csharp
public class BlockView : MonoBehaviour
{
    [SerializeField] private Image _blockImage;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Animator _animator;
    
    public async UniTask Initialize(BlockData blockData)
    {
        _blockImage.sprite = GetSpriteForBlock(blockData.Type, blockData.Level);
        _levelText.text = blockData.Level.ToString();
    }
    
    public async UniTask PlayMoveAnimation(Vector3 from, Vector3 to, float duration = 0.3f)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(to, duration));
        sequence.SetEase(Ease.OutQuad);
        
        await sequence.Play().ToUniTask();
    }
    
    public async UniTask PlayMergeAnimation()
    {
        _animator.SetTrigger("Merge");
        await UniTask.Delay(1000); // 애니메이션 지속 시간
    }
}
```

## 7. 데이터 흐름 설계

### 7.1 사용자 액션 처리 흐름
```
1. 사용자 입력 (터치/클릭)
    ↓
2. InputPointerHandler
    ↓
3. GameController.MoveBlock()
    ↓
4. MoveBlockCommand → Mediator
    ↓
5. MoveBlockCommandHandler
    ↓
6. Domain Logic (Board.MoveBlock())
    ↓
7. Repository.SaveAsync()
    ↓
8. Database Update
    ↓
9. Event Publishing
    ↓
10. UI Update (BoardView)
```

### 7.2 데이터 동기화 흐름
```
1. Server Data Change
    ↓
2. Supabase Realtime Event
    ↓
3. RealtimeService.OnBoardUpdate()
    ↓
4. Domain Event Publishing
    ↓
5. UI Event Handlers
    ↓
6. View Updates
```

## 8. 성능 최적화 설계

### 8.1 캐싱 전략
```csharp
internal class CacheService
{
    private readonly IMemoryCache _cache;
    
    // 단계적 캐시 무효화
    public void InvalidateBoardCache(Ulid boardId)
    {
        _cache.Remove($"board_{boardId}");
        _cache.Remove($"blocks_{boardId}");
        _cache.Remove($"cells_{boardId}");
    }
    
    // 미리 로드 전략
    public async Task PreloadBoardData(Ulid boardId)
    {
        var tasks = new[]
        {
            _boardRepository.GetByIdAsync(boardId),
            _blockRepository.GetByBoardIdAsync(boardId),
            _cellRepository.GetByBoardIdAsync(boardId)
        };
        
        await Task.WhenAll(tasks);
    }
}
```

### 8.2 오브젝트 풀링
```csharp
public class BlockViewPool : MonoBehaviour
{
    [SerializeField] private BlockView _blockPrefab;
    [SerializeField] private int _initialPoolSize = 50;
    
    private Stack<BlockView> _pool = new();
    
    public BlockView GetBlock()
    {
        if (_pool.Count > 0)
        {
            var block = _pool.Pop();
            block.gameObject.SetActive(true);
            return block;
        }
        
        return Instantiate(_blockPrefab);
    }
    
    public void ReturnBlock(BlockView block)
    {
        block.gameObject.SetActive(false);
        _pool.Push(block);
    }
}
```

### 8.3 메모리 관리
```csharp
public class MemoryManager
{
    private readonly Dictionary<Type, WeakReference> _cache = new();
    
    public T Get<T>() where T : class, new()
    {
        var type = typeof(T);
        
        if (_cache.TryGetValue(type, out var weakRef) && 
            weakRef.Target is T cached)
        {
            return cached;
        }
        
        var newInstance = new T();
        _cache[type] = new WeakReference(newInstance);
        return newInstance;
    }
    
    public void ClearCache()
    {
        _cache.Clear();
        GC.Collect();
    }
}
```

## 9. 에러 처리 및 복구 설계

### 9.1 에러 처리 전략
```csharp
public class ErrorHandler
{
    public async Task<FastResult<T>> ExecuteWithRetry<T>(
        Func<Task<T>> operation,
        int maxRetries = 3,
        TimeSpan delay = default)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                var result = await operation();
                return FastResult.Success(result);
            }
            catch (Exception ex) when (IsRetryableError(ex))
            {
                if (attempt == maxRetries - 1)
                    return FastResult.Fail<T>(ErrorCode.RetryLimitExceeded);
                
                await Task.Delay(delay);
            }
        }
        
        return FastResult.Fail<T>(ErrorCode.UnexpectedError);
    }
    
    private bool IsRetryableError(Exception ex)
    {
        return ex is TimeoutException or HttpRequestException;
    }
}
```

### 9.2 상태 복구 메커니즘
```csharp
public class StateRecoveryService
{
    public async Task<FastResult<GameSession>> RecoverGameSession(Ulid sessionId)
    {
        try
        {
            // 1. 로컬 백업 확인
            var localBackup = await _localStorage.GetBackupAsync(sessionId);
            if (localBackup != null && localBackup.IsValid)
            {
                return await RestoreFromLocalBackup(localBackup);
            }
            
            // 2. 서버 백업 확인
            var serverBackup = await _serverStorage.GetBackupAsync(sessionId);
            if (serverBackup != null)
            {
                return await RestoreFromServerBackup(serverBackup);
            }
            
            // 3. 새 세션 생성
            return await CreateNewSession();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to recover game session");
            return FastResult.Fail<GameSession>(ErrorCode.RecoveryFailed);
        }
    }
}
```

## 10. 보안 설계

### 10.1 클라이언트 보안
```csharp
public class SecurityService
{
    private readonly IEncryptionService _encryption;
    
    public async Task<string> EncryptGameData(string data)
    {
        var key = await GetEncryptionKey();
        return _encryption.Encrypt(data, key);
    }
    
    public bool ValidateGameState(GameSession session)
    {
        // 게임 상태 무결성 검증
        var checksum = CalculateChecksum(session);
        return checksum == session.Checksum;
    }
    
    private string CalculateChecksum(GameSession session)
    {
        var data = JsonSerializer.Serialize(session);
        return SHA256.HashData(Encoding.UTF8.GetBytes(data)).ToBase64();
    }
}
```

### 10.2 서버 통신 보안
```csharp
public class SecureApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    
    public async Task<T> SecureRequest<T>(string endpoint, object data)
    {
        var token = await _tokenService.GetValidTokenAsync();
        
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(endpoint, content);
        response.EnsureSuccessStatusCode();
        
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseJson);
    }
}
```

## 11. 모니터링 및 로깅 설계

### 11.1 성능 모니터링
```csharp
public class PerformanceMonitor
{
    private readonly ILogger<PerformanceMonitor> _logger;
    
    public async Task<T> MeasureAsync<T>(string operationName, Func<Task<T>> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var result = await operation();
            
            _logger.LogInformation("Operation {OperationName} completed in {ElapsedMs}ms", 
                operationName, stopwatch.ElapsedMilliseconds);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation {OperationName} failed after {ElapsedMs}ms", 
                operationName, stopwatch.ElapsedMilliseconds);
            throw;
        }
        finally
        {
            stopwatch.Stop();
        }
    }
}
```

### 11.2 이벤트 추적
```csharp
public class EventTracker
{
    public void TrackGameEvent(string eventName, Dictionary<string, object> properties)
    {
        var eventData = new
        {
            EventName = eventName,
            Properties = properties,
            Timestamp = DateTime.UtcNow,
            SessionId = _sessionService.CurrentSessionId
        };
        
        // 로컬 저장
        _localEventStore.Store(eventData);
        
        // 서버 전송 (비동기)
        _ = Task.Run(async () =>
        {
            try
            {
                await _analyticsService.SendEventAsync(eventData);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send analytics event");
            }
        });
    }
}
```

## 12. 확장성 고려사항

### 12.1 모듈화 설계
```csharp
public interface IGameModule
{
    string Name { get; }
    Version Version { get; }
    Task InitializeAsync(IServiceProvider serviceProvider);
    Task ShutdownAsync();
}

public class ModuleManager
{
    private readonly List<IGameModule> _modules = new();
    
    public async Task LoadModuleAsync<T>() where T : IGameModule, new()
    {
        var module = new T();
        await module.InitializeAsync(_serviceProvider);
        _modules.Add(module);
    }
    
    public async Task UnloadModuleAsync(string moduleName)
    {
        var module = _modules.FirstOrDefault(m => m.Name == moduleName);
        if (module != null)
        {
            await module.ShutdownAsync();
            _modules.Remove(module);
        }
    }
}
```

### 12.2 플러그인 시스템
```csharp
public interface IGamePlugin
{
    Task<bool> CanHandleAsync(GameEvent gameEvent);
    Task HandleAsync(GameEvent gameEvent);
}

public class PluginManager
{
    private readonly List<IGamePlugin> _plugins = new();
    
    public async Task ProcessEventAsync(GameEvent gameEvent)
    {
        var applicablePlugins = new List<IGamePlugin>();
        
        foreach (var plugin in _plugins)
        {
            if (await plugin.CanHandleAsync(gameEvent))
            {
                applicablePlugins.Add(plugin);
            }
        }
        
        var tasks = applicablePlugins.Select(p => p.HandleAsync(gameEvent));
        await Task.WhenAll(tasks);
    }
}
```

## 13. 방향 기반 블록 이동 시스템

### 13.1 설계 배경
기존 시스템은 보드 내부에서만 블록 이동이 가능했으나, 사용자가 보드 바깥 영역으로 블록을 드래그할 때 직관적인 반응이 필요했습니다. 단순히 원래 위치로 되돌리는 것이 아니라, 드래그 방향을 고려한 지능적인 배치가 요구되었습니다.

### 13.2 아키텍처 설계 원칙

#### 13.2.1 도메인 중심 설계
- **Direction 값 객체**: 방향 개념을 도메인 모델로 추상화
- **Board 집합 루트 확장**: 방향 기반 빈 셀 탐색 책임 추가
- **기존 로직 재사용**: `FindNearestEmptyCell` 메서드 활용으로 DRY 원칙 준수

#### 13.2.2 Clean Architecture 유지
- **응용 계층**: `MoveBlockFromDirectionCommand` 새로운 사용 사례 추가
- **도메인 계층**: 방향 기반 이동 비즈니스 로직 캡슐화
- **프레젠테이션 계층**: 드래그 상태 관리 및 UI 이벤트 처리

#### 13.2.3 CQRS 패턴 확장
- 기존 `MoveBlockCommand`와 분리된 새로운 커맨드 타입
- 방향 기반 이동의 특수한 요구사항을 별도 처리
- 명령과 조회의 책임 분리 유지

### 13.3 설계 결정사항

#### 13.3.1 방향 계산 방식
- **월드 좌표 기반**: 화면 좌표계와 독립적인 일관된 방향 계산
- **자동 정규화**: 거리에 관계없이 순수한 방향 정보만 추출
- **효율적 경계 계산**: 복잡한 수학적 교점 계산 대신 간단한 곱셈과 클램핑 사용

#### 13.3.2 상태 관리 전략
- **드래그 시작 위치 추적**: 프레젠테이션 계층에서 상태 관리
- **일관된 초기화**: 모든 종료 지점에서 상태 초기화로 메모리 누수 방지
- **폴백 메커니즘**: 실패 시 기존 로직으로 안전한 복구

#### 13.3.3 UI 통합 방식
- **기존 이벤트 시스템 활용**: `MoveBlockCommand` 재사용으로 UI 업데이트
- **투명한 통합**: 기존 UI 컴포넌트 수정 없이 새로운 기능 추가
- **일관된 사용자 경험**: 기존 이동과 동일한 시각적 피드백

### 13.4 확장성 고려사항

#### 13.4.1 다양한 방향 로직 지원
- `Direction` 값 객체의 확장 가능성
- 기본 방향 상수 제공 (Up, Down, Left, Right)
- 향후 대각선 방향이나 곡선 경로 지원 가능

#### 13.4.2 성능 최적화
- 방향 계산의 O(1) 복잡도 유지
- 기존 `FindNearestEmptyCell`의 최적화 혜택 승계
- 메모리 효율적인 값 객체 설계

### 13.5 시스템 통합 영향

#### 13.5.1 기존 시스템과의 호환성
- 기존 블록 이동 로직 완전 보존
- 새로운 기능의 독립적 동작
- 점진적 배포 가능한 구조

#### 13.5.2 테스트 가능성
- 방향 계산 로직의 단위 테스트 용이성
- 도메인 로직과 UI 로직의 분리된 테스트
- 모의 객체를 활용한 통합 테스트 지원

이 설계 문서는 머지 게임 시스템의 전체 아키텍처와 주요 구성 요소들을 포괄적으로 다루고 있으며, 확장 가능하고 유지보수 가능한 시스템을 구축하기 위한 구체적인 가이드라인을 제공합니다.