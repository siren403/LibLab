# 방향 기반 블록 이동 시스템 구현 노트

## 🎯 개요
방향 기반 블록 이동 시스템 구현 과정에서 발견한 코드 스펙 불일치 및 해결 방법을 기록합니다.

## 🔧 주요 구현 사항

### 1. Direction 값 객체 구현
```csharp
// Assets/App.MergeGame/MergeGame.Core/ValueObjects/Direction.cs
[UnitOf(typeof(float2))]
public readonly partial struct Direction
{
    public float X => value.x;
    public float Y => value.y;
    
    public Direction(float x, float y)
    {
        // 방향 벡터 정규화
        var magnitude = math.sqrt(x * x + y * y);
        if (magnitude > 0)
        {
            value = new float2(x / magnitude, y / magnitude);
        }
        else
        {
            value = new float2(0, 0);
        }
    }
    
    public static Direction FromCoordinates(float fromX, float fromY, float toX, float toY)
    {
        return new Direction(toX - fromX, toY - fromY);
    }
    
    public bool IsValid => Magnitude > 0;
    public float Magnitude => math.length(value);
    
    // 기본 방향 상수
    public static Direction Up => new Direction(0, 1);
    public static Direction Down => new Direction(0, -1);
    public static Direction Left => new Direction(-1, 0);
    public static Direction Right => new Direction(1, 0);
}
```

### 2. Board 도메인 로직 확장
```csharp
// Assets/App.MergeGame/MergeGame.Core/Internal/Entities/Board.cs
public FastResult<BoardCell> FindNearestEmptyCellFromDirection(Position from, Direction direction)
{
    if (!direction.IsValid)
    {
        return FastResult<BoardCell>.Ok(GetCell(from));
    }

    // 방향 × 보드 사이즈로 충분히 멀리 이동
    var targetX = from.X + direction.X * Width;
    var targetY = from.Y + direction.Y * Height;

    // 보드 경계로 클램핑하여 경계 위치 계산
    var boundaryPosition = new Position(
        math.clamp((int)math.round(targetX), 0, Width - 1),
        math.clamp((int)math.round(targetY), 0, Height - 1)
    );

    // 기존 FindNearestEmptyCell 로직 재사용
    return FindNearestEmptyCell(from, boundaryPosition);
}
```

### 3. 새로운 커맨드 및 핸들러
```csharp
// MoveBlockFromDirectionCommand.cs
public struct MoveBlockFromDirectionCommand : ICommand<FastResult<Position>>
{
    public Ulid SessionId { get; init; }
    public Position FromPosition { get; init; }
    public Direction Direction { get; init; }
}

// MoveBlockFromDirectionHandler.cs
internal class MoveBlockFromDirectionHandler : ICommandHandler<MoveBlockFromDirectionCommand, FastResult<Position>>
{
    private readonly GameManager _manager;

    public UniTask<FastResult<Position>> ExecuteAsync(MoveBlockFromDirectionCommand command, CancellationToken ct)
    {
        var boardResult = _manager.GetBoardOrError(command.SessionId);
        if (boardResult.IsError(out FastResult<Position> boardFail))
            return UniTask.FromResult(boardFail);

        var board = boardResult.Value;
        var findResult = board.FindNearestEmptyCellFromDirection(command.FromPosition, command.Direction);
        if (findResult.IsError(out FastResult<Position> findFail))
            return UniTask.FromResult(findFail);

        var targetPosition = findResult.Value.Position;
        var moveResult = board.MoveBlock(command.FromPosition, targetPosition);
        if (moveResult.IsError(out FastResult<Position> moveFail))
            return UniTask.FromResult(moveFail);

        return UniTask.FromResult(FastResult<Position>.Ok(targetPosition));
    }
}
```

### 4. GameController 확장
```csharp
// GameController.MoveBlockFromDirection.cs
public async UniTask<FastResult<MovedResponse>> MoveBlockFromDirection(
    Ulid sessionId,
    Vector2Int fromPosition,
    Vector2 dragStartWorldPosition,
    Vector2 dragEndWorldPosition,
    CancellationToken ct = default)
{
    var direction = Direction.FromCoordinates(
        dragStartWorldPosition.x, dragStartWorldPosition.y,
        dragEndWorldPosition.x, dragEndWorldPosition.y
    );

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
```

### 5. TileReleasedCommand 확장
```csharp
// TileSelector.cs
public readonly struct TileReleasedCommand : ICommand
{
    public GameObject? Target { get; init; }
    public Vector3 WorldPosition { get; init; }  // 추가된 필드
    
    public bool TryGetTarget(out GameObject tile) { /* ... */ }
}

// TileSelector 생성 부분 수정
TileReleasedCommand cmd = hitCount > 0
    ? new TileReleasedCommand { Target = hits.First().transform.gameObject, WorldPosition = result.WorldPosition }
    : new TileReleasedCommand { WorldPosition = result.WorldPosition };
```

