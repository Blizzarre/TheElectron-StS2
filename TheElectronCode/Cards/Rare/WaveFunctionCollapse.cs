using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class WaveFunctionCollapse : ElectronCard
{
    public WaveFunctionCollapse() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
        WithTip(ElectronHoverTip.Produce);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var queue = Owner.PlayerCombatState?.GetQuarkQueue();

        if (queue != null && queue.HasAny())
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

            var currQuarks = new List<QuarkModel>(queue.Quarks);
            await QuarkCmd.Fuse(choiceContext, Owner, this, play);

            foreach (var quark in currQuarks.Select(q => q.CreateClone()))
                await QuarkCmd.Produce(choiceContext, quark, Owner, this, play);
        }
    }
}