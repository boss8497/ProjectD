using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour{
    public SpriteRenderer sr;
    public Transform      arrow;
    public float          fixedAngle = 90f;
    public float          angle;

    public void Initialize(Direction dir, SpriteRenderer mapSr){
        arrow.gameObject.SetActive(false);
        var tr = transform;
        tr.position = Vector3.zero;
        var mapBounds     = mapSr.bounds;
        var playerBounds  = sr.bounds;
        var startPosition = Vector3.zero;

        switch (dir){
            case Direction.Top:
                startPosition.y = mapBounds.size.y / 2 - playerBounds.size.y / 2;
                break;
            case Direction.Bottom:
                startPosition.y = playerBounds.size.y / 2 - mapBounds.size.y / 2;
                break;
            case Direction.Left:
                startPosition.x = playerBounds.size.x / 2 - mapBounds.size.x / 2;
                break;
            case Direction.Right:
                startPosition.x = mapBounds.size.x / 2 - playerBounds.size.x / 2;
                break;
        }

        tr.position = startPosition;
    }

    private void OnMouseDown(){
        StopAllCoroutines();
        arrow.gameObject.SetActive(true);
        arrow.rotation = new Quaternion();
        
        var tr          = arrow;
        var dirPos      = tr.position;
        var targetAngle = Mathf.Acos(dirPos.x / dirPos.magnitude) * Mathf.Rad2Deg;
        var dotResult   = Vector3.Dot(tr.up,  Vector3.zero - dirPos);
        var resultAngle = dotResult >= 0 ? fixedAngle - targetAngle : fixedAngle + targetAngle;
        
        tr.Rotate(new Vector3(0, 0, resultAngle));
        StartCoroutine(CoMoveAngle(targetAngle));
    }

    IEnumerator CoMoveAngle(float targetAngel){
        yield return null;
    }
}