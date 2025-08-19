using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client._Arc.Overlays;

public sealed class CaramelldansenOverlay : Overlay
{
    private readonly ShaderInstance _caramelldansenShader;
    private readonly IPlayerManager _playerManager;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    public override bool RequestScreenTexture => true;

    public float EffectScale = 1.0f;
    public float TimeScale = 0.5f;
    public float RainbowStrength = 0.7f;

    public CaramelldansenOverlay()
    {
        var protoMan = IoCManager.Resolve<IPrototypeManager>();
        _caramelldansenShader = protoMan.Index<ShaderPrototype>("Caramelldansen").InstanceUnique();
        _playerManager = IoCManager.Resolve<IPlayerManager>();
    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        // Optionally, you can add more checks here if needed
        return EffectScale > 0;
    }

    public void UpdateFromComponent(Content.Shared._Arc.CaramelldansenComponent? comp)
    {
        if (comp != null)
        {
            TimeScale = comp.TimeScale;
            RainbowStrength = comp.RainbowStrength;
        }
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null)
            return;
        var handle = args.WorldHandle;
        _caramelldansenShader.SetParameter("SCREEN_TEXTURE", ScreenTexture);
        _caramelldansenShader.SetParameter("effectScale", EffectScale);
        _caramelldansenShader.SetParameter("TimeScale", TimeScale);
        _caramelldansenShader.SetParameter("RainbowStrength", RainbowStrength);
        handle.UseShader(_caramelldansenShader);
        handle.DrawRect(args.WorldBounds, Color.White);
        handle.UseShader(null);
    }
}
