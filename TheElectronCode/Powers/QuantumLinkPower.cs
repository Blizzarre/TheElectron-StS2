using BaseLib.Hooks;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheElectron.TheElectronCode.Hooks;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Powers;

public class QuantumLinkPower : TheElectronPower, IAfterFaradLost
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;
    
    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
    {
        // Only show quantum link for the applier
        if (!LocalContext.IsMe(Applier) || context.Creature != Owner) return [];
        var forecast = new HealthBarForecastSegment
        {
            AffectsHpLabel = true,
            Amount = Amount,
            Direction = HealthBarForecastDirection.FromRight,
            Color = new Color(0.694f, 0.0f, 0.839f),
            OverlaySelfModulate = new Color(0.867f, 0.071f, 0.957f),
            Order = 100, 
            OverlayMaterial = PreloadManager.Cache.GetMaterial(ElectronResource.QuantumLinkHpForecastMaterialPath)
        };
        return [forecast];
    }
    
    public async Task AfterFaradLost(PlayerChoiceContext choiceContext, Player player, decimal amountLost,
        CardModel? cardSource = null,
        CardPlay? cardPlay = null)
    {
        if (amountLost <= 0) return;
        // Only damages the owner of this power on the player's turn
        // But ignore side checking if the applier has Superposition power
        var ignoreSide = player.Creature.HasPower<SuperpositionPower>();
        var onCorrectSide = player.Creature == Applier && CombatState.CurrentSide == player.Creature.Side;
        if (!onCorrectSide && !ignoreSide) return;

        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, Amount,
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
        if (target != Applier) return;
        if (damageResult.UnblockedDamage <= 0) return;
        
        // Only damages the owner of this power on the damage-receiving-player's turn
        // But ignore side checking if the applier has Superposition power
        var ignoreSide = target.HasPower<SuperpositionPower>();
        var onCorrectSide = target.Side == CombatSide.Player && CombatState.CurrentSide == target.Side;
        if (!onCorrectSide && !ignoreSide) return;

        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, Amount,
            ValueProp.Unblockable | ValueProp.Unpowered, target, null, null);
        await Cmd.CustomScaledWait(0.2f, 0.35f);
    }
}