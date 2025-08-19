using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;
using Robust.Shared.GameStates;
using Content.Client._Arc.Overlays;
using Content.Shared._Arc;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Client.Audio;

namespace Content.Client._Arc;

public sealed class CaramelldansenOverlaySystem : EntitySystem
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly AudioSystem _audio = default!;

    private CaramelldansenOverlay _overlay = default!;
    // Client-only: tracks the currently playing audio entity for the local player
    private Entity<AudioComponent>? _playingAudio;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<CaramelldansenComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<CaramelldansenComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<CaramelldansenComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<CaramelldansenComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);
        _overlay = new();
    }

    private void PlayMusic(EntityUid uid, CaramelldansenComponent comp)
    {
        if (comp.Music == null)
            return;
        // Only play if not already playing
        if (_playingAudio != null && _playingAudio.Value.Comp.Playing)
            return;
        var audioParams = AudioParams.Default.WithLoop(true).WithVolume(comp.RainbowStrength);
        var result = _audio.PlayEntity(comp.Music, Filter.Local(), uid, true, audioParams);
        if (result != null)
            _playingAudio = new Entity<AudioComponent>(result.Value.Entity, result.Value.Component);
    }

    private void StopMusic()
    {
        if (_playingAudio != null && _playingAudio.Value.Comp.Playing)
        {
            _audio.Stop(_playingAudio.Value.Owner);
            EntityManager.QueueDeleteEntity(_playingAudio.Value.Owner);
        }
        _playingAudio = null;
    }

    private void UpdateOverlayFromComponent(EntityUid uid)
    {
        if (_player.LocalEntity != uid)
            return;
        if (!EntityManager.TryGetComponent(uid, out CaramelldansenComponent? comp))
            return;
        if (_overlay.TimeScale != comp.TimeScale || _overlay.RainbowStrength != comp.RainbowStrength)
            _overlay.UpdateFromComponent(comp);
        // Update music volume if playing, but only if it changed significantly
        if (_playingAudio != null && _playingAudio.Value.Comp.Playing)
        {
            float minDb = -32f;
            float strength = MathF.Max(_overlay.RainbowStrength, 0f);
            float targetDb = MathF.Pow(minDb, strength + 1);

            const float epsilon = 0.01f;
            if (MathF.Abs(_playingAudio.Value.Comp.Params.Volume - targetDb) > epsilon)
                _audio.SetVolume(_playingAudio, targetDb);
        }
    }

    private void OnPlayerAttached(EntityUid uid, CaramelldansenComponent component, LocalPlayerAttachedEvent args)
    {
        _overlayMan.AddOverlay(_overlay);
        UpdateOverlayFromComponent(uid);
        PlayMusic(uid, component);
    }

    private void OnPlayerDetached(EntityUid uid, CaramelldansenComponent component, LocalPlayerDetachedEvent args)
    {
        if (_overlay != null)
            _overlayMan.RemoveOverlay(_overlay);
        StopMusic();
    }

    private void OnInit(EntityUid uid, CaramelldansenComponent component, ComponentInit args)
    {
        if (_player.LocalEntity == uid)
        {
            _overlayMan.AddOverlay(_overlay);
            UpdateOverlayFromComponent(uid);
            PlayMusic(uid, component);
        }
    }

    private void OnShutdown(EntityUid uid, CaramelldansenComponent component, ComponentShutdown args)
    {
        if (_player.LocalEntity == uid && _overlay != null)
            _overlayMan.RemoveOverlay(_overlay);
        StopMusic();
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (_player.LocalEntity != null && EntityManager.TryGetComponent(_player.LocalEntity, out CaramelldansenComponent? comp))
        {
            UpdateOverlayFromComponent(_player.LocalEntity.Value);
        }
    }
}
