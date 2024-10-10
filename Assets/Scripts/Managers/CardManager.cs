using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;

namespace Managers
{
    public class CardManager : Manager
    {
        List<GameObject> cards = new();

        public List<String> GetCardNameList()
        {
            List<String> result = new List<string>();

            for (int i = 0; i < cards.Capacity; i++)
            {
                result.Add(cards[i].name);
            }

            return result;
        }

        public int GetCardNumber()
        {
            return cards.Capacity;
        }

        public GameObject GetCard(int index)
        {
            return Instantiate(cards[index]);
        }

        public override void InstallBindings()
        {
            Container.Bind<CardManager>().FromInstance(this).AsSingle();
        }
    }
}
