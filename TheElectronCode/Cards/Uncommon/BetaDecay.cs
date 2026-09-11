using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class BetaDecay : ElectronCard
{
    public BetaDecay() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(ElectronHoverTip.Fuse);
        WithVar(new FaradVar(2));
        WithTip(ElectronHoverTip.Farad);
        WithCalculatedVar("FaradGain", 0,
            static (card, _) =>
                card.DynamicVars.Farad.BaseValue * (card.Owner.PlayerCombatState?.GetQuarkQueue()?.Quarks.Count ?? 0));
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var queue = Owner.PlayerCombatState?.GetQuarkQueue();

        if (queue != null && queue.HasAny())
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

            var count = queue.Quarks.Count;
            await QuarkCmd.Fuse(choiceContext, Owner, this, play);

            await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars.Farad.BaseValue * count, this, play);
        }
    }
}