using Godot;

namespace TheElectron.TheElectronCode.Utils;

public class FontColors
{
    // Font color stored as tuple: item1 - font_color; item2 - font_shadow_color; item3 - font_outline_color
    public static readonly (Color, Color, Color) DefaultFontColor =
        (new Color("fff6e2"), new Color("00000040"), new Color("333333e6"));


    public static readonly (Color, Color, Color) LightBlueFontColor = (new Color("d6f3ff"), new Color("00000030"),
        new Color("2b4a7f"));
}