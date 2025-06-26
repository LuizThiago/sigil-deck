using System;

using Cyberspeed.CardMatch.Enums;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Cyberspeed.CardMatch.Cards
{
    /// <summary>
    /// Represents a single card in the game. It manages the card's state,
    /// its symbol, and player interactions like clicks and hovers.
    /// </summary>
    public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public event Action<Card> OnCardClicked;

        [SerializeField] private CardVisualController _visualController;
        [SerializeField] private CardAnimationsController _animationsController;

        private CardState _currentState = CardState.Hidden;
        private bool _isBlocked = false;

        public bool IsBlocked => _isBlocked || _currentState == CardState.Disabled;

        /// <summary>
        /// Gets the symbol identifier for this card.
        /// </summary>
        public int Symbol { get; private set; }

        #region Initialization

        /// <summary>
        /// Initializes the card with a specific symbol.
        /// </summary>
        /// <param name="symbol">The integer representing the card's symbol.</param>
        public void Initialize(int symbol)
        {
            Symbol = symbol;
            _visualController.SetSymbol(Symbol);
        }

        #endregion

        #region Card State

        /// <summary>
        /// Reveals the card, playing the reveal animation.
        /// </summary>
        public void RevealCard()
        {
            if (_currentState == CardState.Revealed) return;

            SetState(CardState.Revealed);
            _animationsController.PlayReveal();
        }

        /// <summary>
        /// Hides the card.
        /// </summary>
        public void HideCard()
        {
            if (_currentState == CardState.Hidden) return;

            SetState(CardState.Hidden);
        }

        /// <summary>
        /// Plays the shake animation for a mismatch and then hides the card.
        /// </summary>
        public void MissCard()
        {
            if (_currentState == CardState.Hidden) return;

            _animationsController.PlayShake(() => _animationsController.PlayHide(() => SetState(CardState.Hidden)));
        }

        /// <summary>
        /// Disables the card after a successful match, playing a destroy animation.
        /// </summary>
        public void DisableCard()
        {
            if (_currentState == CardState.Disabled) return;

            SetState(CardState.Disabled);
            _animationsController.PlayDestroy(() => _visualController.SetState(CardState.Disabled));
        }

        /// <summary>
        /// Explicitly sets the blocked state of the card.
        /// </summary>
        /// <param name="isBlocked">True to block interaction, false to unblock.</param>
        public void SetBlocked(bool isBlocked)
        {
            _isBlocked = isBlocked;
        }

        /// <summary>
        /// Sets the state of the card and updates the blocked state.
        /// </summary>
        /// <param name="state">The new state to set.</param>
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