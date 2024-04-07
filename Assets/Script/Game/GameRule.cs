using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Runtime_Block{
    public Block            blockinfo;
    public GameObject       gameObject;
    public Block_Controller controller;
}

public class Runtime_Pattern{
    public List<Runtime_Block> blocks;
}

public class GameRule : MonoBehaviour{
    public int        level;
    public GameObject map;
    public Transform content;
    public GameObject player;

    private int             score;
    private StageInfo       stageInfo;
    private Runtime_Pattern runtime_pattern;

    private void Awake(){
        GameManager.OnBeginEnterGame += OnBeginEnterGame;
    }

    private void OnDestroy(){
        GameManager.OnBeginEnterGame -= OnBeginEnterGame;
    }

    private void OnBeginEnterGame(){
        Initialize();
    }

    public void Initialize(){
        score     = 0;
        stageInfo = StageDataManager.Instance.GetStageInfo(level);
        if (stageInfo == null){
            Debug.LogError("not found Stage Info");
        }

        LoadPattern();
    }

    private void LoadPattern(){
        foreach (var pattern in stageInfo.patterns){
            var rpattern = new Runtime_Pattern();
            rpattern.blocks = new List<Runtime_Block>();
            foreach (var block in pattern.blocks){
                rpattern.blocks.Add(LoadBlock(block));
            }
        }
    }

    private Runtime_Block LoadBlock(Block _block){
        var rBlock = new Runtime_Block();
        rBlock.blockinfo  = _block;
        rBlock.gameObject = ObjectPoolingManager.Instance.Pop(rBlock.blockinfo.poolingKey, content);
        rBlock.controller = rBlock.gameObject.GetComponent<Block_Controller>();
        rBlock.controller.SetData(_block, map.transform);
        return rBlock;
    }
}