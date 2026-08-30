using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using TheElectron.TheElectronCode.Models;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Nodes.Quarks;

public partial class NQuark : NClickableControl
{
    private static string ScenePath => ElectronResource.NQuarkPath;
    
    private Control _labelContainer = null!;
    
    private Control _bounds = null!;
    
    private NSelectionReticle _selectionReticle = null!;

    private MegaLabel _label = null!;

    private Sprite2D _outline = null!;
    
    private Sprite2D _stableOutline = null!;
    
    private Control _visualContainer = null!;
    
    private NQuarkVisuals? _sprite;
    
    private Tween? _curTween;
    
    private bool _isLocal;

    public bool IsFocused
    {
        get;
        private set
        {
            field = value;
            OnFocusChanged?.Invoke();
        }
    }

    public Action? OnFocusChanged;
    
    public QuarkModel? Model { get; private set; }

    public static NQuark Create(bool isLocal)
    {
        var nQuark = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NQuark>();
        nQuark._isLocal = isLocal;
        return nQuark;
    }
    
    public static NQuark Create(bool isLocal, QuarkModel? model)
    {
        var nQuark = Create(isLocal);
        nQuark.Model = model;

        return nQuark;
    }

    public override void _Ready()
    {
        ConnectSignals();
        
        _bounds = GetNode<Control>("%Bounds");
        _labelContainer = GetNode<Control>("%LabelContainer");
        _outline = GetNode<Sprite2D>("%Outline");
        _stableOutline = GetNode<Sprite2D>("%StableOutline");
        _visualContainer = GetNode<Control>("%VisualContainer");
        
        _label = CreateLabel(FontColors.DefaultFontColor);
        _labelContainer.AddChildSafely(_label);
        
        _selectionReticle = BaseSceneIndex.SelectionReticleScene.Instantiate<NSelectionReticle>();
        this.AddChildSafely(_selectionReticle);
        _selectionReticle.Size = new Vector2(80, 80);
        _selectionReticle.Position = new Vector2(-40, -40);
        _selectionReticle.PivotOffset = new Vector2(40, 40);
        
        CreateTween().TweenProperty(_outline, "scale", _outline.Scale, 0.25).From(Vector2.Zero);
        
        UpdateVisuals();
    }
    
    private static MegaLabel CreateLabel((Color, Color, Color) fontColor)
    {
        var label = new MegaLabel();
        label.MaxFontSize = 24;
        label.AutoSizeEnabled = false;
        label.HorizontalAlignment = HorizontalAlignment.Left;
        label.VerticalAlignment = VerticalAlignment.Top;
        label.AddThemeColorOverride("font_color", fontColor.Item1);
        label.AddThemeColorOverride("font_shadow_color", fontColor.Item2);
        label.AddThemeColorOverride("font_outline_color", fontColor.Item3);
        label.AddThemeConstantOverride("shadow_offset_x", 3);
        label.AddThemeConstantOverride("shadow_offset_y", 3);
        label.AddThemeConstantOverride("outline_size", 13);
        label.AddThemeConstantOverride("shadow_outline_size", 0);
        label.AddThemeFontOverride("font", BaseResourceIndex.FontKreonBoldShared);
        label.AddThemeFontSizeOverride("font_size", 24);
        label.Text = "";

        return label;
    }

    public void UpdateVisuals()
    {
        if (!IsNodeReady() || !CombatManager.Instance.IsInProgress) return;

        if (Model == null)
        {
            _sprite?.QueueFreeSafely();
            _label.Visible = false;
            _stableOutline.Visible = false;
            return;
        }

        if (_sprite == null)
        {
            _sprite = Model.CreateSprite();
            _visualContainer.AddChildSafely(_sprite);
            _sprite.Position = Vector2.Zero;
            _label.Visible = Model.ShowLabel;
            _curTween?.Kill();
            _curTween = CreateTween().SetParallel();
            _curTween.TweenProperty(_sprite, "scale", Vector2.One, 0.35).From(Vector2.Zero)
                .SetTrans(Tween.TransitionType.Back)
                .SetEase(Tween.EaseType.Out);
            _curTween.TweenProperty(_stableOutline, "scale", new Vector2(0.5f, 0.5f), 0.35).From(Vector2.Zero)
                .SetTrans(Tween.TransitionType.Back)
                .SetEase(Tween.EaseType.Out);
        }

        _outline.Visible = false;
        _stableOutline.Visible = Model.IsStable;
        _stableOutline.Modulate = Model.HasStableSlot ? new Color("fff03b") : new Color("7d7620");
        _labelContainer.Visible = _isLocal;
        if (!_isLocal) Modulate = Model.DarkenedColor;

        var text = Model.Value.ToString("0");
        _label.SetTextAutoSize(text);
    }

    public void DisableHover()
    {
        _bounds.MouseFilter = MouseFilterEnum.Ignore;
        OnUnfocus();
    }
    
    protected override void OnFocus()
    {
        if (Model == null && !_isLocal) return;
        IsFocused = true;
        var hoverTips = Model?.HoverTips ?? new List<IHoverTip> { QuarkModel.EmptySlotHoverTip };
        var nHoverTipSet = NHoverTipSet.CreateAndShow(_bounds, hoverTips, HoverTip.GetHoverTipAlignment(_bounds));
        nHoverTipSet?.SetFollowOwner();
        _labelContainer.Visible = true;
        Modulate = Colors.White;
        if (!NControllerManager.Instance?.IsUsingDirectionalNavigation ?? false)
            return;
        _selectionReticle.OnSelect();
    }

    protected override void OnUnfocus()
    {
        IsFocused = false;
        _labelContainer.Visible = _isLocal;
        if (Model != null) Modulate = _isLocal ? Colors.White : Model.DarkenedColor;

        NHoverTipSet.Remove(_bounds);
        _selectionReticle.OnDeselect();
    }
}