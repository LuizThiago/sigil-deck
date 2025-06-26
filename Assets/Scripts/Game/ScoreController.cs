using System;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _scorePerMatch = 10;
    [Header("References")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _livesText;
    
    private int _score;

    public void ScoreMatch()
    {
        _score += _scorePerMatch;
        _scoreText.text = $"Score: {_score}";
    }
    
    public void ResetScore()
    {
        _score = 0;
        _scoreText.text = $"Score: {_score}";
    }
    
    public void SetLives(int lives)
    {
        if (lives < 0)
        {
            _livesText.gameObject.SetActive(false);
            return;
        }

        if (!_livesText.gameObject.activeSelf)
        {
            _livesText.gameObject.SetActive(true);
        }
        
        _livesText.text = $"Lives: {lives}";
    }
}
