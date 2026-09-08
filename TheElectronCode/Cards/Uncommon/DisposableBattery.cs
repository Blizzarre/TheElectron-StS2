using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class DisposableBattery : ElectronCard
{
    public DisposableBattery() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(1);
        WithEnergy(1, 1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
}