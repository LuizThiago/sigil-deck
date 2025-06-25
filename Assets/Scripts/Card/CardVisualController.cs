using Cyberspeed.CardMatch.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cyberspeed.CardMatch.Cards
{
    public class CardVisualController : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private CardVisual _frontVisual;
        [SerializeField] private CardVisual _backVisual;
        
        #region Public Methods
        
        public void SetState(CardState state)
        {
            if (state == CardState.Disabled)
            {
                _root.SetActive(false);
            }
        }
        
        public void SetSymbolText(string symbol)
        {
            _frontVisual.SetSymbolText(symbol);
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