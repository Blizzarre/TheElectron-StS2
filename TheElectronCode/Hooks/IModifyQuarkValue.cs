using MegaCrit.Sts2.Core.Entities.Players;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Hooks;

public interface IModifyQuarkValueAdditive
{
    public decimal ModifyQuarkValueAdditive(QuarkModel quark, decimal value);
}

public interface IModifyQuarkValueMultAdd
{
    public decimal ModifyQuarkValueMultAdd(QuarkModel quark, decimal mult);
}

public interface IModifyQuarkValueMult
{
    public decimal ModifyQuarkValueMult(QuarkModel quark, decimal mult);
}