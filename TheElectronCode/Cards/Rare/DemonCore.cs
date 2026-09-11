using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class DemonCore : ElectronEmptyCard
{
    public DemonCore() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPower<SpinPower>(7, 3);
        WithPower<StabilityPower>(2, 1);
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<SpinPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<SpinPower>().BaseValue, Owner.Creature, this);
    }

    protected override async Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<StabilityPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<StabilityPower>().BaseValue, Owner.Creature, this);
    }
}