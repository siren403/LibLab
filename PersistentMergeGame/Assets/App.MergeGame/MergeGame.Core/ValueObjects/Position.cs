using UnitGenerator;

namespace MergeGame.Core.ValueObjects
{
    [UnitOf(typeof((int x, int y)))]
    public readonly partial struct Position
    {
        public int X => value.x;
        public int Y => value.y;

        public Position(int x, int y)
        {
            value = (x, y);
        }

        public void Deconstruct(out int x, out int y)
        {
            (x, y) = AsPrimitive();
        }
    }
}
