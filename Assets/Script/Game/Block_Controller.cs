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

    private Vector3   startPosition;
    private Vector3   endPosition;
    private Direction currentDire;

    private void OnEnable(){
        GameManager.GameResult += GameResult;
    }

    private void OnDisable(){
        GameManager.GameResult -= GameResult;
    }

    private void GameResult(GameResult result){
        StopAllCoroutines();
    }


    public void Init(Block _block, Transform _map, SpriteRenderer _mapSr){
        blockinfo = _block;
        map       = _map;
        mapSr     = _mapSr;
        InitPosition();
        StopMove();
    }

    public void InitPosition(){
        var (start, end)   = GetPosition(0);
        startPosition      = start;
        transform.position = startPosition;
        endPosition        = end;
    }

    private (Vector3 start, Vector3 end) GetPosition(int score = 0){
        startPosition = Vector3.zero;
        endPosition   = Vector3.zero;
        
        var tr = transform;
        tr.localScale = new Vector3(blockinfo.size, blockinfo.size, tr.localScale.z);
        
        var mapBounds   = mapSr.bounds;
        var blockBounds = blockSr.bounds;
        currentDire = blockinfo.direction[score % blockinfo.direction.Count];
        switch (currentDire){
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
        var (start, end)   = GetPosition(score);
        startPosition      = start;
        endPosition        = end;
        StartCoroutine(CoMoveToStartPosition());
    }

    IEnumerator CoMoveToStartPosition(){
        var tr        = transform;
        var dir       =  startPosition - tr.position;
        var dirNormal = dir.normalized;

        while (true){
            tr.position += dirNormal * (blockinfo.speed * 3 * Time.deltaTime);

            if (Vector3.Distance(tr.position, startPosition) <= 0.1f){
                tr.position = startPosition;
                break;
            }

            yield return null;
        }
        StartCoroutine(CoMove(blockinfo.speed));
    }

    public void StartMove(){
        gameObject.SetActive(true);
        StartCoroutine(CoMove(blockinfo.speed));
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