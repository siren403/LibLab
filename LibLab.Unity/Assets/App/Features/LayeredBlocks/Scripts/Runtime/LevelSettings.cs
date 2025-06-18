using SceneLauncher.VContainer;
using UnitGenerator;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;
using VitalRouter;

namespace App.Features.LayeredBlocks
{
    [UnitOf(typeof(Vector2Int), UnitGenerateOptions.Validate)]
    public readonly partial struct BlockSize
    {
        public static readonly BlockSize TwoSquare = new(new Vector2Int(2, 2));

        private partial void Validate()
        {
            if (value.x <= 0 || value.y <= 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(value), "BlockSize must be greater than 0");
            }
        }
    }

    public class LevelSettings : ContainerBehaviour
    {
        private LevelBlock[]? _blockPrefabs;

        private (Vector2Int position, BlockSize size)[] _layout = new[]
        {
            (new Vector2Int(0, 0), BlockSize.TwoSquare),
            (new Vector2Int(2, 0), BlockSize.TwoSquare),
            (new Vector2Int(4, 0), BlockSize.TwoSquare),
            (new Vector2Int(6, 0), BlockSize.TwoSquare),
            (new Vector2Int(8, 0), BlockSize.TwoSquare),
        };

        protected override void Awake()
        {
            base.Awake();
            _blockPrefabs = GetComponentsInChildren<LevelBlock>();
            foreach (var block in _blockPrefabs)
            {
                block.gameObject.SetActive(false);
            }
        }

        [Inject] private Router? _router;

        protected override void OnBuild(IObjectResolver container)
        {
            if (_blockPrefabs != null)
            {
                var layer = new GameObject(".Layer | 1").AddComponent<SortingGroup>();
                layer.transform.localPosition = Vector3.zero;
                layer.sortingOrder = 1;

                var prefab = _blockPrefabs[0];
                var instance = Instantiate(prefab, layer.transform);
                instance.name = $"#1 | A";
                instance.transform.localPosition = Vector3.zero;
                instance.gameObject.SetActive(true);
            }
        }
    }
}
