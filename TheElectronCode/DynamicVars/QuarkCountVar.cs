using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TheElectron.TheElectronCode.DynamicVars;

public class QuarkCountVar : DynamicVar
{
    public const string defaultName = "QuarkCount";

    public QuarkCountVar(int count)
        : this(defaultName, count)
    {
    }

    public QuarkCountVar(string name, int count)
        : base(name, count)
    {
    }
}