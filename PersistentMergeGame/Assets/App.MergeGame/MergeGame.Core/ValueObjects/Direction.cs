using UnitGenerator;
using Unity.Mathematics;

namespace MergeGame.Core.ValueObjects
{
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

        public void Deconstruct(out float x, out float y)
        {
            x = X;
            y = Y;
        }

        /// <summary>
        /// 두 위치 사이의 방향을 계산합니다.
        /// </summary>
        /// <param name="from">시작 위치</param>
        /// <param name="to">목표 위치</param>
        /// <returns>정규화된 방향 벡터</returns>
        public static Direction FromPositions(Position from, Position to)
        {
            return new Direction(to.X - from.X, to.Y - from.Y);
        }

        /// <summary>
        /// 두 위치 사이의 방향을 계산합니다. (float 좌표)
        /// </summary>
        /// <param name="fromX">시작 X 좌표</param>
        /// <param name="fromY">시작 Y 좌표</param>
        /// <param name="toX">목표 X 좌표</param>
        /// <param name="toY">목표 Y 좌표</param>
        /// <returns>정규화된 방향 벡터</returns>
        public static Direction FromCoordinates(float fromX, float fromY, float toX, float toY)
        {
            return new Direction(toX - fromX, toY - fromY);
        }

        /// <summary>
        /// 방향 벡터의 크기를 반환합니다. (정규화된 벡터이므로 항상 1.0 또는 0.0)
        /// </summary>
        public float Magnitude => math.length(value);

        /// <summary>
        /// 방향이 유효한지 확인합니다. (0이 아닌 방향)
        /// </summary>
        public bool IsValid => Magnitude > 0;

        /// <summary>
        /// 영벡터 방향을 반환합니다.
        /// </summary>
        public static Direction Zero => new Direction(0, 0);

        /// <summary>
        /// 상 방향을 반환합니다.
        /// </summary>
        public static Direction Up => new Direction(0, 1);

        /// <summary>
        /// 하 방향을 반환합니다.
        /// </summary>
        public static Direction Down => new Direction(0, -1);

        /// <summary>
        /// 좌 방향을 반환합니다.
        /// </summary>
        public static Direction Left => new Direction(-1, 0);

        /// <summary>
        /// 우 방향을 반환합니다.
        /// </summary>
        public static Direction Right => new Direction(1, 0);

    }
}
