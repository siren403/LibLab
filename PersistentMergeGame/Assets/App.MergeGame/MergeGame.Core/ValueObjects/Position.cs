using UnitGenerator;
using Unity.Mathematics;

namespace MergeGame.Core.ValueObjects
{
    [UnitOf(typeof(int2))]
    public readonly partial struct Position
    {
        public int X => value.x;
        public int Y => value.y;

        public Position(int x, int y)
        {
            value = new int2(x, y);
        }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public static float DistanceSq(Position a, Position b)
        {
            return math.distancesq(a.value, b.value);
        }
    }
}
