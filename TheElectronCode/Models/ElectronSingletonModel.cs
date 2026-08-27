using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Cards;

namespace TheElectron.TheElectronCode.Models;

public class ElectronSingletonModel() : CustomSingletonModel(HookType.Combat)
{
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay is { Card: ElectronEmptyCard emptyCard, IsLastInSeries: true })
        {
            emptyCard.IsPlayedAsEmpty = false;
        }
        
        if (cardPlay is { Card: ElectronDepleteCard depleteCard, IsLastInSeries: true })
        {
            depleteCard.IsEnergyDepleted = false;
        }

        return Task.CompletedTask;
    }
}