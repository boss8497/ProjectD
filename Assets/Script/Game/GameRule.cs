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

    public void Initialize(){
        score     = 0;
        stageInfo = StageDataManager.Instance.GetStageInfo(level);
        if (stageInfo == null){
            Debug.LogError("not found Stage Info");
        }
    }

    private void LoadPattern(){
        foreach (var pattern in stageInfo.patterns){
            var rpattern = new Runtime_Pattern();
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
        return rBlock;
    }
}