using System.Collections;

using Cyberspeed.CardMatch.Cards;
using Cyberspeed.CardMatch.Audio;
using Cyberspeed.CardMatch.Enums;

using UnityEngine;
using UnityEngine.UI;

namespace Cyberspeed.CardMatch.Game
{
    /// <summary>
    /// Manages a single game session, from setting up the board to handling victory and game-over states.
    /// </summary>
    public class GameSessionController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Vector2Int _pairRange = new(2, 8);
        [SerializeField] private int _maxRows = 4;
        [SerializeField] private int _maxColumns = 6;
        [SerializeField] private float _delay = 0.5f;
        [SerializeField] private int _failLimit = -1;
        [SerializeField] private float _restartDelay = 2f;

        [Header("References")]
        [SerializeField] private Card _cardPrefab;
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private ScoreController _scoreController;

        private BoardBuilder _boardBuilder;
        private PairEvaluator _pairEvaluator;

        /// <summary>
        /// Creates and starts a new game session.
        /// It clears the previous board and sets up a new one.
        /// </summary>
        public void CreateNewSession()
        {
            AudioManager.Instance.PlaySfx(SoundType.GameStart);
            StopAllCoroutines();

            _pairEvaluator?.Stop();

            ClearBoard();

            _boardBuilder = new BoardBuilder(_cardPrefab, _grid, _pairRange, _maxRows, _maxColumns);
            _pairEvaluator = new PairEvaluator(this, _boardBuilder, _delay, _failLimit, OnVictory, OnGameOver, OnScoreMatch, OnFail);

            _boardBuilder.OnCardClicked += _pairEvaluator.HandleCardClicked;

            _boardBuilder.BuildBoard();
            _pairEvaluator.Start();
            
            _scoreController.SetLives(_failLimit);
        }

        /// <summary>
        /// Handles the victory state of the game.
        /// It restarts the game after a delay.
        /// </summary>
        private void OnVictory()
        {
            Debug.Log("Victory!");
            StartCoroutine(RestartGameAfterDelay());
        }

        /// <summary>
        /// Handles the game-over state of the game.
        /// It restarts the game after a delay.
        /// </summary>
        private void OnGameOver()
        {
            Debug.Log("Game Over!");
            _scoreController.ResetScore();
            StartCoroutine(RestartGameAfterDelay());
        }
        
        /// <summary>
        /// Handles the score match event.
        /// </summary>
        private void OnScoreMatch()
        {
            Debug.Log("Match!");
            _scoreController.ScoreMatch();
        }
        
        private void OnFail(int remaining)
        {
            _scoreController.SetLives(remaining);
        }

        /// <summary>
        /// Restarts the game after a delay.
        /// </summary>
        private IEnumerator RestartGameAfterDelay()
        {
            yield return new WaitForSeconds(_restartDelay);
            CreateNewSession();
        }

        /// <summary>
        /// Clears the board by destroying all child objects.
        /// </summary>
        private void ClearBoard()
        {
            if (_boardBuilder != null)
                _boardBuilder.OnCardClicked -= _pairEvaluator.HandleCardClicked;

            if (_grid != null)
            {
                foreach (Transform child in _grid.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}
