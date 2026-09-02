using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.HoverTips;

public class ElectronHoverTipFactory
{
    public static IHoverTip Static(ElectronHoverTip tip, Action<LocString>? locAdd = null, params DynamicVar[] vars)
    {
        var text = tip.GetType().GetPrefix() + StringHelper.Slugify(tip.ToString());
        return Static(text, locAdd, vars);
    }

    public static IHoverTip Static(string entry, Action<LocString>? locAdd = null, params DynamicVar[] vars)
    {
        return Static(entry, null, locAdd, vars);
    }

    public static IHoverTip Static(string entry, Texture2D? icon, Action<LocString>? locAdd = null,
        params DynamicVar[] vars)
    {
        var locString = L10NStatic(entry + ".title");
        var locString2 = L10NStatic(entry + ".description");
        foreach (var dynamicVar in vars)
        {
            locString.Add(dynamicVar);
            locString2.Add(dynamicVar);
        }

        locAdd?.Invoke(locString);
        locAdd?.Invoke(locString2);

        return new HoverTip(locString, locString2, icon);
    }

    private static LocString L10NStatic(string entry)
    {
        return new LocString("static_hover_tips", entry);
    }

    public static IHoverTip FromQuark<T>() where T : QuarkModel
    {
        QuarkModel model = ModelDb.Get<T>();
        return model.DumbHoverTip;
    }


    public static HoverTip CreateQuarkHoverTip(QuarkModel quark, LocString description)
    {
        var hoverTip = new HoverTip
        {
            IsSmart = false,
            IsDebuff = false,
            IsInstanced = false,
            CanonicalModel = null,
            ShouldOverrideTextOverflow = false,
            Id = quark.Id.ToString(),
            Title = quark.Title.GetFormattedText(),
            Description = description.GetFormattedText(),
            Icon = quark.Icon
        };
        return hoverTip;
    }
}