using Godot;
using MegaCrit.Sts2.addons.mega_text;
using TheElectron.TheElectronCode.Models;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Nodes;

public partial class NFuseStat : HBoxContainer
{
    private TextureRect? _statIcon;

    private MegaLabel? _label;

    public override void _Ready()
    {
        Modulate = Colors.Transparent;
        
        _statIcon = GetNode<TextureRect>("%StatIcon");
        
        _label = CreateLabel(FontColors.DefaultFontColor);
        AddChild(_label);
    }

    public void SetStatVisual(QuarkModel.FuseStat stat)
    {
        string? powerName = null;
        switch (stat)
        {
            case QuarkModel.FuseStat.Damage:
                powerName = "strength_power";
                break;
            case QuarkModel.FuseStat.Block:
                powerName = "dexterity_power";
                break;
            case QuarkModel.FuseStat.Draw:
                powerName = "draw_cards_next_turn_power";
                break;
            case QuarkModel.FuseStat.Energy:
                powerName = "energy_next_turn_power";
                break;
        }

        var path = $"res://images/atlases/power_atlas.sprites/{powerName}.tres";
        _statIcon?.Texture = ResourceLoader.Load<Texture2D>(path);
    }

    public void SetStatNumber(decimal value)
    {
        _label?.SetTextAutoSize(value.ToString("0"));
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