using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Cards.Common;


public class BlueMatter : ElectronEmptyCard
{
    public BlueMatter() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithQuarkTip<DownQuark>();
        WithVar(new QuarkCountVar(2).WithUpgrade(1));
        WithVar(new DynamicVar("ExtraQuark", 1));
    }

    protected override async Task OnPlayWrapper(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        for (var i = 0; i < DynamicVars.QuarkCount().IntValue; i++)
        {
            await QuarkCmd.Produce<DownQuark>(choiceContext, Owner, this, play);
        }
    }

    protected override async Task OnPlayEmptyAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        for (var i = 0; i < DynamicVars["ExtraQuark"].IntValue; i++)
        {
            await QuarkCmd.Produce<DownQuark>(choiceContext, Owner, this, play);
        }
    }
}