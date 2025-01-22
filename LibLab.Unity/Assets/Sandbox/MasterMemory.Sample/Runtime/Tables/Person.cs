// resharper disable ArrangeAttributes

using MessagePack;

namespace MasterMemory.Sample
{
    public enum Gender
    {
        Male,
        Female,
        Unknown
    }

    [MemoryTable("person")]
    [MessagePackObject(true)]
    public record Person
    {
        [PrimaryKey]
        public int PersonId { get; init; }

        // secondary index can add multiple(discriminated by index-number).
        [SecondaryKey(0), NonUnique]
        [SecondaryKey(1, 1), NonUnique]
        public int Age { get; init; }

        [SecondaryKey(2), NonUnique]
        [SecondaryKey(1), NonUnique]
        public Gender Gender { get; init; }

        public string Name { get; init; }
    }
}
