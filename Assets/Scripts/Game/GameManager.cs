using UnityEngine;

namespace Cyberspeed.CardMatch.Game
{
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
