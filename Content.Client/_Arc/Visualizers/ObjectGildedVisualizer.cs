using System.Linq;
using Robust.Client.GameObjects;
using static Robust.Client.GameObjects.SpriteComponent;
using Content.Client.Kitchen.Components;
using Content.Shared.Clothing;
using Content.Shared.Hands;
using Content.Shared._Arc.Admemery;
using Robust.Shared.Prototypes;
using Robust.Client.Graphics;

namespace Content.Client._Arc.Visualizers
{
    public sealed class ObjectGildedVisualizerSystem : VisualizerSystem<ObjectGildedComponent>
    {
        [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;
        [Dependency] private readonly IPrototypeManager _protoMan = default!;
        [Dependency] private readonly ILogManager _logManager = default!;
        private ISawmill _sawmill = default!;
        private readonly static string ShaderName = "Gilded";
        private ShaderInstance _shader = default!;

        public override void Initialize()
        {
            base.Initialize();

            _shader = _protoMan.Index<ShaderPrototype>(ShaderName).InstanceUnique();

            _sawmill = _logManager.GetSawmill("gilded");

            SubscribeLocalEvent<ObjectGildedComponent, HeldVisualsUpdatedEvent>(OnHeldVisualsUpdated);
            SubscribeLocalEvent<ObjectGildedComponent, EquipmentVisualsUpdatedEvent>(OnEquipmentVisualsUpdated);
            SubscribeLocalEvent<ObjectGildedComponent, ComponentShutdown>(OnShutdown);
        }

        protected override void OnAppearanceChange(EntityUid uid, ObjectGildedComponent component, ref AppearanceChangeEvent args)
        {
            if (args.Sprite == null)
                return;

            // _sawmill.Info("Updating appearence");

            if (!_appearanceSystem.TryGetData(uid, GildedVisuals.Gilded, out bool isGilded) || !isGilded)
            {
                for (var i = 0; i < args.Sprite.AllLayers.Count(); ++i)
                    if (args.Sprite.TryGetLayer(i, out var layer) && layer.Shader == _shader)
                    {
                        args.Sprite.LayerSetShader(i, null, null);
                    }

                return;
            }

            for (var i = 0; i < args.Sprite.AllLayers.Count(); ++i)
            {
                args.Sprite.LayerSetShader(i, _shader);
            }
        }

        private void OnHeldVisualsUpdated(EntityUid uid, ObjectGildedComponent component, HeldVisualsUpdatedEvent args)
        {
            if (args.RevealedLayers.Count == 0)
                return;

            if (!TryComp(args.User, out SpriteComponent? sprite))
                return;

            foreach (var key in args.RevealedLayers)
            {
                if (!sprite.LayerMapTryGet(key, out var index) || sprite[index] is not Layer layer)
                    continue;

                sprite.LayerSetShader(index, ShaderName);
            }
        }

        private void OnEquipmentVisualsUpdated(EntityUid uid, ObjectGildedComponent component, EquipmentVisualsUpdatedEvent args)
        {
            if (args.RevealedLayers.Count == 0)
                return;

            if (!TryComp(args.Equipee, out SpriteComponent? sprite))
                return;

            foreach (var key in args.RevealedLayers)
            {
                if (!sprite.LayerMapTryGet(key, out var index) || sprite[index] is not Layer layer)
                    continue;

                sprite.LayerSetShader(index, ShaderName);
            }
        }

        private void OnShutdown(EntityUid uid, ObjectGildedComponent component, ComponentShutdown args)
        {
            if (!Terminating(uid))
            {
                _appearanceSystem.SetData(uid, GildedVisuals.Gilded, false);

                // Get entity sprite and clear shader.
                if (TryComp<SpriteComponent>(uid, out var sprite))
                {
                    // _sawmill.Info("Removing shader");
                    for (var i = 0; i < sprite.AllLayers.Count(); ++i)
                    {
                        if (sprite.TryGetLayer(i, out var layer) && layer.Shader == _shader)
                        {
                            // _sawmill.Info("Nuking shader on layer");
                            sprite.LayerSetShader(i, null, null);
                        }
                    }
                }
            }
        }
    }
}
