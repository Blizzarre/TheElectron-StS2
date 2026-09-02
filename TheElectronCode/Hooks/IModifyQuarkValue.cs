using MegaCrit.Sts2.Core.Entities.Players;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Hooks;

public interface IModifyQuarkValueAdditive
{
    // must return the modified amount by adding
    public decimal ModifyQuarkValueAdditive(QuarkModel quark, decimal value);
}

public interface IModifyQuarkValueMultAdd
{
    // must return the modified amount by adding (mult starts at 1)
    public decimal ModifyQuarkValueMultAdd(QuarkModel quark, decimal mult);
}

public interface IModifyQuarkValueMult
{
    // must return the modified amount by multiplying (mult starts at 1)
    public decimal ModifyQuarkValueMult(QuarkModel quark, decimal mult);
}