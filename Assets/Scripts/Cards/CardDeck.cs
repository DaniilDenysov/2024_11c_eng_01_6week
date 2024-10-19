using Managers;
using Selectors;
using Characters;
using General;
using UnityEngine;

namespace Cards
{
    public class CardDeck : MonoBehaviour
    {
        [SerializeField] private GameObject ownedBy;
        [SerializeField] private GameObject cardsContainer;
        [SerializeField] private CardManager cardManager;
        
        private const int cardNumber = 4;
        private CharacterStateManager _stateManager;

        void Awake()
        {
            if (!ownedBy.TryGetComponent(out _stateManager))
            {
                Debug.LogError("Failed to find state manager on card deck owner: " + ownedBy.name);
            }
        }

        private void Start()
        {
            EventManager.OnTurnEnd += OnTurnEnd;
            
            for (int i = 0; i < cardNumber; i++)
            {
                AddCard();
            }
        }

        private void Update() {
            cardsContainer.SetActive(CharacterSelector.CurrentCharacter.gameObject == ownedBy);
        }

        private int GetCardNumber()
        {
            return cardsContainer.transform.childCount;
        }

        private void AddCard()
        {
            CardPoolable newCard = cardManager.GetRandomCard();
            
            if (newCard.TryGetComponent(out Card card))
            {
                card.SetUp(ownedBy, _stateManager);
            }
            else
            {
                Debug.LogError("Failed to find Card Component on card: " + newCard.name);
            }
            
            newCard.transform.SetParent(cardsContainer.transform);
            newCard.gameObject.SetActive(true);
        }
        
        private void AddCard(string cardName)
        {
            CardPoolable newCard = cardManager.Get(cardName);
            
            if (newCard.TryGetComponent(out Card card))
            {
                card.SetUp(ownedBy, _stateManager);
            }
            else
            {
                Debug.LogError("Failed to find Card Component on card: " + newCard.name);
            }
            
            newCard.transform.SetParent(cardsContainer.transform);
            newCard.gameObject.SetActive(true);
        }

        private void OnTurnEnd()
        {
            for (int i = GetCardNumber(); i < cardNumber; i++)
            {
                AddCard();
            }
        }
    }
}
