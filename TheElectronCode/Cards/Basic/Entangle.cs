using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Basic;

public class Entangle : ElectronDepleteCard
{
    public Entangle() : base(1, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithPower<QuantumLinkPower>(3, 4);
        WithVar(new FaradVar(1));
    }

    protected override async Task OnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await PowerCmd.Apply<QuantumLinkPower>(choiceContext, play.Target,
            DynamicVars.Power<QuantumLinkPower>().BaseValue, Owner.Creature, this);
    }

    protected override async Task OnPlayDepleteAfter(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ElectronPlayerCmd.GainFarad(choiceContext, Owner, DynamicVars.Farad().BaseValue, this, play);
    }
}