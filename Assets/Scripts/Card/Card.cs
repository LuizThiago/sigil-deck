using System;
using Cyberspeed.CardMatch.Enums;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cyberspeed.CardMatch.Cards
{
    public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public event Action<Card> OnCardClicked;
        
        [SerializeField] private CardVisualController _visualController;
        
        private CardState _currentState = CardState.Hidden;
        private bool _isBlocked = false;
       
        public bool IsBlocked => _isBlocked || _currentState == CardState.Disabled;
        public int Symbol { get; private set; }

        #region Initialization

        public void Initialize(int symbol)
        {
            Symbol = symbol;
            _visualController.SetSymbolText(Symbol.ToString());
        }

        #endregion
        
        #region Card State

        public void RevealCard() => SetState(CardState.Revealed);
        
        public void HideCard() => SetState(CardState.Hidden);

        public void DisableCard()
        {
            SetState(CardState.Disabled);
        } 
        
        public void SetBlocked(bool isBlocked)
        {
            _isBlocked = true;
        }

        private void SetState(CardState state)
        {
            if (IsBlocked || _currentState == state) return;
            
            _currentState = state;
            _visualController.SetState(state);
        }
        
        #endregion
        
        #region Pointer Events
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }
    
        public void OnPointerExit(PointerEventData eventData)
        {
            
        }
    
        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsBlocked || _currentState == CardState.Revealed) return;
            
            OnCardClicked?.Invoke(this);
        }
        
        #endregion
    }
}