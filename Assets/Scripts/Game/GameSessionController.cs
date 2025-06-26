using System.Collections;

using Cyberspeed.CardMatch.Cards;

using UnityEngine;
using UnityEngine.UI;

namespace Cyberspeed.CardMatch.Game
{
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

        private BoardBuilder _boardBuilder;
        private PairEvaluator _pairEvaluator;

        public void CreateNewSession()
        {
            StopAllCoroutines();

            _pairEvaluator?.Stop();

            ClearBoard();

            _boardBuilder = new BoardBuilder(_cardPrefab, _grid, _pairRange, _maxRows, _maxColumns);
            _pairEvaluator = new PairEvaluator(this, _boardBuilder, _delay, _failLimit, OnVictory, OnGameOver);

            _boardBuilder.OnCardClicked += _pairEvaluator.HandleCardClicked;

            _boardBuilder.BuildBoard();
            _pairEvaluator.Start();
        }

        private void OnVictory()
        {
            Debug.Log("Victory!");
            StartCoroutine(RestartGameAfterDelay());
        }

        private void OnGameOver()
        {
            Debug.Log("Game Over!");
            StartCoroutine(RestartGameAfterDelay());
        }

        private IEnumerator RestartGameAfterDelay()
        {
            yield return new WaitForSeconds(_restartDelay);
            CreateNewSession();
        }

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
