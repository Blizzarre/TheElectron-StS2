using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;

namespace TheElectron.TheElectronCode.Cards.Test;

public class GainFarad : ElectronCard
{
    public GainFarad() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithVar(new FaradVar(2).WithUpgrade(1));
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars.Farad().BaseValue);
    }
}