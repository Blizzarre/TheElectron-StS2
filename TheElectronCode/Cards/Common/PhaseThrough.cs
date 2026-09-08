using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;


namespace TheElectron.TheElectronCode.Cards.Common;


public class PhaseThrough : ElectronEmptyCard
{
    public PhaseThrough() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(8, 3);
        WithVar(new QuarkCountVar(1));
        WithTip(ElectronHoverTip.Produce);
        WithQuarkTip<BottomQuark>();
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    protected override async Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
        {
            await QuarkCmd.Produce<BottomQuark>(choiceContext, Owner, this, play);
        }
    }
}