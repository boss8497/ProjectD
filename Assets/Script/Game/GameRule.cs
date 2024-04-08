using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class Runtime_Block{
    public Block            blockinfo;
    public GameObject       gameObject;
    public Block_Controller controller;
}

public class Runtime_Pattern{
    public List<Runtime_Block> blocks;
}

public class Runtime_Coin{
    public GameObject     gameObject;
    public SpriteRenderer sr;
    public Vector3        min;
    public Vector3        max;
}

public class Runtime_Player{
    public GameObject        gameObject;
    public SpriteRenderer    sr;
    public Player_Controller controller;
}

public class GameRule : MonoBehaviour{
    public int            level;
    public GameObject     map;
    public Transform      content;
    public Runtime_Player player;
    public Runtime_Coin   coin;

    private int                   score;
    private StageInfo             stageInfo;
    private List<Runtime_Pattern> runtime_patterns;
    private SpriteRenderer        mapSr;
    private Runtime_Pattern       currentPattern;

    private void Awake(){
        GameManager.OnBeginEnterGame += OnBeginEnterGame;
        GameManager.OnCollisionBlock += OnCollisionBlock;
        GameManager.OnCollisionCoin  += OnCollisionCoin;
    }

    private void OnDestroy(){
        GameManager.OnBeginEnterGame -= OnBeginEnterGame;
        GameManager.OnCollisionBlock -= OnCollisionBlock;
        GameManager.OnCollisionCoin  -= OnCollisionCoin;
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

        mapSr = map.GetComponent<SpriteRenderer>();

        LoadPlayer();
        LoadPattern();
        LoadCoint();
        NextPattern();
        SetCoin();
    }

    public void Release(){
    }

    private void LoadPlayer(){
        player            = new Runtime_Player();
        player.gameObject = ObjectPoolingManager.Instance.Pop(PoolingKey.Player, content);
        player.controller = player.gameObject.GetComponent<Player_Controller>();
        player.sr         = player.gameObject.GetComponent<SpriteRenderer>();
        player.controller.Initialize(Direction.Left, mapSr);
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
    }

    private Runtime_Block LoadBlock(Block _block){
        var rBlock = new Runtime_Block();
        rBlock.blockinfo  = _block;
        rBlock.gameObject = ObjectPoolingManager.Instance.Pop(rBlock.blockinfo.poolingKey, content);
        rBlock.controller = rBlock.gameObject.GetComponent<Block_Controller>();
        rBlock.controller.SetData(_block, map.transform, mapSr);
        return rBlock;
    }

    private void LoadCoint(){
        coin            = new Runtime_Coin();
        coin.gameObject = ObjectPoolingManager.Instance.Pop(PoolingKey.Coin, content, false);
        coin.sr         = coin.gameObject.GetComponent<SpriteRenderer>();

        var mapBounds  = mapSr.bounds;
        var coinBounds = coin.sr.bounds;
        coin.min = new Vector3(mapBounds.min.x + coinBounds.size.x * 0.5f, mapBounds.min.y + coinBounds.size.y * 0.5f);
        coin.min = new Vector3(mapBounds.max.x - coinBounds.size.x * 0.5f, mapBounds.max.y - coinBounds.size.y * 0.5f);
    }


    private void NextPattern(){
        if (currentPattern != null){
            foreach (var block in currentPattern.blocks){
                block.controller.StopMove();
            }
        }
        
        currentPattern = runtime_patterns[score % runtime_patterns.Count];
        if (currentPattern == null){
            Debug.LogError("Pattern Set Error");
            return;
        }

        foreach (var block in currentPattern.blocks){
            block.controller.StartMove();
        }

        GameManager.SetBlockEvent(currentPattern.blocks);
    }

    private void SetCoin(){
        var pos = new Vector3(Random.Range(coin.min.x, coin.max.x), Random.Range(coin.min.y, coin.max.y));
        coin.gameObject.transform.position = pos;
        coin.gameObject.SetActive(true);
        player.controller.SetCoin(coin);
    }
    
    private void OnCollisionCoin(){
        coin.gameObject.SetActive(false);
        score += 1;
        NextPattern();
        //SetCoin();
    }

    private void OnCollisionBlock(){
        //Fail();
    }

    public void Win(){
    }

    public void Fail(){
        Debug.Log($"Fail");
        GameManager.GameResultEvent(false, 0);
    }
}