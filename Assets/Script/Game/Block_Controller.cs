using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Block_Controller : MonoBehaviour{
    public SpriteRenderer blockSr;

    private Block          blockinfo;
    private Transform      map;
    private SpriteRenderer mapSr;
    private StageInfo      stageInfo;

    private Vector3   startPosition;
    private Vector3   endPosition;
    private Direction currentDir;

    private void OnEnable(){
        GameManager.GameResult += GameResult;
    }

    private void OnDisable(){
        GameManager.GameResult -= GameResult;
    }

    private void GameResult(GameResult result){
        StopAllCoroutines();
    }


    public void Init(Block _block, Transform _map, SpriteRenderer _mapSr, StageInfo _stageInfo){
        blockinfo = _block;
        map       = _map;
        mapSr     = _mapSr;
        stageInfo = _stageInfo;
        InitPosition();
        StopMove();
    }

    public void InitPosition(){
        var (start, end)   = GetPosition(GetDirection(0));
        startPosition      = start;
        transform.position = startPosition;
        endPosition        = end;
    }

    private Direction GetDirection(int score = 0){
        return blockinfo.direction[(score / stageInfo.patternRate) % blockinfo.direction.Count];
    }

    private (Vector3 start, Vector3 end) GetPosition(Direction dir){
        startPosition = Vector3.zero;
        endPosition   = Vector3.zero;
        
        var tr   = transform;
        var size = stageInfo.enemySize;
        tr.localScale = new Vector3(size, size, tr.localScale.z);
        
        var mapBounds   = mapSr.bounds;
        var blockBounds = blockSr.bounds;
        currentDir = dir;
        switch (currentDir){
            case Direction.Top:
                startPosition.y = mapBounds.size.y / 2 - blockBounds.size.y / 2;
                endPosition.y   = startPosition.y;
                startPosition.x = mapBounds.size.x   / 2 - blockBounds.size.x / 2;
                endPosition.x   = blockBounds.size.x / 2 - mapBounds.size.x   / 2;
                break;
            case Direction.Bottom:
                startPosition.y = blockBounds.size.y / 2 - mapBounds.size.y / 2;
                endPosition.y   = startPosition.y;
                startPosition.x = blockBounds.size.x / 2 - mapBounds.size.x   / 2;
                endPosition.x   = mapBounds.size.x   / 2 - blockBounds.size.x / 2;
                break;
            case Direction.Left:
                startPosition.x = blockBounds.size.x / 2 - mapBounds.size.x / 2;
                endPosition.x   = startPosition.x;
                startPosition.y = blockBounds.size.y / 2 - mapBounds.size.y   / 2;
                endPosition.y   = mapBounds.size.y   / 2 - blockBounds.size.y / 2;
                break;
            case Direction.Right:
                startPosition.x = mapBounds.size.x / 2 - blockBounds.size.x / 2;
                endPosition.x   = startPosition.x;
                startPosition.y = mapBounds.size.y   / 2 - blockBounds.size.y / 2;
                endPosition.y   = blockBounds.size.y / 2 - mapBounds.size.y   / 2;
                break;
        }

        return (startPosition, endPosition);
    }

    public void StopMove(){
        StopAllCoroutines();
    }

    public void MoveToNextPosition(int score){
        var nextDir  = GetDirection(score);
        var moveFlag = nextDir != currentDir;
        var (start, end) = GetPosition(nextDir);
        startPosition    = start;
        endPosition      = end;
        if(moveFlag){
            StopAllCoroutines();
            StartCoroutine(CoMoveToStartPosition());
        }
    }

    IEnumerator CoMoveToStartPosition(){
        var tr        = transform;
        var dir       =  startPosition - tr.position;
        var dirNormal = dir.normalized;
        var speed     = stageInfo.enemySpeed;

        while (true){
            tr.position += dirNormal * (speed * 3 * Time.deltaTime);

            if (Vector3.Distance(tr.position, startPosition) <= 0.1f){
                tr.position = startPosition;
                break;
            }

            yield return null;
        }
        StartCoroutine(CoMove(speed));
    }

    public void StartMove(){
        gameObject.SetActive(true);
        StartCoroutine(CoMove(stageInfo.enemySpeed));
    }

    IEnumerator CoMove(float speed){
        var tr = transform;

        var dirPos       = (endPosition - startPosition);
        var dirNormalPos = dirPos.normalized;

        while (true){
            var addPos   = Vector3.zero;
            var startPos = tr.position;
            
            while(true){
                var result = dirNormalPos * (speed * Time.deltaTime);
                addPos      += result;
                tr.position += result;

                if (Mathf.Abs(addPos.sqrMagnitude) >= Mathf.Abs(dirPos.sqrMagnitude)){
                    tr.position = startPos + dirPos;
                    break;
                }
                yield return null;
            }
            
            dirPos.Set(dirPos.x * -1f, dirPos.y * -1f, dirPos.z * -1f);
            dirNormalPos = dirPos.normalized;
        }
    }
}