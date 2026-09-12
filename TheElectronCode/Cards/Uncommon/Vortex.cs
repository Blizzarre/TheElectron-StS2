using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class Vortex : ElectronCard
{
    public Vortex() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCalculatedVar("SpinGain", 0, 2,
            static (card, _) => card.Owner.PlayerCombatState?.GetQuarkQueue()?.Quarks.Select(q => q.Id.Entry)
                .Distinct().Count() ?? 0, 2);
        WithTip(typeof(SpinPower));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<SpinPower>(choiceContext, Owner.Creature,
            DynamicVars["SpinGain"].Calculate(), Owner.Creature, this);
    }
}