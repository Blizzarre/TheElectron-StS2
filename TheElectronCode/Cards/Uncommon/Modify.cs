using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class Modify : ElectronEmptyCard
{
    public Modify() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<DexterityPower>(1, 1);
        WithPower<StrengthPower>(1, 1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task BeforeOnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<DexterityPower>().BaseValue, Owner.Creature, this);
    }
    
    protected override async Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<StrengthPower>().BaseValue, Owner.Creature, this);
    }
}