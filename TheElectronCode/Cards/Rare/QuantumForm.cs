using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheElectron.TheElectronCode.Powers;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class QuantumForm : ElectronCard
{
    public QuantumForm() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(ElectronKeywords.Drain, UpgradeType.Add);
        WithEnergyTip();
        WithVar(new EnergyVar("Amount", 1));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        (await PowerCmd.Apply<QuantumFormPower>(choiceContext, Owner.Creature,
            1, Owner.Creature, this))?.SetOwnerCard(this);
    }
}