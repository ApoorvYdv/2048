using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager INSTANCE;

    public TextMeshProUGUI bestScore;
    public TextMeshProUGUI currentScore;

    private void Awake()
    {
        if (INSTANCE == null)
            INSTANCE = this;
        else
            Destroy(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        if (!PlayerPrefs.HasKey("BestScore"))
            PlayerPrefs.SetInt("BestScore", 0);

        currentScore.text = "0";
        bestScore.text = PlayerPrefs.GetInt("BestScore").ToString();
    }

    public void UpdateCurrentScore(int _score)
    {
        int _currentScore;
        int.TryParse(currentScore.text, out _currentScore);
        _currentScore = _currentScore + _score;
        currentScore.text = _currentScore.ToString();

        UpdateBestScore(_currentScore);
    }

    public void UpdateBestScore(int _score)
    {
        if (_score > PlayerPrefs.GetInt("BestScore"))
        {
            bestScore.text = _score.ToString();
            PlayerPrefs.SetInt("BestScore", _score);
        }
    }


}
