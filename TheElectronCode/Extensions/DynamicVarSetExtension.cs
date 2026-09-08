using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheElectron.TheElectronCode.DynamicVars;

namespace TheElectron.TheElectronCode.Extensions;

public static class DynamicVarSetExtension
{
    extension(DynamicVarSet set)
    {
        public DynamicVar Farad => set[FaradVar.defaultName];

        public DynamicVar QuarkCount => set[QuarkCountVar.defaultName];
    }
}