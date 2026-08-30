using System.Text;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using TheElectron.TheElectronCode.Character;
using TheElectron.TheElectronCode.Entities;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Models;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Nodes.Quarks;

public partial class NQuarkManager : NClickableControl
{
    private static Color DarkenedColor => new("a0a0a0");

    private Control? _allContainer;

    private Control? _atomContainer;

    private Control? _quarkContainer;

    private Control? _fuseStats;

    private Control _bounds = null!;

    private readonly List<NFuseStat> _fuseStatList = [];

    private readonly List<NQuark> _quarks = [];

    private Marker2D _centerMarker = null!;

    private readonly List<Marker2D> _quarkTargets = [];

    private NCreature? _creatureNode;

    private NSelectionReticle _selectionReticle = null!;

    private TextureRect _quarkBg = null!;

    private ElectronNParticlesContainer _particlesContainer = null!;

    private const float MinRadius = 24f;

    private const float MaxRadius = 80f;

    private const float ExternalRadiusOffset = 48f;

    private const float BaseRotationSpeed = Mathf.Pi / 8;

    private const float FusingRotationSpeed = Mathf.Pi * 3f;

    private const int StatHeight = 36;

    private Tween? _curTween;

    private Tween? _curStatsTween;

    private Tween? _curFusionTween;


    public static readonly Vector2 CenterOffset = new(0f, -50f);

    private static string ScenePath => ElectronResource.NQuarkManagerPath;

    public bool IsLocal { get; private set; }

    private bool IsQuarkFocused { get; set; }

    private Player Player => _creatureNode?.Entity.Player ?? throw new Exception("QuarkManager does not have a Player");

    private FastNoiseLite _noise = new();

    private float _curNoiseAmplitude = 0;

    private float _curNoiseOffset;

    private bool _hasInitialized = false;

    private float _curRotationSpeed;

    private float _curFuseRotationSpeed;

    private bool _isFusing;

    private HashSet<QuarkModel.FuseStat> _fusingStats = [];

    private static readonly float LerpRate = Mathf.Exp(2);

    private static readonly QuarkModel.FuseStat[] StatOrder =
        [QuarkModel.FuseStat.Damage, QuarkModel.FuseStat.Block, QuarkModel.FuseStat.Draw, QuarkModel.FuseStat.Energy];


    public override void _Ready()
    {
        ConnectSignals();

        Visible = false;

        _noise.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
        _noise.Frequency = 10f;
        _noise.Seed = Rng.Chaotic.NextInt();

        _allContainer = GetNode<Control>("%AllContainer");
        _atomContainer = GetNode<Control>("%AtomContainer");
        _quarkContainer = GetNode<Control>("%Quarks");
        _centerMarker = GetNode<Marker2D>("%CenterMarker");
        _bounds = GetNode<Control>("%Bounds");
        _quarkBg = GetNode<TextureRect>("%QuarkBg");
        _fuseStats = GetNode<Control>("%FuseStats");
        _particlesContainer = GetNode<ElectronNParticlesContainer>("%ParticlesContainer");

        for (var i = 1; i <= 4; i++) _fuseStatList.Add(GetNode<NFuseStat>($"%FuseStat{i}"));
        for (var i = 0; i < 4; i++)
        {
            _fuseStatList[i].SetStatVisual(StatOrder[i]);
        }

        _selectionReticle = BaseSceneIndex.SelectionReticleScene.Instantiate<NSelectionReticle>();
        this.AddChildSafely(_selectionReticle);
        _selectionReticle.Size = new Vector2(240, 160);
        _selectionReticle.Position = new Vector2(-80, -80);
        _selectionReticle.PivotOffset = new Vector2(80, 80);

        _allContainer.Modulate = Colors.Transparent;

        InitAnim();
    }

