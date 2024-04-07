using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour{
    public SpriteRenderer sr;
    public Transform      arrow;
    public float          fixedAngle = 90f;
    public float          angle;
    public float          angleRotateSpeed;
    public float          moveSpeed;

    private SpriteRenderer mapSr;

    public void Initialize(Direction dir, SpriteRenderer _mapSr){
        mapSr = _mapSr;
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

    private void OnMouseUp(){
        StopAllCoroutines();
        arrow.gameObject.SetActive(false);
        StartCoroutine(CoMovePlayer());
    }

    IEnumerator CoMovePlayer(){
        var tr          = transform;
        var isLeft      = tr.position.x <= 0;
        var targetAngle = arrow.rotation.eulerAngles.z;
        var dirVector   = new Vector3(isLeft ? moveSpeed : moveSpeed * -1, Mathf.Cos(Mathf.Deg2Rad * targetAngle) * moveSpeed, 0);

        var mapBounds    = mapSr.bounds;
        var playerBounds = sr.bounds;
        var minVector   = mapBounds.min;
        var maxVector   = mapBounds.max;
        minVector.Set(minVector.x + playerBounds.size.x / 2, minVector.y + playerBounds.size.y / 2, 0);
        maxVector.Set(maxVector.x - playerBounds.size.x / 2, maxVector.y - playerBounds.size.y / 2, 0);
        

        while (true){
            tr.position += dirVector;

            if (tr.position.x <= minVector.x){
                tr.position = new Vector3(minVector.x, tr.position.y);
                break;
            }
            
            if (tr.position.y <= minVector.y){
                tr.position = new Vector3(tr.position.x, minVector.y);
                break;
            }
            
            if (tr.position.x >= maxVector.x){
                tr.position = new Vector3(maxVector.x, tr.position.y);
                break;
            }
            
            if (tr.position.y >= maxVector.y){
                tr.position = new Vector3(tr.position.x, maxVector.y);
                break;
            }
            
            yield return null;
        }
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
        
        StartCoroutine(CoMoveAngle(resultAngle));
    }

    IEnumerator CoMoveAngle(float targetAngel){
        var radiAngle  = angle / 2;
        var startAngle = targetAngel - radiAngle;
        var endAngle   = targetAngel + radiAngle;
        var speed      = angleRotateSpeed;
        
        arrow.Rotate(new Vector3(0, 0, startAngle));

        while (true){
            var sumAngel = 0f;
            while (true){
                var deltaSpeed = speed;
                sumAngel += deltaSpeed;
                arrow.Rotate(new Vector3(0,0, deltaSpeed));
            
                if (Mathf.Abs(sumAngel) >= angle){
                    if (sumAngel < 0){
                        arrow.rotation = Quaternion.Euler(0,0, startAngle);
                    }
                    else{
                        arrow.rotation = Quaternion.Euler(0,0, endAngle);
                    }
                    break;
                }
                yield return null;
            }

            speed *= -1;
            yield return null;
        }
    }
}