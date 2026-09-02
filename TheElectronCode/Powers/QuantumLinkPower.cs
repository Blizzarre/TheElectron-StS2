using MegaCrit.Sts2.Core.Combat;
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

public class QuantumLinkPower : TheElectronPower, IAfterFaradLost
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;


    public async Task AfterFaradLost(PlayerChoiceContext choiceContext, Player player, decimal amountLost,
        CardModel? cardSource = null,
        CardPlay? cardPlay = null)
    {
        // Only damages the owner of this power on the player's turn
        if (player.Creature != Applier || CombatState.CurrentSide != player.Creature.Side) return;
        if (amountLost <= 0) return;

        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, amountLost * Amount,
            ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        await Cmd.CustomScaledWait(0.2f, 0.35f);
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult damageResult,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        // Only damages the owner of this power on the damage-receiving-player's turn
        if (target.Side != CombatSide.Player || CombatState.CurrentSide != target.Side || target != Applier) return;
        if (damageResult.UnblockedDamage <= 0) return;

        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, damageResult.UnblockedDamage * Amount,
            ValueProp.Unblockable | ValueProp.Unpowered, target, null, null);
        await Cmd.CustomScaledWait(0.2f, 0.35f);
    }
}