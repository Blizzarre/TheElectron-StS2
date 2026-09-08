using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Powers;

public class CyclotronPower : TheElectronPower, IAfterQuarksFused
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterQuarksFused(PlayerChoiceContext choiceContext, Player player, IEnumerable<QuarkModel> fusedQuarks)
    {
        if (Owner.Player == player)
        {
            var count = fusedQuarks.Count();
            
            await PowerCmd.Apply<SpinPower>(choiceContext, Owner, count * Amount, Owner, null);
        }
    }
}