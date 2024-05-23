using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_GameResult : MonoBehaviour{
    public UnityEvent On;
    public UnityEvent Off;
    
    public TextMeshProUGUI scoreText;
    public string          scoreFormat;
    
    
    //cheat
    private GameResult      gameResult;
    public  TextMeshProUGUI playerSizeText;
    public  Slider          playerSizeSlider;
    public  TextMeshProUGUI playerSpeedText;
    public  Slider          playerSpeedSlider;
    public  TextMeshProUGUI enemySizeText;
    public  Slider          enemySizeSlider;
    public  TextMeshProUGUI enemySpeedText;
    public  Slider          enemySpeedSlider;
    public  TextMeshProUGUI coinSizeText;
    public  Slider          coinSizeSlider;
    
    private void Awake(){
        Off.Invoke();
    }

    public void Result(GameResult result){
        gameResult = result;
        if (scoreText != null){
            scoreText.text = String.Format(scoreFormat, result.score);
        }
        
        
        //cheat
        if (playerSizeText != null){
            playerSizeText.text = $"Player Size : {gameResult.stageinfo.playerSize}";
            if (playerSizeSlider != null)
                playerSizeSlider.value = gameResult.stageinfo.playerSize;
        }
        if (playerSpeedText != null){
            playerSpeedText.text = $"Player Speed : {gameResult.stageinfo.playerSpeed}";
            if (playerSpeedSlider != null)
                playerSpeedSlider.value = gameResult.stageinfo.playerSpeed;
        }
        if (enemySizeText != null){
            enemySizeText.text = $"Enemy Size : {gameResult.stageinfo.enemySize}";
            if (enemySizeSlider != null)
                enemySizeSlider.value = gameResult.stageinfo.enemySize;
        }
        if (enemySpeedText != null){
            enemySpeedText.text = $"Enemy Speed : {gameResult.stageinfo.enemySpeed}";
            if (enemySpeedSlider != null)
                enemySpeedSlider.value = gameResult.stageinfo.enemySpeed;
        }
        if (coinSizeText != null){
            coinSizeText.text = $"Coin Size : {gameResult.stageinfo.coinSize}";
            if (coinSizeSlider != null)
                coinSizeSlider.value = gameResult.stageinfo.coinSize;
        }
        
        On.Invoke();
    }

    public void ReStart(){
        GameManager.OnReStartGameEvent();
        Off.Invoke();
    }
    
    
    //cheat
    public void OnChangePlayerSize(Slider slider){
        if (gameResult?.stageinfo == null) return;
        gameResult.stageinfo.playerSize = slider.value;
        if (playerSizeText != null){
            playerSizeText.text = $"Player Size : {gameResult.stageinfo.playerSize}";
        }
    }
    public void OnChangePlayerSpeed(Slider slider){
        if (gameResult?.stageinfo == null) return;
        gameResult.stageinfo.playerSpeed = slider.value;
        if (playerSpeedText != null){
            playerSpeedText.text = $"Player Speed : {gameResult.stageinfo.playerSpeed}";
        }
    }
    public void OnChangeEnemySize(Slider slider){
        if (gameResult?.stageinfo == null) return;
        gameResult.stageinfo.enemySize = slider.value;
        if (enemySizeText != null){
            enemySizeText.text = $"Enemy Size : {gameResult.stageinfo.enemySize}";
        }
    }
    public void OnChangeEnemySpeed(Slider slider){
        if (gameResult?.stageinfo == null) return;
        gameResult.stageinfo.enemySpeed = slider.value;
        if (enemySpeedText != null){
            enemySpeedText.text = $"Enemy Speed : {gameResult.stageinfo.enemySpeed}";
        }
    }
    public void OnChangeCoinSize(Slider slider){
        if (gameResult?.stageinfo == null) return;
        gameResult.stageinfo.coinSize = slider.value;
        if (coinSizeText != null){
            coinSizeText.text = $"Coin Size : {gameResult.stageinfo.coinSize}";
        }
    }
}
