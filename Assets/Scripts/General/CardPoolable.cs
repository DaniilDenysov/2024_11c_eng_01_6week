using Managers;
using Shooting;
using UnityEngine;
using UnityEngine.Events;

namespace General
{
    public class CardPoolable : MonoBehaviour
    {
        [SerializeField] protected UnityEvent _onPool;
        private CardManager _cardManager;
        private string _poolableName;

        public void SetUp(CardManager cardManager, string poolableName)
        {
            _cardManager = cardManager;
            _poolableName = poolableName;
        }

        public string GetPoolableName()
        {
            return _poolableName;
        }

        public void Pool()
        {
            _cardManager.Put(this);
            gameObject.transform.SetParent(null);
            _onPool.Invoke();
            gameObject.SetActive(false);
        }
    }
}