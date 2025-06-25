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
        [Tooltip("-1 = no limits")][SerializeField] private int _failLimit = 3;
        [Header("References")]
        [SerializeField] private Card _cardPrefab;
        [SerializeField] private GridLayoutGroup _gridRoot;
        
        private readonly Queue<PairCheckRequest> _pairQueue = new();
        private readonly List<Card> _revealBuffer = new();
        private bool _isProcessingQueue = false;
        private int _failCount = 0;
        private bool _isGameOver = false;
        private int _totalPairs;
        private int _pairsFound;
        
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
            }
            
            _totalPairs = _cardCount / 2;
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
            if (_isGameOver || _revealBuffer.Contains(card)) return;

            card.RevealCard();
            _revealBuffer.Add(card);

            if (_revealBuffer.Count < 2) return;
            
            var request = new PairCheckRequest
            {
                First = _revealBuffer[0],
                Second = _revealBuffer[1]
            };

            _pairQueue.Enqueue(request);
            _revealBuffer.Clear();

            if (!_isProcessingQueue)
                StartCoroutine(ProcessQueue());
        }
        
        #endregion
        
        #region Pair Processing
        
        private IEnumerator ProcessQueue()
        {
            _isProcessingQueue = true;

            while (_pairQueue.Count > 0)
            {
                var pair = _pairQueue.Dequeue();

                yield return new WaitForSeconds(_pairEvaluationSecs);

                if (pair.First.Symbol == pair.Second.Symbol)
                {
                    pair.First.DisableCard();
                    pair.Second.DisableCard();
                    ScorePair();
                }
                else
                {
                    pair.First.HideCard();
                    pair.Second.HideCard();
                    MissPair();

                    if (_isGameOver) break;
                }
            }

            _isProcessingQueue = false;
        }

        private void ScorePair()
        {
            _pairsFound++;
            OnPairFound?.Invoke();
            
            if (_pairsFound >= _totalPairs)
            {
                EndGame(isVictory: true);
            }
        }
        
        private void MissPair()
        {
            _failCount++;
            OnPairFailed?.Invoke();
            
            if (_failLimit > 0 && _failCount >= _failLimit)
            {
                EndGame(isVictory: false);
            }
        }

        #endregion

        #region Game End
        
        private void EndGame(bool isVictory)
        {
            if (isVictory)
            {
                Debug.Log("Congratulations! You found all pairs.");
            }
            else
            {
                Debug.Log("Game Over! You have reached the fail limit.");
                GameOver();
            }
        }
        
        private void GameOver()
        {
            _isGameOver = true;
        }
        
        #endregion
        
        private class PairCheckRequest
        {
            public Card First;
            public Card Second;
        }
    }
}
