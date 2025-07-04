using VExtensions.Mediator.Abstractions;

namespace MergeGame.Api.Game
{
    public partial class GameController
    {
        private readonly IMediator _mediator;

        public GameController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
