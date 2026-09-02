using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using TheElectron.TheElectronCode.Extensions;
using TheElectron.TheElectronCode.Hooks;

namespace TheElectron.TheElectronCode.Commands;

public static class ElectronPlayerCmd
{
    public static async Task GainFarad(PlayerChoiceContext choiceContext, Player player, decimal amount,
        CardModel? cardSource = null, CardPlay? cardPlay = null)
    {
        if (amount <= 0 || CombatManager.Instance.IsEnding || player.Creature.CombatState == null) return;
        var combatState = player.Creature.CombatState;

        var electronCombatState = player.PlayerCombatState?.Electron();
        var finalAmount = ElectronHook.ModifyFaradGain(combatState, player, amount, ValueProp.Move,
            cardPlay?.Card, out var modifiers);
        await ElectronHook.AfterModifyingFaradGain(modifiers);
        if (finalAmount > 0)
            // TODO sfx
            electronCombatState?.GainFarad((int)finalAmount);

        await ElectronHook.AfterFaradGained(combatState, choiceContext, player, finalAmount, cardSource, cardPlay);
    }

    /// <summary>
    /// Decrement Farad from the player by the amount.
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="player"></param>
    /// <param name="amount"></param>
    /// <param name="cardSource"></param>
    /// <param name="cardPlay"></param>
    /// <returns>Excess amount</returns>
    public static async Task LoseFarad(PlayerChoiceContext choiceContext, Player player, decimal amount,
        CardModel? cardSource = null, CardPlay? cardPlay = null)
    {
        if (amount <= 0 || CombatManager.Instance.IsEnding || player.Creature.CombatState == null) return;

        var combatState = player.Creature.CombatState;
        var electronCombatState = player.PlayerCombatState?.Electron();
        electronCombatState?.LoseFarad((int)amount);

        await ElectronHook.AfterFaradLost(combatState, choiceContext, player, amount, cardSource, cardPlay);
    }
}