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
    }

    private void SetPosition(){
        var tr = transform;
        tr.position = Vector3.zero;
        var pos = tr.position;
        tr.localScale = new Vector3(blockinfo.size, blockinfo.size, tr.localScale.z);

        switch (blockinfo.direction){
            case Direction.Top:
                pos.y = mapSr.size.y / 2 - blockSr.bounds.size.y / 2;
                break;
            case Direction.Bottom:
                pos.y = blockSr.bounds.size.y / 2 - mapSr.size.y / 2;
                break;
            case Direction.Left:
                pos.x = mapSr.size.x / 2 - blockSr.bounds.size.x / 2;
                break;
            case Direction.Right:
                pos.x = blockSr.bounds.size.x / 2 - mapSr.size.x / 2;
                break;
        }

        switch (blockinfo.moveDirection){
            case MoveDirection.TopToBottom:
                pos.y = mapSr.size.y / 2 - blockSr.bounds.size.y / 2;
                break;
            case MoveDirection.BottomToTop:
                pos.y = blockSr.bounds.size.y / 2 - mapSr.size.y / 2;
                break;
            case MoveDirection.LeftToRight:
                pos.x = mapSr.size.x / 2 - blockSr.bounds.size.x / 2;
                break;
            case MoveDirection.RightToLeft:
                pos.x = blockSr.bounds.size.x / 2 - mapSr.size.x / 2;
                break;
        }

        startPosition = transform.position;
    }
}