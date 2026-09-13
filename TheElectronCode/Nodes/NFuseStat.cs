using Godot;
using MegaCrit.Sts2.addons.mega_text;
using TheElectron.TheElectronCode.Models;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Nodes;

public partial class NFuseStat : HBoxContainer
{
    private static readonly Dictionary<QuarkModel.FuseStat, bool> CombinedDisplay = new()
    {
        { QuarkModel.FuseStat.Damage, false },
        { QuarkModel.FuseStat.Block, true },
        { QuarkModel.FuseStat.Energy, true },
        { QuarkModel.FuseStat.Draw, true },
        { QuarkModel.FuseStat.SelfDamage, false },
    };
    
    private TextureRect? _statIcon;

    private MegaLabel? _label;

    private QuarkModel.FuseStat _fuseStat;

    public override void _Ready()
    {
        Modulate = Colors.Transparent;

        _statIcon = GetNode<TextureRect>("%StatIcon");

        _label = CreateLabel(FontColors.DefaultFontColor);
        AddChild(_label);
    }

    public void SetStatVisual(QuarkModel.FuseStat stat)
    {
        _fuseStat = stat;
        var powerName = stat switch
        {
            QuarkModel.FuseStat.Damage => "strength_power",
            QuarkModel.FuseStat.Block => "dexterity_power",
            QuarkModel.FuseStat.Draw => "draw_cards_next_turn_power",
            QuarkModel.FuseStat.Energy => "energy_next_turn_power",
            QuarkModel.FuseStat.SelfDamage => "inferno_power",
            _ => ""
        };

        var path = $"res://images/atlases/power_atlas.sprites/{powerName}.tres";
        _statIcon?.Texture = ResourceLoader.Load<Texture2D>(path);
    }

    public void SetStatNumbers(IEnumerable<decimal> values)
    {
        if (CombinedDisplay[_fuseStat])
        {
            _label?.SetTextAutoSize(values.Sum().ToString("0"));
        }
        else
        {
            var valueList = values.ToList();
            // All values should be the same, but you never know...
            _label?.SetTextAutoSize(valueList.Count > 1
                ? $"{valueList.First():0}×{valueList.Count} ({valueList.Sum():0})"
                : $"{valueList.First():0}");
        }
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
}