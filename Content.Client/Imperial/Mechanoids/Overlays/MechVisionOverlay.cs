using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Maths;
using Robust.Shared.IoC;

namespace Content.Client.Imperial.Mechanoids;

public sealed class MechVisionOverlay : Overlay
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    private static readonly ProtoId<ShaderPrototype> Shader = "MechVisionShader";
    public override OverlaySpace Space => OverlaySpace.ScreenSpace;
    private readonly ShaderInstance _mechVisionShader;

    public MechVisionOverlay()
    {
        IoCManager.InjectDependencies(this);
        _mechVisionShader = _prototype.Index(Shader).Instance();
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        var handle = args.ScreenHandle;
        var viewport = args.ViewportBounds;

        handle.UseShader(_mechVisionShader);
        handle.DrawRect(viewport, Color.White);
        handle.UseShader(null);
    }
}
