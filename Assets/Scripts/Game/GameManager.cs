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

        private void Start()
        {
            StartNewGame();
        }

        private void StartNewGame()
        {
            _sessionController.CreateNewSession();
        }
    }
}
