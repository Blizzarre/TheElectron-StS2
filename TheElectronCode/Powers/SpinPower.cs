using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.Models;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Powers;

public class SpinPower : TheElectronPower, IModifyQuarkValueAdditive, IAfterQuarksFused
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    
    public decimal ModifyQuarkValueAdditive(QuarkModel quark, decimal value)
    {
        if (quark is UpQuark or DownQuark)
        {
            return value + Amount;
        }

        return value;
    }

    public async Task AfterQuarksFused(PlayerChoiceContext choiceContext, Player player, IEnumerable<QuarkModel> fusedQuarks)
    {
        if (player.Creature == Owner)
        {
            await PowerCmd.Remove(this);
        }
    }
}