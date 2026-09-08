using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheElectron.TheElectronCode.Hooks;

namespace TheElectron.TheElectronCode.Powers;

public class MalleableChassisPower : TheElectronPower, IAfterFaradLost
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && result.UnblockedDamage > 0)
        {
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        }
    }

    public async Task AfterFaradLost(PlayerChoiceContext choiceContext, Player player, decimal amountLost, CardModel? cardSource = null,
        CardPlay? cardPlay = null)
    {
        if (player == Owner.Player && amountLost > 0)
        {
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        }
    }
}