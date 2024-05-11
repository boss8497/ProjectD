using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

public class GameResult{
    public bool isWin;
    public int  score;

    public void Reset(){
        score = 0;
        isWin = false;
    }

    public void Set(int _score, bool _isWin){
        Reset();
        score = _score;
        isWin = _isWin;
    }
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
    private GameResult            gameResult;

    private void Awake(){
        GameManager.OnCollisionBlock += OnCollisionBlock;
        GameManager.OnCollisionCoin  += OnCollisionCoin;
        GameManager.OnReStartGame    += OnReStartGame;
    }

    private void OnDestroy(){
        GameManager.OnCollisionBlock -= OnCollisionBlock;
        GameManager.OnCollisionCoin  -= OnCollisionCoin;
        GameManager.OnReStartGame    -= OnReStartGame;
    }
    
    private void OnReStartGame(){
        ReStart();
    }

    public async Task Initialize(){
        score     = 0;
        stageInfo = StageDataManager.Instance.GetStageInfo(level);
        if (stageInfo == null){
            Debug.LogError("not found Stage Info");
        }

        mapSr = map.GetComponent<SpriteRenderer>();

        LoadPlayer();
        LoadPattern(stageInfo);
        LoadCoint();
        NextPattern(true);
        await SetCoin();
    }

    private async void ReStart(){
        score = 0;
        player.controller.Initialize(Direction.Left, mapSr);
        NextPattern(true);
        await SetCoin();
        GameManager.SetScoreEvent(score);
        GameManager.OnReStartGameEndEvent();
    }

    public void Release(){
        foreach (var pattern in runtime_patterns){
            foreach (var block in pattern.blocks){
                ObjectPoolingManager.Instance.Push(block.gameObject, block.blockinfo.poolingKey);
            }
        }
        
        ObjectPoolingManager.Instance.Push(player.gameObject, PoolingKey.Player);
        ObjectPoolingManager.Instance.Push(coin.gameObject, PoolingKey.Coin);
    }

    private void LoadPlayer(){
        player            = new Runtime_Player();
        player.gameObject = ObjectPoolingManager.Instance.Pop(PoolingKey.Player, content);
        player.controller = player.gameObject.GetComponent<Player_Controller>();
        player.sr         = player.gameObject.GetComponent<SpriteRenderer>();
        player.controller.Initialize(Direction.Left, mapSr);
    }

    private void LoadPattern(StageInfo info){
        runtime_patterns = new List<Runtime_Pattern>();
        foreach (var pattern in stageInfo.patterns){
            var rpattern = new Runtime_Pattern();
            rpattern.blocks = new List<Runtime_Block>();
            foreach (var block in pattern.blocks){
                rpattern.blocks.Add(LoadBlock(block, info));
            }

            runtime_patterns.Add(rpattern);
        }
    }

    private Runtime_Block LoadBlock(Block _block, StageInfo info){
        var rBlock = new Runtime_Block();
        rBlock.blockinfo  = _block;
        rBlock.gameObject = ObjectPoolingManager.Instance.Pop(rBlock.blockinfo.poolingKey, content);
        rBlock.controller = rBlock.gameObject.GetComponent<Block_Controller>();
        rBlock.controller.Init(_block, map.transform, mapSr, info);
        return rBlock;
    }

    private void LoadCoint(){
        coin            = new Runtime_Coin();
        coin.gameObject = ObjectPoolingManager.Instance.Pop(PoolingKey.Coin, content, false);
        coin.sr         = coin.gameObject.GetComponent<SpriteRenderer>();

        var padding    = stageInfo.coninPadding;
        var mapBounds  = mapSr.bounds;
        var coinBounds = coin.sr.bounds;
        coin.min = new Vector3(mapBounds.min.x + coinBounds.size.x * 0.5f + padding.x, mapBounds.min.y + coinBounds.size.y * 0.5f + padding.y);
        coin.max = new Vector3(mapBounds.max.x - coinBounds.size.x * 0.5f - padding.x, mapBounds.max.y - coinBounds.size.y * 0.5f - padding.y);
    }


    private void NextPattern(bool initPos = false){
        var currentPattern = runtime_patterns.FirstOrDefault();
        if (currentPattern == null) return;

        if (initPos){
            foreach (var block in currentPattern.blocks){
                block.controller.InitPosition();
                block.controller.StartMove();
            }
        }

        if(initPos == false){
            foreach (var block in currentPattern.blocks){
                block.controller.MoveToNextPosition(score);
            }
        }

        GameManager.SetBlockEvent(currentPattern.blocks);
    }

    private async Task SetCoin(){
        coin.gameObject.transform.position = new Vector3(Random.Range(coin.min.x, coin.max.x), Random.Range(coin.min.y, coin.max.y));;
        coin.gameObject.SetActive(true);

        while (player.controller.CreateCoinCollision(coin)){
            coin.gameObject.transform.position = new Vector3(Random.Range(coin.min.x, coin.max.x), Random.Range(coin.min.y, coin.max.y));
            await Task.Delay(1);
        }
        
        player.controller.SetCoin(coin);
    }
    
    private void OnCollisionCoin(){
        coin.gameObject.SetActive(false);
        score += 1;
        NextPattern();
        SetCoin();
        GameManager.SetScoreEvent(score);
    }

    private void OnCollisionBlock(){
        Fail();
    }

    public void Win(){
    }

    public void Fail(){
        if (gameResult == null){
            gameResult = new GameResult();
        }
        gameResult.Set(score, false);
        GameManager.GameResultEvent(gameResult);
    }
}