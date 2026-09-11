using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class Modify : ElectronDepleteCard
{
    public Modify() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<StrengthPower>(2, 1);
        WithPower<DexterityPower>(1, 1);
    }

    protected override async Task BeforeOnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<StrengthPower>().BaseValue, Owner.Creature, this);
    }

    protected override async Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<DexterityPower>().BaseValue, Owner.Creature, this);
    }
}