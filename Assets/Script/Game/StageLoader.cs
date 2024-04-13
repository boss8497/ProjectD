using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLoader : MonoBehaviour{
    public  GameObject content;
    private GameObject currentStage;
    private GameRule currentGameRule;
    private StageInfo  currentStageInfo;
    private void Awake(){
        GameManager.OnBeginEnterGame += OnBeginEnterGame;
    }

    private void OnDestroy(){
        GameManager.OnBeginEnterGame -= OnBeginEnterGame;
    }

    private void OnBeginEnterGame(StageInfo stageInfo){
        currentStageInfo = stageInfo;
        InitializeStage();
    }

    private async void InitializeStage(){
        if (currentStage != null){
            ObjectPoolingManager.Push(currentStage);
            currentStage = null;
        }

        currentStage = await AddressableManager.Instance.LoadAsset<GameObject>(currentStageInfo.stagePrefabPath);
        var ins = Instantiate(currentStage, content.transform);
        ins.transform.localPosition = Vector3.zero;
        currentGameRule             = ins.GetComponent<GameRule>();
        await currentGameRule.Initialize();
    }
}