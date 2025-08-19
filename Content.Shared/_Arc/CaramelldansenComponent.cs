using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.GameStates;

namespace Content.Shared._Arc;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class CaramelldansenComponent : Component
{
    [DataField("timeScale"), AutoNetworkedField]
    public float TimeScale = 0.5f;

    [DataField("rainbowStrength"), AutoNetworkedField]
    public float RainbowStrength = 0.2f;

    [DataField("audioFile"), AutoNetworkedField]
    public SoundSpecifier Music = new SoundPathSpecifier("/Audio/_Arc/caramelldansen.ogg");
}
