using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Basic;

public class StableField : ElectronCard
{
    public StableField() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithVar("Amount", 2, 1);
        WithTip(ElectronHoverTip.Produce);
        WithTip(ElectronHoverTip.Stable);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<StabilityPower>(choiceContext, Owner.Creature, DynamicVars["Amount"].BaseValue,
            Owner.Creature, this);
    }
}