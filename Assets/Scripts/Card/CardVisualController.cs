using Cyberspeed.CardMatch.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cyberspeed.CardMatch.Cards
{
    public class CardVisualController : MonoBehaviour
    {
        [SerializeField] private CardVisual _frontVisual;
        [SerializeField] private CardVisual _backVisual;
        
        #region Public Methods
        
        public void SetState(CardState state)
        {
            switch (state)
            {
                case CardState.Disabled:
                    gameObject.SetActive(false);
                    return;
                case CardState.Revealed:
                    Reveal();
                    break;
                case CardState.Hidden:
                    Hide();
                    break;
            }
        }
        
        public void SetSymbolText(string symbol)
        {
            _frontVisual.SetSymbolText(symbol);
        }
        
        #endregion
        
        #region Visual State Handling

        private void Reveal()
        {
            _frontVisual.Image.gameObject.SetActive(true);
            _backVisual.Image.gameObject.SetActive(false);
        }
        
        private void Hide()
        {
            _frontVisual.Image.gameObject.SetActive(false);
            _backVisual.Image.gameObject.SetActive(true);
        }
        
        #endregion

        [System.Serializable]
        private class CardVisual
        {
            [SerializeField] private Image _image;
            [SerializeField] private TMP_Text _symbolText;
            
            public Image Image => _image;
            
            public void SetSymbolText(string symbol)
            {
                _symbolText.text = symbol;
            }
        }
    }
}