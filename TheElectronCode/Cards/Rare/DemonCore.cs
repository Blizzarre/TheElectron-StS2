using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class DemonCore : ElectronCard
{
    public DemonCore() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPower<SpinPower>(5, 4);
        WithPower<StabilityPower>(5);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<SpinPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<SpinPower>().BaseValue, Owner.Creature, this);

        await PowerCmd.Apply<StabilityPower>(choiceContext, Owner.Creature,
            DynamicVars.Power<StabilityPower>().BaseValue, Owner.Creature, this);
    }
}