using System;
using System.Collections;
using System.Collections.Generic;

using Cyberspeed.CardMatch.Cards;
using Cyberspeed.CardMatch.Audio;
using Cyberspeed.CardMatch.Enums;

using UnityEngine;

namespace Cyberspeed.CardMatch.Game
{
    /// <summary>
    /// Processes card pair evaluation requests. It manages a queue of selected card pairs,
    /// checks if they are a match, and triggers victory or game-over conditions.
    /// </summary>
    public class PairEvaluator
    {
        private readonly WaitForSeconds _wait;
        private readonly int _failLimit;
        private readonly Action _onVictory;
        private readonly Action _onGameOver;
        private readonly Action _onMatch;
        private readonly Action<int> _onFail;
        private readonly MonoBehaviour _runner;
        private readonly BoardBuilder _boardBuilder;
        private readonly Queue<PairCheckRequest> _queue = new();
        private readonly List<Card> _buffer = new();
        private int _pairsFound;
        private int _fails;
        private bool _gameOver;
        private Coroutine _processingCoroutine;

        /// <summary>
        /// Initializes a new instance of the <see cref="PairEvaluator"/> class.
        /// </summary>
        /// <param name="runner">The MonoBehaviour to run the processing coroutine on.</param>
        /// <param name="boardBuilder">Reference to the board builder to get total pair count.</param>
        /// <param name="delay">The delay between evaluating pairs.</param>
        /// <param name="failLimit">The number of allowed fails before game over. -1 for infinite.</param>
        /// <param name="onVictory">Action to invoke when all pairs are found.</param>
        /// <param name="onGameOver">Action to invoke when the fail limit is reached.</param>
        /// <param name="onMatch">Action to invoke when a pair is found.</param>
        /// <param name="onFail">Action to invoke when a pair is found.</param>
        public PairEvaluator(MonoBehaviour runner, BoardBuilder boardBuilder, float delay, int failLimit, Action onVictory, Action onGameOver, Action onMatch, Action<int> onFail)
        {
            _runner = runner;
            _boardBuilder = boardBuilder;
            _wait = new WaitForSeconds(delay);
            _failLimit = failLimit;
            _onVictory = onVictory;
            _onGameOver = onGameOver;
            _onMatch = onMatch;
            _onFail = onFail;
        }

        /// <summary>
        /// Starts the pair processing queue.
        /// </summary>
        public void Start()
        {
            _processingCoroutine = _runner.StartCoroutine(ProcessQueue());
        }

        /// <summary>
        /// Stops the pair processing queue.
        /// </summary>
        public void Stop()
        {
            if (_processingCoroutine != null)
                _runner.StopCoroutine(_processingCoroutine);
        }

        /// <summary>
        /// Handles a card click event. Adds the card to a buffer and, if the buffer is full,
        /// enqueues a new pair check request.
        /// </summary>
        /// <param name="card">The card that was clicked.</param>
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

        /// <summary>
        /// Processes the queue of pair check requests.
        /// </summary>
        /// <returns>An IEnumerator that can be used to process the queue.</returns>
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
                        OnMatch(pair);
                    }
                    else
                    {
                        OnFail(pair);
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

        /// <summary>
        /// Handles a failed pair check request.
        /// </summary>
        /// <param name="pair"></param>
        private void OnFail(PairCheckRequest pair)
        {
            AudioManager.Instance.PlaySfx(SoundType.MatchFail);
            pair.First.MissCard();
            pair.Second.MissCard();
            _fails++;
            
            _onFail?.Invoke(_failLimit - _fails);

            if (_failLimit > 0 && _fails >= _failLimit)
            {
                _gameOver = true;
                _onGameOver?.Invoke();
            }
        }

        /// <summary>
        /// Handles a successful pair check request.
        /// </summary>
        /// <param name="pair"></param>
        private void OnMatch(PairCheckRequest pair)
        {
            AudioManager.Instance.PlaySfx(SoundType.MatchSuccess);
            pair.First.DisableCard();
            pair.Second.DisableCard();
            _pairsFound++;
                     
            _onMatch?.Invoke();

            if (_pairsFound >= _boardBuilder.TotalPairs)
            {
                _gameOver = true;
                _onVictory?.Invoke();
            }
        }

        /// <summary>
        /// Represents a request to check if two cards form a pair.
        /// </summary>
        private class PairCheckRequest
        {
            public Card First;
            public Card Second;
        }
    }
}