using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Models.Quarks;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Common;

public class AlloyArmor : ElectronDepleteCard
{
    public AlloyArmor() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeyword(ElectronKeywords.Drain);
        WithBlock(12, 2);
        WithQuarkTip<UpQuark>();
        WithVar(new QuarkCountVar(1).WithUpgrade(1));
    }

    protected override async Task OnPlayWrapper(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    protected override async Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
            await QuarkCmd.Produce<UpQuark>(choiceContext, Owner, this, play);
    }
}