using Cyberspeed.CardMatch.Enums;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Cyberspeed.CardMatch.Cards
{
    /// <summary>
    /// Manages the visual representation of a card, including its front, back, and symbol.
    /// </summary>
    public class CardVisualController : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Image _frontVisual;
        [SerializeField] private Image _backVisual;
        [SerializeField] private Sprite[] _symbols;

        #region Public Methods

        /// <summary>
        /// Sets the visual state of the card.
        /// </summary>
        /// <param name="state">The new state for the card.</param>
        public void SetState(CardState state)
        {
            if (state == CardState.Disabled)
            {
                _root.SetActive(false);
            }
        }

        /// <summary>
        /// Sets the symbol index on the front of the card.
        /// </summary>
        /// <param name="index">The index of the symbol to display.</param>
        public void SetSymbol(int index)
        {
            if (index < 0 || index >= _symbols.Length)
            {
                Debug.LogError($"Invalid symbol index: {index}");
                return;
            }

            _frontVisual.sprite = _symbols[index];
        }

        #endregion
    }
}