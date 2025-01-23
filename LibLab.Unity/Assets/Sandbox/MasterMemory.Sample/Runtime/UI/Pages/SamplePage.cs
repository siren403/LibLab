using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Properties;
using UnityEngine.UIElements;
using VitalRouter;
using VitalRouter.MRuby;

namespace MasterMemory.Sample.UI
{
    [MRubyObject]
    public partial struct SampleCommand : ICommand
    {

    }

    [Routes]
    [Serializable]
    [MRubyCommand("sample", typeof(SampleCommand))]
    [MRubyCommand(AddCommand.Key, typeof(AddCommand))]
    public partial class SamplePresenter : MRubyCommandPreset, IPresenter
    {
        public readonly CalculatorPresenter Calculator = new();

        [Route]
        private void On(SampleCommand cmd)
        {
        }

        [Route]
        private async ValueTask On(AddCommand cmd, PublishContext ctx)
        {
            await Calculator.On(ctx.CancellationToken);
        }
    }

    public class Foo
    {
        public override string ToString()
        {
            return nameof(Foo);
        }
    }

    [UxmlObject]
    public partial class SampleObject
    {
        public readonly Foo Foo = new();
    }


    [UxmlElement]
    public partial class SamplePage : PageBase<SamplePresenter>
    {
        [CreateProperty]
        public SampleObject SampleObject { get; set; } = new();

        /*
         * UxmlObjectReference is required property
         */
        [UxmlObjectReference(nameof(Coin))]
        [CreateProperty]
        public Coin Coin { get; set; }

        protected override PageContext CreateContext()
        {
            MRubyContext ruby = MRubyContext.Create();
            return PageContext.Create(this, ruby, new SamplePresenter(), new Router());
        }
    }

    [UxmlObject]
    public partial class ContextBinding : CustomBinding
    {
        public static readonly Dictionary<string, PageContext> Contexts = new();
        public static readonly Dictionary<string, Router> Routers = new();

        [UxmlAttribute]
        public string SourceName;

        protected override void OnActivated(in BindingActivationContext context)
        {
            if (string.IsNullOrWhiteSpace(SourceName)) return;

            VisualElement target = context.targetElement;

            switch (context.bindingId.ToString())
            {
                case nameof(ISharedContext.Context) when target is ISharedContext has:
                    if (!Contexts.TryGetValue(SourceName, out PageContext ctx)) return;
                    has.Context = ctx;
                    break;
                case nameof(IBindableRouter.Router) when target is IBindableRouter has:
                    if (!Routers.TryGetValue(SourceName, out Router router)) return;
                    has.Router = router;
                    break;
            }
        }
    }

    public interface IBindableRouter
    {
        Router Router { set; }
    }

    public interface ISharedContext
    {
        PageContext Context { set; }
    }
}
