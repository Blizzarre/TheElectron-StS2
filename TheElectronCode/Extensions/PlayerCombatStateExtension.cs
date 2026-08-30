using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using TheElectron.TheElectronCode.Entities;
using TheElectron.TheElectronCode.Field;

namespace TheElectron.TheElectronCode.Extensions;

public static class PlayerCombatStateExtension
{
    public class ElectronCombatState(PlayerCombatState combatState, QuarkQueue quarkQueue)
    {
        public int Farad
        {
            get;
            set
            {
                if (field == value) return;
                var farad = field;
                field = Math.Max(value, 0);
                var state = combatState._player.Creature.CombatState;
                if (state != null)
                {
                    CombatManager.Instance.History.FaradModified(state, field - farad, combatState._player);
                }

                FaradChanged?.Invoke(farad, field);
            }
        }

        public QuarkQueue QuarkQueue => quarkQueue;

        public event Action<int, int>? FaradChanged;
        

        public void GainFarad(int amount)
        {
            Farad += amount;
        }

        public void LoseFarad(int amount)
        {
            Farad -= amount;
        }
    }
    
    extension(PlayerCombatState playerCombatState)
    {
        public QuarkQueue? GetQuarkQueue()
        {
            var electronCombatState = playerCombatState.Electron();
            return electronCombatState?.QuarkQueue;
        }

        public int GetFarad()
        {
            var electronCombatState = playerCombatState.Electron();
            return electronCombatState?.Farad ?? 0;
        }
        
        public ElectronCombatState? Electron()
        {
            return ElectronField.ElectronCombatState[playerCombatState];
        }
    }
}