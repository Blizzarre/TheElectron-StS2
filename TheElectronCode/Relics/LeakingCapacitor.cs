using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Relics;

public class LeakingCapacitor : TheElectronRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    private const string UpQuarkKey = "UpQuark";
    private const string DownQuarkKey = "DownQuark";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new(UpQuarkKey, 1),
        new(DownQuarkKey, 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ElectronHoverTipFactory.Static(ElectronHoverTip.Produce),
        ElectronHoverTipFactory.FromQuark<UpQuark>(),
        ElectronHoverTipFactory.FromQuark<DownQuark>()
    ];

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner.Creature) && Owner.PlayerCombatState is { TurnNumber: <= 1 })
        {
            for (var i = 0; i < DynamicVars[UpQuarkKey].IntValue; i++)
                await QuarkCmd.Produce<UpQuark>(new BlockingPlayerChoiceContext(), Owner);
            for (var i = 0; i < DynamicVars[DownQuarkKey].IntValue; i++)
                await QuarkCmd.Produce<DownQuark>(new BlockingPlayerChoiceContext(), Owner);
        }
    }
}