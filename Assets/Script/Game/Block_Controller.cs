using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block_Controller : MonoBehaviour{
    public SpriteRenderer blockSr;

    private Block          blockinfo;
    private Transform      map;
    private SpriteRenderer mapSr;

    private Vector3 startPosition;
    private Vector3 endPosition;

    public void SetData(Block _block, Transform _map){
        blockinfo = _block;
        map       = _map;
        mapSr     = map.GetComponent<SpriteRenderer>();
        SetPosition();
        StopMove();
    }

    private void SetPosition(){
        startPosition = Vector3.zero;
        endPosition   = Vector3.zero;
        
        var tr = transform;
        tr.position = Vector3.zero;
        tr.localScale = new Vector3(blockinfo.size, blockinfo.size, tr.localScale.z);
        
        var mapBounds   = mapSr.bounds;
        var blockBounds = blockSr.bounds;
        
        switch (blockinfo.direction){
            case Direction.Top:
                startPosition.y = mapBounds.size.y / 2 - blockBounds.size.y / 2;
                endPosition.y   = startPosition.y;
                break;
            case Direction.Bottom:
                startPosition.y = blockBounds.size.y / 2 - mapBounds.size.y / 2;
                endPosition.y   = startPosition.y;
                break;
            case Direction.Left:
                startPosition.x = blockBounds.size.x / 2 - mapBounds.size.x / 2;
                endPosition.x   = startPosition.x;
                break;
            case Direction.Right:
                startPosition.x = mapBounds.size.x / 2 - blockBounds.size.x / 2;
                endPosition.x   = startPosition.x;
                break;
        }

        switch (blockinfo.moveDirection){
            case MoveDirection.TopToBottom:
                startPosition.y         = mapBounds.size.y   / 2 - blockBounds.size.y / 2;
                endPosition.y = blockBounds.size.y / 2 - mapBounds.size.y   / 2;
                break;
            case MoveDirection.BottomToTop:
                startPosition.y         = blockBounds.size.y / 2 - mapBounds.size.y   / 2;
                endPosition.y = mapBounds.size.y   / 2 - blockBounds.size.y / 2;
                break;
            case MoveDirection.LeftToRight:
                startPosition.x         = blockBounds.size.x / 2 - mapBounds.size.x   / 2;
                endPosition.x = mapBounds.size.x   / 2 - blockBounds.size.x / 2;
                break;
            case MoveDirection.RightToLeft:
                startPosition.x         = mapBounds.size.x   / 2 - blockBounds.size.x / 2;
                endPosition.x = blockBounds.size.x / 2 - mapBounds.size.x   / 2;
                break;
        }

        transform.position = startPosition;
    }

    public void StopMove(){
        StopAllCoroutines();
        gameObject.SetActive(false);
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