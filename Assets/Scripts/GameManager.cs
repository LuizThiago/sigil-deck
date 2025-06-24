using System;
using System.Collections;
using System.Collections.Generic;
using Cyberspeed.CardMatch.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace Cyberspeed.CardMatch.Game
{
    public class GameManager : MonoBehaviour
    {
        public event Action OnPairFound;
        public event Action OnPairFailed;
        
        [Header("Settings")]
        [SerializeField] private int _cardCount = 10;
        [SerializeField] private int _columns = 3;
        [SerializeField] private float _pairEvaluationSecs = 3f;
        [Header("References")]
        [SerializeField] private Card _cardPrefab;
        [SerializeField] private GridLayoutGroup _gridRoot;

        private readonly List<Card> _cards = new List<Card>();
        private readonly List<Card> _selectedCards = new List<Card>();
        
        private const int Pair = 2;
        private bool _isInputDisabled = false;
    
        #region MonoBehaviour
        
        private void Awake()
        {
            InitializeBoard();
        }
        
        private void Start()
        {
            SpawnCards();
        }
        
        #endregion
        
        #region Initialization

        private void InitializeBoard()
        {
            _gridRoot.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridRoot.constraintCount = _columns;
        }
        
        private void SpawnCards()
        {
            var symbols = GenerateShuffledSymbols();

            foreach (var symbol in symbols) 
            {
                var card = Instantiate(_cardPrefab, _gridRoot.transform);
                card.Initialize(symbol);
                card.OnCardClicked += OnCardClicked;
                _cards.Add(card);
            }
        }
        
        private List<int> GenerateShuffledSymbols()
        {
            var symbols = new List<int>();

            for (int i = 0; i < _cardCount / 2; i++)
            {
                symbols.Add(i);
                symbols.Add(i);
            }

            for (int i = 0; i < symbols.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, symbols.Count);
                (symbols[i], symbols[randomIndex]) = (symbols[randomIndex], symbols[i]);
            }

            return symbols;
        }
        
        #endregion
        
        #region Card Handling
        
        private void OnCardClicked(Card card)
        {
            if (_isInputDisabled || _selectedCards.Contains(card)) return;
            
            _selectedCards.Add(card);
            card.RevealCard();

            if (_selectedCards.Count != Pair) return;
            
            StartCoroutine(EvaluatePair());
        }

        private IEnumerator EvaluatePair()
        {
            _isInputDisabled = true;
            
            yield return new WaitForSeconds(_pairEvaluationSecs);
            
            if (IsPair())
            {
                ScorePair();
                DisablePair();
            }
            else
            {
                MissPair();
                ResetSelectedCards();
            }
            
            _isInputDisabled = false;
        }
        
        private bool IsPair()
        {
            var first = _selectedCards[0];
            var second = _selectedCards[1];

            return first.Symbol == second.Symbol;
        }

        private void ScorePair()
        {
            OnPairFound?.Invoke();
        }
        
        private void MissPair()
        {
            OnPairFailed?.Invoke();
        }

        private void DisablePair()
        {
            foreach (var card in _selectedCards)
            {
               card.DisableCard();
            }
            
            _selectedCards.Clear();
        }
        
        private void ResetSelectedCards()
        {
            foreach (var card in _selectedCards)
            {
                if (card == null) continue;
                
                card.HideCard();
            }
            
            _selectedCards.Clear();
        }

        #endregion
    }
}
