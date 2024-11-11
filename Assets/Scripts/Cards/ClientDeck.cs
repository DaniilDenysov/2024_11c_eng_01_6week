using System;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.Events;


namespace Cards
{
    public class ClientDeck : LabelContainer<Card, ClientDeck>
    {
        public override ClientDeck GetInstance()
        {
            return this;
        }

        public override void Add(Card item)
        {
            base.Add(item);
            
            EventManager.FireEvent(EventManager.OnCardCountChange, items.Count);
        }
        
        public override bool Remove(Card item)
        {
            bool result = base.Remove(item);
            
            if (result)
            {
                EventManager.FireEvent(EventManager.OnCardCountChange, items.Count);
            }

            return result;
        }

        public void DiscardCards()
        {
            foreach (var item in items) 
            {
                item.DiscardMove();
            }
        }
    }
}
