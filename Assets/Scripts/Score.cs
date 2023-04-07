using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Score : MonoBehaviour
{
    private int _scoreGame = 0;
    [SerializeField] private Text _scoreText;

    private void OnEnable()
    {
        CoinController._scoreForCoint += ScoreSum;
        PlayerController._resetLevel += ResetScore;
    }
    private void OnDisable()
    {
        CoinController._scoreForCoint -= ScoreSum;
        PlayerController._resetLevel -= ResetScore;
    }

    private void Update()
    {
        _scoreText.text = (_scoreGame).ToString();
    }
    private void ScoreSum()
    {
        _scoreGame += 1;
    }
    private void ResetScore()
    {
        _scoreGame = 0;
    }
}
