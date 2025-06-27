using UnityEngine;

namespace App.MergeGame.Data
{
    [CreateAssetMenu(fileName = "Block", menuName = "Merge Game/Block Data", order = 0)]
    public class BlockData : ScriptableObject
    {
        [field: SerializeField] public Block Prefab { get; private set; } = null!;
        [field: SerializeField] public long Id { get; private set; }
    }
}
