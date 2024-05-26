using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum MoveDirection{
    Horizontal = 0,
    Vertical,
}

public class Block_Controller : MonoBehaviour{
    public SpriteRenderer blockSr;

    private Block          blockinfo;
    private Transform      map;
    private SpriteRenderer mapSr;
    private StageInfo      stageInfo;

    private Direction     currentDir;
    private MoveDirection moveDir;

    private Vector3[] edgePos;

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
        moveDir   = MoveDirection.Horizontal;
        InitEdge();
        InitPosition();
        StopMove();
    }

    public void InitPosition(){
        currentDir         = blockinfo.startDirection;
        transform.position = edgePos[(int)currentDir];
    }

    private void InitEdge(){
        var mapBounds   = mapSr.bounds;
        var blockBounds = blockSr.bounds;
        edgePos = new Vector3[Enum.GetValues(typeof(Direction)).Length];
        edgePos[(int)Direction.TopLeft] = new Vector3(mapBounds.size.x / 2 - blockBounds.size.x / 2,
                                                      mapBounds.size.y / 2 - blockBounds.size.y / 2, 0);
        edgePos[(int)Direction.TopRight] = new Vector3(blockBounds.size.x / 2 - mapBounds.size.x   / 2,
                                                       mapBounds.size.y   / 2 - blockBounds.size.y / 2, 0);

        edgePos[(int)Direction.BottomLeft] = new Vector3(mapBounds.size.x   / 2 - blockBounds.size.x / 2,
                                                         blockBounds.size.y / 2 - mapBounds.size.y   / 2, 0);
        edgePos[(int)Direction.BottomRight] = new Vector3(blockBounds.size.x / 2 - mapBounds.size.x / 2,
                                                          blockBounds.size.y / 2 - mapBounds.size.y / 2, 0);
    }

    public void StopMove(){
        StopAllCoroutines();
    }

    private MoveDirection GetDirection(int score = 0){
        return (MoveDirection)(score / stageInfo.patternRate % 2);
    }

    public void MoveToNextPosition(int score){
        var nextMoveDir = GetDirection(score);
        var changeFlag  = moveDir != nextMoveDir;
        if (changeFlag){
            moveDir = nextMoveDir;
            StopAllCoroutines();
            StartCoroutine(CoMoveToStartPosition());
        }
    }

    private (Direction, Vector3) GetNextPosition(){
        var pos     = transform.position;
        var minDis = float.MaxValue;
        var dir    = Direction.BottomLeft;

        for (var i = 0; i < edgePos.Length; ++i){
            var dis = Mathf.Abs(Vector3.Distance(pos, edgePos[i]));
            if (dis < minDis){
                minDis = dis;
                dir    = (Direction)i;
            }
        }

        return (dir, edgePos[(int)dir]);
    }

    IEnumerator CoMoveToStartPosition(){
        var (nextDir, nextPosition)= GetNextPosition();
        var tr        = transform;
        var dir       = nextPosition - tr.position;
        var dirNormal = dir.normalized;
        var speed     = stageInfo.enemySpeed;

        while (true){
            tr.position += dirNormal * (speed * Time.deltaTime);

            if (Vector3.Distance(tr.position, nextPosition) <= 0.1f){
                tr.position = nextPosition;
                break;
            }

            yield return null;
        }
        currentDir = nextDir;
        StartCoroutine(CoMove(speed));
    }

    public void StartMove(){
        gameObject.SetActive(true);
        StartCoroutine(CoMove(stageInfo.enemySpeed));
    }

    IEnumerator CoMove(float speed){
        var tr            = transform;
        var startPosition = tr.position;
        var endPosition   = startPosition;
        if (moveDir == MoveDirection.Horizontal){
            endPosition.x = startPosition.x * -1f;
        }
        else{
            endPosition.y = startPosition.y * -1f;
        }


        var dirPos       = (endPosition - startPosition);
        var dirNormalPos = dirPos.normalized;

        while (true){
            var addPos   = Vector3.zero;
            var startPos = tr.position;

            while (true){
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