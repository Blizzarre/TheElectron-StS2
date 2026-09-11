using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;
using TheElectron.TheElectronCode.Powers;
using TheElectron.TheElectronCode.Utils;

namespace TheElectron.TheElectronCode.Cards.Rare;

public class FissionCell : ElectronCard
{
    public FissionCell() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(ElectronKeywords.Drain);
        WithTip(ElectronHoverTip.Produce);
        WithQuarkTip<UpQuark>();
        WithVar(new QuarkCountVar(2).WithUpgrade(1));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, CreatureAnimator.castTrigger, Owner.Character.CastAnimDelay);

        await PowerCmd.Apply<FissionCellPower>(choiceContext, Owner.Creature,
            1, Owner.Creature, this);

        for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
            await QuarkCmd.Produce<UpQuark>(choiceContext, Owner, this, play);
    }
}