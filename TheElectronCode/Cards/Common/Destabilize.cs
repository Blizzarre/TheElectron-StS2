using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Common;

public class Destabilize : ElectronCard
{
    public Destabilize() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithPower<QuantumLinkPower>(4, 1);
        WithPower<VulnerablePower>(2, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await PowerCmd.Apply<QuantumLinkPower>(choiceContext, play.Target,
            DynamicVars.Power<QuantumLinkPower>().BaseValue, Owner.Creature, this);

        await PowerCmd.Apply<VulnerablePower>(choiceContext, play.Target,
            DynamicVars.Power<VulnerablePower>().BaseValue, Owner.Creature, this);
    }
}