using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Powers;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class OnTheEdge : ElectronDepleteCard
{
    public OnTheEdge() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(ElectronKeywords.Drain);
        WithBlock(34, 8);
        WithVar("SelfDamage", 10);
    }

    protected override async Task OnPlayWrapper(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    protected override async Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        (await PowerCmd.Apply<OverTheEdgePower>(choiceContext, Owner.Creature, 2, Owner.Creature, this))?.SetDamage(
            DynamicVars["SelfDamage"].BaseValue);
    }
}