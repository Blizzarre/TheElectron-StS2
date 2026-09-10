using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class FlyHigh : ElectronCard
{
    public FlyHigh() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(10, 4);
        WithQuarkTip<UpQuark>();
        WithVar(new QuarkCountVar(1));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.CardBlock(this, play);

        var queue = Owner.PlayerCombatState?.GetQuarkQueue();
        if (queue == null) return;
        var produceCount = queue.Capacity - queue.Quarks.Count;

        for (var i = 0; i < produceCount; i++) await QuarkCmd.Produce<UpQuark>(choiceContext, Owner, this, play);
    }
}