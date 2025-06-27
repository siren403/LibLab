namespace App.MergeGame.Core.Internal.Enums
{
    internal enum GameState
    {
        WaitingToStart, // 게임 시작 대기 (초기 상태)
        Playing, // 게임 진행 중
        Paused, // 게임 일시정지
        GameOver, // 게임 종료 (더 이상 이동 불가)
        Completed // 게임 완료 (목표 달성)
    }
}
