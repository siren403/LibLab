using App.Features.LayeredBlocks.Commands;
using Microsoft.Extensions.Logging;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VitalRouter;
using VitalRouter.VContainer;

namespace App.Features.LayeredBlocks
{
    public class LayerdBlocksScene : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterVitalRouter(routing =>
            {
                routing.Map<LayerdBlocksPresenter>();
            });
            builder.RegisterEntryPoint<EntryPoint>();
        }

        class EntryPoint : IInitializable
        {
            private readonly ILogger<EntryPoint> _logger;

            public EntryPoint(ILogger<EntryPoint> logger)
            {
                _logger = logger;
            }

            public void Initialize()
            {

            }
        }
    }

    [Routes]
    public partial class LayerdBlocksPresenter
    {
        private readonly ILogger<LayerdBlocksPresenter> _logger;

        public LayerdBlocksPresenter(ILogger<LayerdBlocksPresenter> logger)
        {
            Debug.LogWarning("LayeredBlocksPresenter constructor");
            _logger = logger;
        }

        [Route]
        private void OnBlockClicked(BlockClickedCommand command)
        {
            _logger.LogInformation("Block clicked");
        }
    }
}