    public override void _Process(double delta)
    {
        if (!_hasInitialized) return;

        // TODO change rotation speed based on Spin
        var targetSpeed = BaseRotationSpeed + (_isFusing ? _curFuseRotationSpeed : 0);
        if (IsQuarkFocused && !_isFusing)
        {
            targetSpeed = 0;
        }

        var weight = 1 - (float)Mathf.Exp(-LerpRate * delta);
        _curRotationSpeed = Mathf.Lerp(_curRotationSpeed, targetSpeed, weight);
        _centerMarker.Rotation += (float)delta * _curRotationSpeed;
        
        if (_isFusing)
        {
            _curNoiseOffset += (float)delta;
            var noiseOffset = new Vector2(_noise.GetNoise2D(-100f, _curNoiseOffset),
                _noise.GetNoise2D(100f, _curNoiseOffset));
            _atomContainer?.Position = noiseOffset * _curNoiseAmplitude;
        }

        SetQuarkPositions();
    }

    private void SetQuarkPositions()
    {
        var capacity = Player.PlayerCombatState?.GetQuarkQueue()?.Capacity ?? 0;
        for (var i = 0; i < capacity; i++)
        {
            _quarks[i].GlobalPosition = _quarkTargets[i].GlobalPosition;
        }
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
        CombatManager.Instance.CombatSetUp += OnCombatSetup;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
        CombatManager.Instance.CombatSetUp -= OnCombatSetup;
    }

    private void InitAnim()
    {
        if (_hasInitialized) return;
        var capacity = Player.PlayerCombatState?.GetQuarkQueue()?.Capacity ?? 0;
        if (capacity > 0)
        {
            Visible = true;
            _hasInitialized = true;

            var radius = Mathf.Lerp(MinRadius, MaxRadius, (float)capacity / QuarkQueue.MaxCapacity);
            SetBgSize(radius + ExternalRadiusOffset);

            var tween = CreateTween().Parallel();
            tween.TweenProperty(_allContainer, "scale", Vector2.One, 0.35).From(Vector2.Zero)
                .SetEase(Tween.EaseType.InOut)
                .SetTrans(Tween.TransitionType.Sine);
            tween.TweenProperty(_allContainer, "position", new Vector2(0, MinRadius - radius), 0.35).From(Vector2.Zero)
                .SetEase(Tween.EaseType.InOut)
                .SetTrans(Tween.TransitionType.Sine);
            tween.TweenProperty(_allContainer, "modulate:a", 1, 0.35).From(0);
        }
    }

    public static NQuarkManager Create(NCreature creature, bool isLocal)
    {
        if (creature.Entity.Player == null)
            throw new InvalidOperationException("NQuarkManager can only be applied to player creatures");

        var nQuarkManager = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NQuarkManager>();
        nQuarkManager._creatureNode = creature;
        nQuarkManager.IsLocal = isLocal;
        return nQuarkManager;
    }

    private void OnCombatSetup(CombatState _)
    {
        if (!Player.Creature.IsAlive || Player.PlayerCombatState == null) return;

        var quarkQueue = Player.PlayerCombatState.GetQuarkQueue();
        if (quarkQueue != null)
            AddSlotAnim(quarkQueue.Capacity);
    }

    private void OnCombatStateChanged(CombatState _)
    {
        // TODO update numbers
    }

    public void AddSlotAnim(int amount)
    {
        InitAnim();

        for (var i = 0; i < amount; i++)
        {
            var nQuark = NQuark.Create(IsLocal);
            nQuark.OnFocusChanged += OnQuarkFocusChanged;
            var marker = new Marker2D();
            _centerMarker.AddChildSafely(marker);
            _quarkTargets.Add(marker);
            _quarkContainer?.AddChildSafely(nQuark);
            _quarks.Add(nQuark);
            nQuark.Position = Vector2.Zero;
        }

        TweenLayout();
        // TODO Controller navigation
    }

