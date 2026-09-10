using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Common;

public class AlloyArmor : ElectronDepleteCard
{
    public AlloyArmor() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeyword(ElectronKeywords.Drain);
        WithBlock(10, 3);
        WithEnergy(1);
    }

    protected override async Task OnPlayWrapper(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    protected override async Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
}