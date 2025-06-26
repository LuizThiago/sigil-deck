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
        [SerializeField] private CardVisual _frontVisual;
        [SerializeField] private CardVisual _backVisual;

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
        /// Sets the symbol text on the front of the card.
        /// </summary>
        /// <param name="symbol">The symbol to display.</param>
        public void SetSymbolText(string symbol)
        {
            _frontVisual.SetSymbolText(symbol);
        }

        #endregion

        /// <summary>
        /// Represents the visual elements of one side of a card (front or back).
        /// </summary>
        [System.Serializable]
        private class CardVisual
        {
            [SerializeField] private Image _image;
            [SerializeField] private TMP_Text _symbolText;

            public Image Image => _image;

            /// <summary>
            /// Sets the symbol text.
            /// </summary>
            /// <param name="symbol">The text to display as the symbol.</param>
            public void SetSymbolText(string symbol)
            {
                _symbolText.text = symbol;
            }
        }
    }
}