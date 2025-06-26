using UnityEngine;

namespace Cyberspeed.CardMatch.Game
{
    /// <summary>
    /// The main entry point for the game. This class is responsible for
    /// initiating the game session.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameSessionController _sessionController;
        [SerializeField] private ScoreController _scoreController;
        
        private void Start()
        {
            StartNewGame();
        }

        private void StartNewGame()
        {
            _scoreController.ResetScore();
            _sessionController.CreateNewSession();
        }
    }
}
