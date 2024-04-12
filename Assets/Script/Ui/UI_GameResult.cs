using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UI_GameResult : MonoBehaviour{
    public UnityEvent On;
    public UnityEvent Off;
    
    public TextMeshProUGUI scoreText;
    public string          scoreFormat;

    private void Awake(){
        Off.Invoke();
    }

    public void Result(GameResult result){
        if (scoreText != null){
            scoreText.text = String.Format(scoreFormat, result.score);
        }
        On.Invoke();
    }

    public void ReStart(){
        GameManager.OnReStartGameEvent();
        Off.Invoke();
    }
}
