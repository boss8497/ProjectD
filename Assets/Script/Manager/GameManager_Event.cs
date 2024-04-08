using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class GameManager : MonoBehaviour{
    public static event System.Action BeginGameInitialize;
    public static void BeginGameInitializeEvent(){
        BeginGameInitialize?.Invoke();
    }
    public static event System.Action EndGameInitialize;
    public static void EndGameInitializeEvent(){
        EndGameInitialize?.Invoke();
    }

    public static event System.Action<float> GameInitializeProgress;
    public static void GameInitializeProgressEvent(float progress){
        GameInitializeProgress?.Invoke(progress);
    }
    
    public static event System.Action OnBeginEnterGame;
    public static void BeginEnterGame(){
        OnBeginEnterGame?.Invoke();
    }
    public static event System.Action OnEndEnterGame;
    public static void EndEnterGameEvent(){
        OnEndEnterGame?.Invoke();
    }
    
    
    public static event System.Action<Transform> PlayerMove;
    public static void PlayerMoveEvent(Transform tr){
        PlayerMove?.Invoke(tr);
    }

    public static event System.Action<List<Runtime_Block>> SetBlock;
    public static void SetBlockEvent(List<Runtime_Block> blocks){
        SetBlock?.Invoke(blocks);
    }

    public static event System.Action OnCollisionBlock;
    public static void OnCollisionBlockEvent(){
        OnCollisionBlock?.Invoke();
    }
    
    public static event System.Action OnCollisionCoin;
    public static void OnCollisionCoinEvent(){
        OnCollisionCoin?.Invoke();
    }

    public static event System.Action<bool, int> GameResult;
    public static void GameResultEvent(bool isWin, int score){
        GameResult?.Invoke(isWin, score);
    }
    
    
    public static event System.Action OnCreateCoinRequest;
    public static void OnCreateCoinRequestEvent(){
        OnCreateCoinRequest?.Invoke();
    }
    
}