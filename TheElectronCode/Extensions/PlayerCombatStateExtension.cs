using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using TheElectron.TheElectronCode.Entities;

namespace TheElectron.TheElectronCode.Extensions;

public class PlayerCombatStateExtension
{
    public class ElectronCombatState(PlayerCombatState combatState, QuarkQueue quarkQueue)
    {
        private int _farad;

        public int Farad
        {
            get => _farad;
            private set
            {
                if (_farad == value) return;
                var farad = _farad;
                _farad = value;
                var state = combatState._player.Creature.CombatState;
                if (state != null)
                {
                    CombatManager.Instance.History.FaradModified(state, _farad - farad, combatState._player);
                }
                FaradChanged?.Invoke(farad, _farad);
            }
        }

        public QuarkQueue QuarkQueue => quarkQueue;

        public event Action<int, int>? FaradChanged;
    }
}