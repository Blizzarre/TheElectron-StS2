using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheElectron.TheElectronCode.Hooks;

public interface IModifyFaradGain
{
    public int ModifyFaradGain(Player player, decimal amount, ValueProp props, CardModel? cardSource);
}

public interface IAfterModifyingFaradGain
{
    public Task AfterModifyingFaradGain();
}