using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace TheElectron.TheElectronCode.Utils;

public class ElectronKeywords
{
    [CustomEnum, KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Drain;
}

public class ElectronEnums
{
    [CustomEnum] public static CardCostColor CostColorDrain;
    [CustomEnum] public static CardCostColor CostColorEmpty;
}

public class ElectronTags
{
    [CustomEnum] public static CardTag Empty;
    [CustomEnum] public static CardTag Deplete;
}