using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Nodes;

public partial class NFaradCounter : Control
{
    private static readonly StringName V = new("v");
    private static readonly StringName S = new("s");
    private static readonly StringName YScrollSpeed = new("y_scroll_speed");

    private Player? _player;

    private TextureRect _icon = null!;

    private ShaderMaterial _hsv = null!;

    private int _displayedFaradCount;

    private float _lerpingCount;

    private float _velocity;

    private MegaLabel _label = null!;

    private Tween? _hsvTween;

    private bool _isListeningToCombatState;

    private HoverTip _hoverTip;

    public void Initialize(Player player)
    {
        _player = player;
        ConnectFaradChangedSignal();
        RefreshVisibility();
    }

    public override void _Ready()
    {
        _icon = GetNode<TextureRect>("%Icon");
        _hsv = (ShaderMaterial)_icon.Material;
        _label = CreateLabel(FontColors.LightBlueFontColor);
        _label.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(_label);

        _hoverTip = new HoverTip(new LocString("static_hover_tips", "THEELECTRON-FARAD_COUNT.title"),
            new LocString("static_hover_tips", "THEELECTRON-FARAD_COUNT.description"));
        Connect(Control.SignalName.MouseEntered, Callable.From(OnHovered));
        Connect(Control.SignalName.MouseExited, Callable.From(OnUnhovered));
        SetFaradCountText(0, true);
        Visible = false;
    }

    // TODO move this to utils class
    private static MegaLabel CreateLabel((Color, Color, Color) fontColor)
    {
        var label = new MegaLabel();
        label.MaxFontSize = 32;
        label.AutoSizeEnabled = false;
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.VerticalAlignment = VerticalAlignment.Center;
        label.AddThemeColorOverride("font_color", fontColor.Item1);
        label.AddThemeColorOverride("font_shadow_color", fontColor.Item2);
        label.AddThemeColorOverride("font_outline_color", fontColor.Item3);
        label.AddThemeConstantOverride("shadow_offset_x", 3);
        label.AddThemeConstantOverride("shadow_offset_y", 3);
        label.AddThemeConstantOverride("outline_size", 15);
        label.AddThemeConstantOverride("shadow_outline_size", 15);
        label.AddThemeFontOverride("font", BaseResourceIndex.FontKreonBoldSpaceTwo);
        label.AddThemeFontSizeOverride("font_size", 28);
        label.Text = "0";

        return label;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        ConnectFaradChangedSignal();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_player == null || !_isListeningToCombatState) return;
        var electronCombatState = _player.PlayerCombatState?.Electron();
        electronCombatState?.FaradChanged -= OnFaradChanged;
        _isListeningToCombatState = false;
    }

    private void ConnectFaradChangedSignal()
    {
        if (_player != null && !_isListeningToCombatState)
        {
            var electronCombatState = _player.PlayerCombatState?.Electron();
            electronCombatState?.FaradChanged += OnFaradChanged;
            _isListeningToCombatState = true;
        }
    }

    private void OnHovered()
    {
        var nHoverTipSet = NHoverTipSet.CreateAndShow(this, _hoverTip);
        nHoverTipSet?.GlobalPosition = GlobalPosition + new Vector2(84, 0);
    }

    private void OnUnhovered()
    {
        NHoverTipSet.Remove(this);
    }

    private void OnFaradChanged(int oldFarad, int newFarad)
    {
        UpdateFaradCount(oldFarad, newFarad);
        RefreshVisibility();
    }

    private void UpdateFaradCount(int oldCount, int newCount)
    {
        if (newCount < oldCount)
        {
            _hsvTween?.Kill();
            _hsv.SetShaderParameter(V, 1f);
            _lerpingCount = newCount;
            SetFaradCountText(newCount);
        }
        else if (newCount > oldCount)
        {
            _hsvTween?.Kill();
            _hsvTween = CreateTween();
            _hsvTween.TweenMethod(Callable.From<float>(UpdateShaderV), 1f, 2f, 0.1f);
            _hsvTween.TweenMethod(Callable.From<float>(UpdateShaderV), 2f, 1f, 0.5f);
            //TODO vfx gain Farad
        }
    }

    private void SetFaradCountText(int count, bool initSetup = false)
    {
        if (!initSetup && _displayedFaradCount == count) return;

        _displayedFaradCount = count;

        _label.AddThemeColorOverride(ThemeConstants.Label.FontColor,
            count == 0 ? StsColors.red : FontColors.LightBlueFontColor.Item1);
        _label.SetTextAutoSize(count.ToString());

        if (count == 0)
        {
            _hsv.SetShaderParameter(S, 0.5f);
            _hsv.SetShaderParameter(V, 0.85f);
            _hsv.SetShaderParameter(YScrollSpeed, -0.08f);
        }
        else
        {
            _hsv.SetShaderParameter(S, 1f);
            _hsv.SetShaderParameter(V, 1f);
            _hsv.SetShaderParameter(YScrollSpeed, Mathf.Lerp(-0.16, -0.32, Mathf.Min((count - 1)/20.0, 1.0)));
        }
    }

    public override void _Process(double delta)
    {
        if (_player == null) return;
        var farad = GetPlayerFarad(_player);

        _lerpingCount =
            MathHelper.SmoothDamp(_lerpingCount, farad, ref _velocity, 0.1f, (float)delta);
        SetFaradCountText(Mathf.RoundToInt(_lerpingCount));
    }

    private static int GetPlayerFarad(Player player)
    {
        var farad = player.PlayerCombatState?.GetFarad() ?? 0;
        return farad;
    }


    private void UpdateShaderV(float value)
    {
        _hsv.SetShaderParameter(V, value);
    }

    private void RefreshVisibility()
    {
        if (_player == null)
        {
            Visible = false;
            return;
        }

        var farad = GetPlayerFarad(_player);

        var shouldAlwaysShowFarad = _player.Character is Character.TheElectron;

        Visible = Visible || shouldAlwaysShowFarad || farad > 0;
    }
}