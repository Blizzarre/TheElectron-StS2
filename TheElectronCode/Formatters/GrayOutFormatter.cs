using MegaCrit.Sts2.Core.Localization;
using SmartFormat.Core.Extensions;
using TheElectron.TheElectronCode.Extensions;

namespace TheElectron.TheElectronCode.Formatters;

// Credit to Mangochicken for the help with this
public class GrayOutFormatter : IFormatter
{
    public string Name { get; set; } = "eGrayOut";
    public bool CanAutoDetect { get; set; }
    
    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        // Gray when false. 
        // Usage: {Var:IsGrayOut:TextWithFormatting}
        if (formattingInfo.CurrentValue is not bool value) { return false; }

        if (value)
        {
            formattingInfo.FormatAsChild(formattingInfo.Format!, formattingInfo.CurrentValue);
        }
        else
        {
            // TODO allow configuring color with formatter option
            // We reformat by calling the smartFormatter.Format and then stripping color tags from the resulting text
            formattingInfo.Write("[color=dim_gray]");
            var rawStr = string.Join("", formattingInfo.Format!.Items.Select(i => i.RawText));
            var formattedStr = LocManager._smartFormatter.Format(formattingInfo.FormatDetails.Provider, rawStr,
                formattingInfo.FormatDetails.OriginalArgs);
            formattingInfo.Write(formattedStr.StripColorBbCodes());
            formattingInfo.Write("[/color]");
        }

        return true;
    }
}