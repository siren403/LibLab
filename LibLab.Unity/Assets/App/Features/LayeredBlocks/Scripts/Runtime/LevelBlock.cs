using App.Features.LayeredBlocks.Commands;
using Microsoft.Extensions.Logging;
using SceneLauncher.VContainer;
using UnitGenerator;
using UnityEngine;
using VContainer;
using VitalRouter;

namespace App.Features.LayeredBlocks
{
    [UnitOf(typeof(int), UnitGenerateOptions.Validate)]
    public readonly partial struct BlockId
    {
        private partial void Validate()
        {
            if (value <= 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(value), "BlockId must be greater than 0");
            }
        }

    }

    [RequireComponent(typeof(BoxCollider2D))]
    public class LevelBlock : ContainerBehaviour
    {
        [Inject] private ILogger<LevelBlock>? _logger;
        [Inject] private Router? _router;

        private void OnMouseDown()
        {
            _router?.PublishAsync(new BlockClickedCommand());
        }
    }
}
