using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TheElectron.TheElectronCode.Commands;
using TheElectron.TheElectronCode.DynamicVars;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.HoverTips;
using TheElectron.TheElectronCode.Models.Quarks;

namespace TheElectron.TheElectronCode.Cards.Uncommon;

public class CloudChamber : ElectronCard
{
    public CloudChamber() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(6, 3);
        WithTip(ElectronHoverTip.Produce);
        WithQuarkTip<CharmQuark>();
        WithVar(new QuarkCountVar(1));
        WithVar("UniqueQuarks", 3);
    }

    private bool HasEnoughUniqueQuarks()
    {
        var uniqueCount = Owner.PlayerCombatState?.GetQuarkQueue()?.Quarks.Select(q => q.Id.Entry).Distinct()
            .Count() ?? 0;
        return uniqueCount >= DynamicVars["UniqueQuarks"].IntValue;
    }

    protected override bool ShouldGlowGoldInternal => HasEnoughUniqueQuarks();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        if (HasEnoughUniqueQuarks())
            for (var i = 0; i < DynamicVars.QuarkCount.IntValue; i++)
                await QuarkCmd.Produce<CharmQuark>(choiceContext, Owner, this, play);
    }
}