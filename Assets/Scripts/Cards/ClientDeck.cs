using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;


namespace Cards
{
    public class ClientDeck : LabelContainer<Card, ClientDeck>
    {
        public override ClientDeck GetInstance()
        {
            return this;
        }
    }
}
