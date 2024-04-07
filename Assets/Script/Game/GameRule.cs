using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private int                  score;
    private StageInfo            stageInfo;
    private List<Runtime_Pattern> runtime_patterns;

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
        runtime_patterns = new List<Runtime_Pattern>();
        foreach (var pattern in stageInfo.patterns){
            var rpattern = new Runtime_Pattern();
            rpattern.blocks = new List<Runtime_Block>();
            foreach (var block in pattern.blocks){
                rpattern.blocks.Add(LoadBlock(block));
            }
            runtime_patterns.Add(rpattern);
        }

        var first = runtime_patterns.FirstOrDefault();
        foreach (var block in first.blocks){
            block.controller.StartMove();
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