using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Powers;

namespace TheElectron.TheElectronCode.Cards.Common;

public class PlugIn : ElectronCard
{
    public PlugIn() : base(0, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
    {
        WithPower<QuantumLinkPower>(2, 1);
        WithVar("FaradLoss", 1);
        WithTip(ElectronHoverTip.Farad);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await PowerCmd.Apply<QuantumLinkPower>(choiceContext, CombatState!.HittableEnemies,
            DynamicVars.Power<QuantumLinkPower>().BaseValue, Owner.Creature, this);

        await ElectronPlayerCmd.LoseFarad(choiceContext, Owner, DynamicVars["FaradLoss"].BaseValue, this, play);
    }
}