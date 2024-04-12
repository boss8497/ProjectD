using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UI_InGame : MonoBehaviour{
    public UnityEvent On;
    public UnityEvent Off;

    public UI_GameResult gameResult;

    public TextMeshProUGUI scoreText;
    public string          scoreTextFormat;
    private void Awake(){
        GameManager.OnBeginEnterGame += OnBeginEnterGame;
        GameManager.SetScore         += SetScore;
        GameManager.GameResult       += OnGameResult;
        Off.Invoke();
    }

    private void OnDestroy(){
        GameManager.OnBeginEnterGame -= OnBeginEnterGame;
        GameManager.SetScore         -= SetScore;
        GameManager.GameResult       -= OnGameResult;
    }

    private void OnGameResult(GameResult result){
        gameResult.Result(result);
    }

    private void SetScore(int score){
        if (scoreText != null){
            scoreText.text = string.Format(scoreTextFormat, score);
        }
    }

    private void OnBeginEnterGame(){
        if (scoreText != null){
            scoreText.text = string.Format(scoreTextFormat, 0);
        }
        On.Invoke();
    }
}
