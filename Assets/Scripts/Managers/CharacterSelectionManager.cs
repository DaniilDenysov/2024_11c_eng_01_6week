using Lobby;
using System.Linq;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class CharacterSelectionManager : NetworkBehaviour
    {
        [SerializeField] private List<LobbyCharacterSelector> selectors = new List<LobbyCharacterSelector>();

        [ClientRpc]
        public void Block (string lastCharacterGUID,int i)
        {
            selectors.Where((s)=>s.GetCharacterData().CharacterGUID == lastCharacterGUID).FirstOrDefault().SetBlock(false);
            selectors[i].SetBlock(true);
        }

        public int GetSelectorIndex(LobbyCharacterSelector selector) => selectors.IndexOf(selector);
    }
}
