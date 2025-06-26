using System;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _scorePerMatch = 10;
    [Header("References")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _highScoreText;
    [SerializeField] private TMP_Text _livesText;
    
    private int _score;
    private int _highScore;
    
    private void Awake()
    {
        _highScore = PlayerPrefs.GetInt("HighScore", 0);
        SetHighScoreText();
    }

    public void ScoreMatch()
    {
        _score += _scorePerMatch;
        _scoreText.text = $"Score: {_score}";
        
        CheckHighScore();
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
    
    private void CheckHighScore()
    {
        if (_score <= _highScore) return;
        
        _highScore = _score;
        PlayerPrefs.SetInt("HighScore", _highScore);
        
        SetHighScoreText();
    }
    
    private void SetHighScoreText()
    {
        if (_highScore == 0)
        {
            _highScoreText.gameObject.SetActive(false);
        }
        else
        {
            if (!_highScoreText.gameObject.activeSelf)
            {
                _highScoreText.gameObject.SetActive(true);
            }
        
            _highScoreText.text = $"High Score: {_highScore}";
        }
    }
}
