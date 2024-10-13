using Lobby;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class LobbyCharacterSelectionManager : NetworkBehaviour
    {
        [SerializeField] private List<LobbyCharacterSelector> selectors = new List<LobbyCharacterSelector>();

        [SerializeField] private LobbyCharacterSelector currentlySelected;

        public void Select (LobbyCharacterSelector selected)
        {
            if (currentlySelected != null)
            {
                currentlySelected.SetBlock(false);
            }
            currentlySelected.SetBlock(true);
        }

    }
}
