using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Cards;
using TheElectron.TheElectronCode.Field;

namespace TheElectron.TheElectronCode.Models;

public class ElectronSingletonModel() : CustomSingletonModel(HookType.Combat)
{
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay is {IsLastInSeries: true})
            ElectronField.DrainExcessEnergy[cardPlay.Card] = 0;

        if (cardPlay is { Card: ElectronEmptyCard emptyCard, IsLastInSeries: true })
        {
            emptyCard.IsPlayedAsEmpty = false;
            emptyCard.HasPaidEnergyCost = false;
        }

        if (cardPlay is { Card: ElectronDepleteCard depleteCard, IsLastInSeries: true })
            depleteCard.IsEnergyDepleted = false;

        return Task.CompletedTask;
    }
}