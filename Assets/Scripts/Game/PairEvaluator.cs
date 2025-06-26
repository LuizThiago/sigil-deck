using System;
using System.Collections;
using System.Collections.Generic;

using Cyberspeed.CardMatch.Cards;

using UnityEngine;

namespace Cyberspeed.CardMatch.Game
{
    public class PairEvaluator
    {
        private readonly WaitForSeconds _wait;
        private readonly int _failLimit;
        private readonly Action _onVictory;
        private readonly Action _onGameOver;
        private readonly MonoBehaviour _runner;
        private readonly BoardBuilder _boardBuilder;
        private readonly Queue<PairCheckRequest> _queue = new();
        private readonly List<Card> _buffer = new();
        private int _pairsFound;
        private int _fails;
        private bool _gameOver;
        private Coroutine _processingCoroutine;

        public PairEvaluator(MonoBehaviour runner, BoardBuilder boardBuilder, float delay, int failLimit, Action onVictory, Action onGameOver)
        {
            _runner = runner;
            _boardBuilder = boardBuilder;
            _wait = new WaitForSeconds(delay);
            _failLimit = failLimit;
            _onVictory = onVictory;
            _onGameOver = onGameOver;
        }

        public void Start()
        {
            _processingCoroutine = _runner.StartCoroutine(ProcessQueue());
        }

        public void Stop()
        {
            if (_processingCoroutine != null)
                _runner.StopCoroutine(_processingCoroutine);
        }

        public void HandleCardClicked(Card card)
        {
            if (_gameOver || _buffer.Contains(card)) return;

            card.RevealCard();
            _buffer.Add(card);

            if (_buffer.Count < 2) return;

            var pair = new PairCheckRequest { First = _buffer[0], Second = _buffer[1] };
            _queue.Enqueue(pair);
            _buffer.Clear();
        }

        private IEnumerator ProcessQueue()
        {
            while (!_gameOver)
            {
                if (_queue.Count > 0)
                {
                    var pair = _queue.Dequeue();
                    yield return _wait;

                    if (_gameOver)
                    {
                        pair.First.MissCard();
                        pair.Second.MissCard();
                        continue;
                    }

                    if (pair.First.Symbol == pair.Second.Symbol)
                    {
                        pair.First.DisableCard();
                        pair.Second.DisableCard();
                        _pairsFound++;

                        if (_pairsFound >= _boardBuilder.TotalPairs)
                        {
                            _gameOver = true;
                            _onVictory?.Invoke();
                        }
                    }
                    else
                    {
                        pair.First.MissCard();
                        pair.Second.MissCard();
                        _fails++;

                        if (_failLimit > 0 && _fails >= _failLimit)
                        {
                            _gameOver = true;
                            _onGameOver?.Invoke();
                        }
                    }
                }
                else
                {
                    yield return null;
                }
            }

            // Cleanup any remaining cards in the buffer when the game ends.
            if (_buffer.Count > 0)
            {
                _buffer[0].MissCard();
                _buffer.Clear();
            }
        }

        private class PairCheckRequest
        {
            public Card First;
            public Card Second;
        }
    }
}