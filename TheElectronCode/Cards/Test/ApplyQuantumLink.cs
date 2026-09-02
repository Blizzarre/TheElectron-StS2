using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Test;

public class ApplyQuantumLink : ElectronCard
{
    public ApplyQuantumLink() : base(0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy)
    {
        WithPower<QuantumLinkPower>(2, 1);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await PowerCmd.Apply<QuantumLinkPower>(choiceContext, play.Target,
            DynamicVars.Power<QuantumLinkPower>().BaseValue, Owner.Creature, this);
    }
}