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
    
    public TextMeshProUGUI exceptionText;
    private void Awake(){
        GameManager.OnBeginEnterGame += OnBeginEnterGame;
        GameManager.SetScore         += SetScore;
        GameManager.GameResult       += OnGameResult;
        GameManager.ExceptionMessage += ExceptionMessage;
        Off.Invoke();
    }

    private void OnDestroy(){
        GameManager.OnBeginEnterGame -= OnBeginEnterGame;
        GameManager.SetScore         -= SetScore;
        GameManager.GameResult       -= OnGameResult;
        GameManager.ExceptionMessage -= ExceptionMessage;
    }

    private void ExceptionMessage(string obj){
        if (exceptionText.text.Split('\n').Length > 10){
            exceptionText.text = string.Empty;
        }
        exceptionText.text += $"\n{obj}";
    }

    private void OnGameResult(GameResult result){
        gameResult.Result(result);
    }

    private void SetScore(int score){
        if (scoreText != null){
            scoreText.text = string.Format(scoreTextFormat, score);
        }
    }

    private void OnBeginEnterGame(StageInfo stageInfo){
        if (scoreText != null){
            scoreText.text = string.Format(scoreTextFormat, 0);
        }
        On.Invoke();
    }
}
