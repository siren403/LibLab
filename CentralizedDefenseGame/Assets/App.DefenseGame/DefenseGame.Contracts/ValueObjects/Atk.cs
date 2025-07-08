using UnitGenerator;

namespace DefenseGame.Contracts.ValueObjects
{
    [UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator)]
    public partial struct Atk
    {
    }
}
