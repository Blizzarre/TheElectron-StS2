using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheElectron.TheElectronCode.DynamicVars;

namespace TheElectron.TheElectronCode.Extensions;

public static class DynamicVarSetExtension
{
    public static DynamicVar Farad(this DynamicVarSet set)
    {
        return set[FaradVar.defaultName];
    }
    
    public static DynamicVar QuarkCount(this DynamicVarSet set)
    {
        return set[QuarkCountVar.defaultName];
    }
}