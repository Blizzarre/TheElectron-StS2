using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class Lightspeed : ElectronCard
{
    public Lightspeed() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(12, 4);
        WithTip(ElectronKeywords.Drain);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        foreach (var card in PileType.Hand.GetPile(Owner).Cards
                     .Where(c => c.CanDrain()))
        {
            ElectronCardCmd.ApplySingleTurnDrain(card);
        }
    }
}