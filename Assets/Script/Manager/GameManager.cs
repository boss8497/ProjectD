using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public partial class GameManager : MonoBehaviour{
    public static   GameManager Instance;
    public readonly TimeSpan    TimeOut = new(TimeSpan.TicksPerSecond * 10);
    
    private void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
            return;
        }

        Initialize();
    }
    
    //게임 실행시 초기화
    private async void Initialize(){
        GameManager.BeginGameInitializeEvent();
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount  = 1;
        await Initialize_Addressable();
        GameManager.EndGameInitializeEvent();
    }

    //인트로 -> 인게임 로딩
    public async void Loading(){
        await Initialize_ObjectPooling();
        await LoadData();
        GameManager.GameInitializeProgressEvent(0.7f);

        GameManager.GameInitializeProgressEvent(1.0f);
    }

    private async Task Initialize_Addressable(){
        if(AddressableManager.Instance == null){
            AddressableManager.Instance = new AddressableManager();
            AddressableManager.Instance.Initialize();
        }
        
        while (AddressableManager.Instance.Valid() == false){
            var waitTime = 0.0f;
            await Task.Delay(1);
            waitTime += Time.deltaTime;
            if (waitTime > TimeOut.Seconds){
                throw new Exception("AddressableManager is Not Initialized");
            }
        }
    }

    private async Task Initialize_ObjectPooling(){
        await ObjectPoolingManager.Instance.Initialize();
    }

    //Load GameData
    private async Task LoadData(){
        
    }
}