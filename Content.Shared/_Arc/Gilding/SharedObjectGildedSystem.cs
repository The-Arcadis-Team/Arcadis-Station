using Robust.Shared.Serialization;
using Content.Shared.DoAfter;

namespace Content.Shared._Arc.Admemery;

[Serializable, NetSerializable]
public sealed partial class GildedObjectMeltedEvent : SimpleDoAfterEvent
{
    [DataField("delay", required: true)]
    public float Delay;
    public GildedObjectMeltedEvent(float delay)
    {
        Delay = delay;
    }
}