    public void RemoveSlotAnim(int amount)
    {
        if (amount > _quarks.Count)
        {
            throw new InvalidOperationException("There are not enough slots to remove.");
        }

        for (var i = 0; i < amount; i++)
        {
            var nQuark = _quarks.Last();
            nQuark.QueueFreeSafely();
            _quarks.Remove(nQuark);

            var marker = _quarkTargets.Last();
            marker.QueueFreeSafely();
            _quarkTargets.Remove(marker);

            // TODO Remove ui focus
        }

        TweenLayout();
        // TODO Controller navigation
    }

    public void AddQuarkAnim()
    {
        var quarkModel = Player.PlayerCombatState?.GetQuarkQueue()?.Quarks.LastOrDefault();
        var emptyQuark = _quarks.FirstOrDefault(q => q.Model == null); // find first empty slot
        if (emptyQuark == null)
            throw new InvalidOperationException(
                "There is no empty slot for adding new Quark."); // Quarks must be fused if full first before adding new Quarks

        var newQuark = NQuark.Create(IsLocal, quarkModel);
        newQuark.OnFocusChanged += OnQuarkFocusChanged;

        emptyQuark.AddSiblingSafely(newQuark);
        _quarks.Insert(_quarks.IndexOf(emptyQuark), newQuark);
        newQuark.Position = emptyQuark.Position;

        _quarkContainer?.RemoveChildSafely(emptyQuark);
        _quarks.Remove(emptyQuark);
        emptyQuark.QueueFreeSafely();

        TweenLayout();
        // TODO Controller navigation
        UpdateVisuals();
    }

    public void RemoveQuarkAnim(QuarkModel quark)
    {
        var removeQuark = _quarks.Last(n => n.Model == quark);
        var tween = CreateTween();
        _quarks.Remove(removeQuark);

        tween.TweenProperty(removeQuark, "modulate:a", 0, 0.25);
        tween.Chain().TweenCallback(Callable.From(removeQuark.QueueFreeSafely));
        var emptyQuark = NQuark.Create(IsLocal);
        emptyQuark.OnFocusChanged += OnQuarkFocusChanged;
        _quarkContainer?.AddChildSafely(emptyQuark);
        _quarks.Add(emptyQuark);
        emptyQuark.Position = removeQuark.Position;

        if (removeQuark.HasFocus())
        {
            _creatureNode?.Hitbox.TryGrabFocus();
        }

        TweenLayout();
        // TODO Controller navigation
        UpdateVisuals();
    }


    public async Task BeginFuseQuarksAnim(HashSet<QuarkModel.FuseStat> stats)
    {
        _fusingStats = stats;
        _isFusing = true;
        _noise.Seed = Rng.Chaotic.NextInt();
        _curNoiseOffset = 0;

        foreach (var quark in _quarks)
        {
            quark.DisableHover();
        }

        OnUnfocus();

        var duration = SaveManager.Instance.PrefsSave.FastMode != FastModeType.Normal ? 0.2f : 0.35f;

        _curFuseRotationSpeed = 0;

        _curFusionTween?.Kill();
        _curFusionTween = CreateTween().SetParallel();

        // Spin up animation
        _curFusionTween.TweenProperty(_atomContainer, "modulate", new Color(3f, 3f, 3f), duration);
        _curFusionTween.TweenProperty(_atomContainer, "scale", new Vector2(0.5f, 0.5f), duration)
            .SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quad);
        _curFusionTween.TweenProperty(this, "_curFuseRotationSpeed", FusingRotationSpeed, duration);
        _curFusionTween.TweenProperty(this, "_curNoiseAmplitude", 10, duration);
        