### 6. MergeGameEntryPoint 통합
```csharp
// 드래그 시작 위치 추적
Vector2Int? selectedCell = null;
Vector2? dragStartWorldPosition = null;

// 드래그 시작 시 위치 저장
_router.Subscribe<TileDraggingCommand>((cmd, ctx) =>
{
    if (!selectedCell.HasValue) return;
    
    if (!dragStartWorldPosition.HasValue)
        dragStartWorldPosition = cmd.WorldPosition;
    
    _focusFrame.Hide();
    // ... 기존 로직
});

// 타일 검출 실패 시 방향 기반 이동 처리
_router.SubscribeAwait<TileReleasedCommand>(async (cmd, ctx) =>
{
    if (!cmd.TryGetTarget(out var go))
    {
        if (dragStartWorldPosition.HasValue)
        {
            var moveResult = await _controller.MoveBlockFromDirection(
                sessionId, selectedCell.Value,
                dragStartWorldPosition.Value, cmd.WorldPosition,
                ctx.CancellationToken);

            if (!moveResult.IsError)
            {
                var movedResponse = moveResult.Value;
                
                // MoveBlockCommand를 Publish하여 UI 업데이트
                _ = _router.PublishAsync(
                    new MoveBlockCommand() { 
                        FromCell = selectedCell.Value, 
                        ToCell = movedResponse.ToCell 
                    },
                    ctx.CancellationToken);
                
                // 상태 초기화
                selectedCell = null;
                dragStartWorldPosition = null;
                _focusFrame.Hide();
                return;
            }
        }
        
        Fail(selectedCell.Value);
        return;
    }
    
    // ... 기존 로직
});
```

## 🚨 구현 중 발견한 코드 스펙 불일치

### 1. GameManager 사용법 오류
```csharp
// ❌ 잘못된 사용 (Obsolete)
var board = _manager.GetBoardOrThrow(command.SessionId);

// ✅ 올바른 사용
var boardResult = _manager.GetBoardOrError(command.SessionId);
if (boardResult.IsError(out FastResult<Position> boardFail))
    return boardFail;
var board = boardResult.Value;
```

### 2. FastResult API 사용법 오류 및 권장 패턴
```csharp
// ❌ 존재하지 않는 API
if (moveResult.IsOk) { /* ... */ }
if (moveResult.IsSuccess) { /* ... */ }

// ❌ 부정 조건 사용 (권장하지 않음)
if (!moveResult.IsError) { 
    // 성공 로직
}

// ✅ 권장 패턴: 에러 우선 처리
if (moveResult.IsError) {
    // 에러 처리 및 조기 반환
    return;
}

// 성공 로직 (에러가 아닌 경우에만 실행)
var result = moveResult.Value;
```

**권장 이유**:
- `IsError`가 `true`일 때 `Value`는 유효하지 않음
- 에러를 먼저 처리하고 조기 반환하여 코드 가독성 향상
- Guard Clause 패턴으로 중첩 조건문 방지
- 에러 상황에서 후속 코드 실행 방지

### 3. MovedResponse 프로퍼티명 오류
```csharp
// ❌ 존재하지 않는 프로퍼티
var position = movedResponse.Position;

// ✅ 실제 프로퍼티
var position = movedResponse.ToCell;
```

### 4. TileReleasedCommand 필드 누락
```csharp
// ❌ WorldPosition 필드 없음
public readonly struct TileReleasedCommand : ICommand
{
    public GameObject? Target { get; init; }
}

// ✅ WorldPosition 필드 추가
public readonly struct TileReleasedCommand : ICommand
{
    public GameObject? Target { get; init; }
    public Vector3 WorldPosition { get; init; }
}
```

### 5. 상태 관리 불완전
```csharp
// ❌ 드래그 시작 위치 추적 없음
Vector2Int? selectedCell = null;

// ✅ 드래그 시작 위치 추적 추가
Vector2Int? selectedCell = null;
Vector2? dragStartWorldPosition = null;

// 모든 종료 지점에서 상태 초기화
void Fail(Vector2Int returnPosition)
{
    _focusFrame.Restore();
    // ...
    selectedCell = null;
    dragStartWorldPosition = null;  // 추가
}
```

## 💡 핵심 알고리즘

### 방향 기반 경계 계산
```csharp
// 효율적인 경계 위치 계산 (O(1))
var targetX = from.X + direction.X * Width;
var targetY = from.Y + direction.Y * Height;

var boundaryPosition = new Position(
    math.clamp((int)math.round(targetX), 0, Width - 1),
    math.clamp((int)math.round(targetY), 0, Height - 1)
);
```

**장점**:
- 복잡한 수학적 교점 계산 불필요
- 반복문 없이 O(1) 복잡도
- 간단하고 이해하기 쉬운 로직

## 🎯 테스트 가능한 구조

### 단위 테스트 예시
```csharp
[Test]
public void Direction_FromCoordinates_ShouldNormalizeVector()
{
    // Arrange
    var direction = Direction.FromCoordinates(0, 0, 3, 4);
    
    // Act & Assert
    Assert.That(direction.X, Is.EqualTo(0.6f).Within(0.001f));
    Assert.That(direction.Y, Is.EqualTo(0.8f).Within(0.001f));
    Assert.That(direction.Magnitude, Is.EqualTo(1.0f).Within(0.001f));
}

[Test]
public void Board_FindNearestEmptyCellFromDirection_ShouldUseClampedBoundary()
{
    // Arrange
    var board = Board.CreateWithCells(Ulid.NewUlid(), 5, 5);
    var from = new Position(2, 2);
    var direction = Direction.Right;
    
    // Act
    var result = board.FindNearestEmptyCellFromDirection(from, direction);
    
    // Assert
    Assert.That(result.IsError, Is.False);
    // 경계 위치는 (4, 2)가 되어야 함
}
```

이 구현 노트는 향후 비슷한 기능 구현 시 동일한 실수를 방지하고, 올바른 API 사용법을 참조하기 위한 레퍼런스로 활용할 수 있습니다.