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
        [SerializeField] private CardAnimationsController _animationsController;
        
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

        public void RevealCard()
        {
            SetState(CardState.Revealed);
            _animationsController.PlayReveal();
        }

        public void HideCard() => SetState(CardState.Hidden);
        
        public void MissCard()
        {
            SetState(CardState.Hidden);
            _animationsController.PlayShake(() => _animationsController.PlayHide());
        }

        public void DisableCard()
        {
            SetState(CardState.Disabled);
            _animationsController.PlayDestroy(() => _visualController.SetState(CardState.Disabled));
        } 
        
        public void SetBlocked(bool isBlocked)
        {
            _isBlocked = isBlocked;
        }

        private void SetState(CardState state)
        {
            if (_currentState == state) return;
            
            _currentState = state;
            SetBlocked(_currentState != CardState.Hidden);
        }
        
        #endregion
        
        #region Pointer Events
    
        public void OnPointerEnter(PointerEventData eventData)
        {
            _animationsController.PlayPunchScale();
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