        while (!await WaitAndInterruptIfNecessary(duration))
        {
            return;
        }
    }

    public async Task StepFuseQuarksAnim(QuarkModel.FuseStat stat)
    {
        _curFusionTween?.Kill();
        _curFusionTween = CreateTween().SetParallel();

        var duration = SaveManager.Instance.PrefsSave.FastMode != FastModeType.Normal ? 0.15f : 0.3f;

        if (_fusingStats.Remove(stat))
        {
            var items = _fusingStats.Count;
            var height = (StatHeight * items) + (items - 1) * 6;
            var initialPosition = new Vector2(0, -height / 2f);

            for (var i = 0; i < 4; i++)
            {
                var fuseStat = _fuseStatList[i];

                if (_fusingStats.Contains(StatOrder[i])) // Reorder remaining stats
                {
                    _curFusionTween.TweenProperty(fuseStat, "position", initialPosition, duration)
                        .SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
                    _curFusionTween.TweenProperty(fuseStat, "modulate:a", 1, duration);
                    initialPosition.Y += StatHeight + 6;
                }
                else if (stat == StatOrder[i]) // Move the current stat being used out
                {
                    _curFusionTween.TweenProperty(fuseStat, "position:x", fuseStat.Position.X + 60f, duration)
                        .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Quad);
                    _curFusionTween.TweenProperty(fuseStat, "modulate:a", 0, duration);
                    _curFusionTween.Chain().TweenProperty(fuseStat, "position:x", fuseStat.Position.X, 0); // return to starting position
                }
            }
        }

        _particlesContainer.PlayOneShot();

        // Last stat has been removed -> Fade out all Quarks and slots
        if (_fusingStats.Count == 0)
        {
            var tween = CreateTween().SetParallel();

            foreach (var quark in _quarks)
            {
                tween.TweenProperty(quark, "modulate:a", 0, duration);
            }
        }

        while (!await WaitAndInterruptIfNecessary(duration))
        {
            return;
        }
    }

    public void EndFuseQuarksAnim()
    {
        _curFusionTween = CreateTween().SetParallel();

        var duration = 0.5;

        var capacity = Player.PlayerCombatState?.GetQuarkQueue()?.Capacity ?? 0;

        foreach (var quark in _quarks)
        {
            quark.QueueFreeSafely();
        }
        _quarks.Clear();

        for (var i = 0; i < capacity; i++)
        {
            var emptyQuark = NQuark.Create(IsLocal);
            emptyQuark.OnFocusChanged += OnQuarkFocusChanged;
            _quarkContainer?.AddChildSafely(emptyQuark);
            _quarks.Add(emptyQuark);
        }

        // Spin down animation
        _curFusionTween.TweenProperty(_atomContainer, "modulate", new Color(1f, 1f, 1f), duration);
        _curFusionTween.TweenProperty(_atomContainer, "scale", Vector2.One, duration)
            .SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
        _curFusionTween.TweenProperty(this, "_curFuseRotationSpeed", FusingRotationSpeed, duration)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);;
        _curFusionTween.TweenProperty(this, "_curNoiseAmplitude", 0, duration);
        _curFusionTween.TweenCallback(Callable.From(() => { _isFusing = false; }));

        _fusingStats.Clear();
        
        TweenLayout();
    }

    private void OnQuarkFocusChanged()
    {
        IsQuarkFocused = _quarks.Any(q => q.IsFocused);
    }

    private void TweenLayout()
    {
        var capacity = Player.PlayerCombatState?.GetQuarkQueue()?.Capacity ?? 0;

        var radius = Mathf.Lerp(MinRadius, MaxRadius, (float)capacity / QuarkQueue.MaxCapacity);
        var extRadius = radius + ExternalRadiusOffset;

        _curTween?.Kill();
        _curTween = CreateTween().SetParallel();

        if (capacity > 1)
        {
            var scaling = (float)Mathf.Lerp(1, 0.8, (Mathf.Max(3, capacity) - 3) / 7d);

            // Spread the targets positions around the 'circle'
            var step = 2f * Mathf.Pi / capacity;

            for (var i = 0; i < capacity; i++)
            {
                var pos = Vector2.Right.Rotated(i * step) * radius;
                _curTween.TweenProperty(_quarkTargets[i], "position", pos, 0.35).SetEase(Tween.EaseType.InOut)
                    .SetTrans(Tween.TransitionType.Sine);
                _curTween.TweenProperty(_quarks[i], "scale", new Vector2(scaling, scaling), 0.35)
                    .SetEase(Tween.EaseType.InOut)
                    .SetTrans(Tween.TransitionType.Sine);
            }
        }
        else if (capacity == 1)
        {
            _curTween.TweenProperty(_quarkTargets[0], "position", Vector2.Zero, 0.35).SetEase(Tween.EaseType.InOut)
                .SetTrans(Tween.TransitionType.Sine);
            _curTween.TweenProperty(_quarks[0], "scale", Vector2.One, 0.35).SetEase(Tween.EaseType.InOut)
                .SetTrans(Tween.TransitionType.Sine);
        }

        _curTween.TweenMethod(Callable.From<float>(SetBgSize), _quarkBg.Size.X / 2, extRadius, 0.35);
        _curTween.TweenProperty(_allContainer, "position", new Vector2(0, MinRadius - radius), 0.35)
            .SetEase(Tween.EaseType.InOut)
            .SetTrans(Tween.TransitionType.Sine);

        UpdateAndTweenStats();
    }

    private void UpdateAndTweenStats()
    {
        var capacity = Player.PlayerCombatState?.GetQuarkQueue()?.Capacity ?? 0;
        var extRadius = Mathf.Lerp(MinRadius, MaxRadius, (float)capacity / QuarkQueue.MaxCapacity) +
                        ExternalRadiusOffset;

        _curStatsTween?.Kill();
        _curStatsTween = CreateTween().SetParallel();

        _curStatsTween.TweenProperty(_fuseStats, "position:x", extRadius + 10, 0.35)
            .SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);

        var stats = GetStats();
        var items = stats.Count;
        var height = (StatHeight * items) + (items - 1) * 6;
        var initialPosition = new Vector2(0, -height / 2f);

        for (var i = 0; i < 4; i++)
        {
            var fuseStat = _fuseStatList[i];
            if (stats.TryGetValue(StatOrder[i], out var value))
            {
                fuseStat.SetStatNumber(value);
                _curStatsTween.TweenProperty(fuseStat, "position", initialPosition, 0.35)
                    .SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
                _curStatsTween.TweenProperty(fuseStat, "modulate:a", 1, 0.35);
                initialPosition.Y += StatHeight + 6;
            }
            else
            {
                _curStatsTween.TweenProperty(fuseStat, "position", new Vector2(0, -StatHeight / 2f), 0.35)
                    .SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
                _curStatsTween.TweenProperty(fuseStat, "modulate:a", 0, 0.35);
            }
        }

        _bounds.Size = new Vector2(80, height);
        _bounds.Position = new Vector2(extRadius + 6, -height / 2f);
        // Inputs are routed through _allContainer from _bounds
        _allContainer?.MouseFilter = items > 0 ? MouseFilterEnum.Pass : MouseFilterEnum.Ignore;
    }

    private Dictionary<QuarkModel.FuseStat, decimal> GetStats()
    {
        var ret = new Dictionary<QuarkModel.FuseStat, decimal>();
        foreach (var quark in _quarks.Select(nQuark => nQuark.Model).OfType<QuarkModel>()
                     .Where(quark => quark.Stat != QuarkModel.FuseStat.None))
        {
            ret[quark.Stat] = ret.GetValueOrDefault(quark.Stat, 0) + quark.Value;
        }

        return ret;
    }

    private void SetBgSize(float radius)
    {
        _quarkBg.Size = new Vector2(radius * 2, radius * 2);
        _quarkBg.Position = new Vector2(-radius, -radius);
    }

    public void UpdateVisuals()
    {
        if (!IsNodeReady() || !CombatManager.Instance.IsInProgress) return;
        if (_isFusing) return; // Keep the numbers locked during Fuse animation.
        
        foreach (var quark in _quarks)
        {
            quark.UpdateVisuals();
        }

        UpdateAndTweenStats();
    }

    protected override void OnFocus()
    {
        if(_isFusing) return;
        
        var nHoverTipSet =
            NHoverTipSet.CreateAndShow(_bounds, GetStatsHoverTip(), HoverTip.GetHoverTipAlignment(_bounds));
        nHoverTipSet?.SetExtraFollowOffset(new Vector2(0, -12));
        nHoverTipSet?.SetFollowOwner();

        SelfModulate = Colors.White;

        if (!NControllerManager.Instance?.IsUsingDirectionalNavigation ?? false)
            return;
        _selectionReticle.OnSelect();
    }

    private HoverTip GetStatsHoverTip()
    {
        var stats = GetStats();
        var prefix = GetQuarkOwnerPool().EnergyColorName;
        var locStringTitle = new LocString("static_hover_tips", "THEELECTRON-FUSION_STATS.title");
        var locStringDesc = new LocString("static_hover_tips", "THEELECTRON-FUSION_STATS.description");

        List<string> appendDesc = [];
        if (stats.ContainsKey(QuarkModel.FuseStat.Damage))
        {
            var locString = new LocString("static_hover_tips", "THEELECTRON-FUSION_STATS.damage");
            locString.Add("Damage", stats.GetValueOrDefault(QuarkModel.FuseStat.Damage));
            appendDesc.Add(locString.GetFormattedText());
        }
        if (stats.ContainsKey(QuarkModel.FuseStat.Block))
        {
            var locString = new LocString("static_hover_tips", "THEELECTRON-FUSION_STATS.block");
            locString.Add("Block", stats.GetValueOrDefault(QuarkModel.FuseStat.Block));
            appendDesc.Add(locString.GetFormattedText());
        }
        if (stats.ContainsKey(QuarkModel.FuseStat.Draw))
        {
            var locString = new LocString("static_hover_tips", "THEELECTRON-FUSION_STATS.draw");
            locString.Add("Draw", stats.GetValueOrDefault(QuarkModel.FuseStat.Draw));
            appendDesc.Add(locString.GetFormattedText());
        }
        if (stats.ContainsKey(QuarkModel.FuseStat.Energy))
        {
            var locString = new LocString("static_hover_tips", "THEELECTRON-FUSION_STATS.energy");
            locString.Add(new EnergyVar((int)stats.GetValueOrDefault(QuarkModel.FuseStat.Energy)){ColorPrefix = prefix});
            appendDesc.Add(locString.GetFormattedText());
        }
        
        locStringDesc.Add("EffectsString", string.Join("\n", appendDesc));

        var hoverTip = new HoverTip(locStringTitle, locStringDesc);
        return hoverTip;
    }

    protected override void OnUnfocus()
    {
        SelfModulate = IsLocal ? Colors.White : DarkenedColor;
        NHoverTipSet.Remove(_bounds);
        
        _selectionReticle.OnDeselect();
    }

    public void ClearQuarks()
    {
        _curFusionTween?.Kill();
        _curStatsTween?.Kill();

        _curTween?.Kill();

        _curTween = CreateTween().SetParallel();

        _curTween.TweenProperty(this, "modulate:a", 0, 0.35);
        _curTween.TweenProperty(this, "scale", Vector2.Zero, 0.35);

        foreach (var quark in _quarks)
            _curTween.Chain().SetParallel().TweenCallback(Callable.From(quark.QueueFreeSafely));

        _quarks.Clear();
        
        SetProcess(false);
    }

    private async Task<bool> WaitAndInterruptIfNecessary(float seconds)
    {
        var currTime = 0f;
        while (currTime <= seconds)
        {
            if (!IsInsideTree()) return false;
            currTime += await this.AwaitProcessFrame();
        }

        return true;
    }
    
    private IPoolModel GetQuarkOwnerPool()
    {
        return Player.Character.CardPool;
    }
}