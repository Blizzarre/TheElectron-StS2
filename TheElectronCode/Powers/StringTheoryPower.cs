using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace TheElectron.TheElectronCode.Powers;

public class StringTheoryPower : TheElectronPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        // Extra check to not apply Quantum Link to the owner of this power
        if (dealer == Owner && target != Owner && result.UnblockedDamage > 0)
        {
            Flash();
            await PowerCmd.Apply<QuantumLinkPower>(choiceContext, target, Amount, dealer, null);
        }
    }
}