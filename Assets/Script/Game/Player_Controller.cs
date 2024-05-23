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
    private float          moveSpeed;

    private SpriteRenderer      mapSr;
    private List<Runtime_Block> blocks = new List<Runtime_Block>();
    private Runtime_Coin        coin;

    private Coroutine angleCoroutine;
    private Coroutine collisionBlockCoroutine;
    private Coroutine movePlayerCoroutine;

    private Vector3 dirVector;
    public Vector3 DirVector => dirVector;

    private bool isMove     = false;
    private bool gameResult = false;
    private void OnEnable(){
        GameManager.SetBlock   += SetBlock;
        GameManager.GameResult += GameResult;
        
    }

    private void OnDisable(){
        GameManager.SetBlock   -= SetBlock;
        GameManager.GameResult -= GameResult;
    }

    private void GameResult(GameResult result){
        StopAllCoroutines();
        ResetCoroutine();
        gameResult = true;
    }

    private void ResetCoroutine(){
        StopAllCoroutines();
        if (angleCoroutine != null){
            StopCoroutine(angleCoroutine);
            angleCoroutine = null;
        }
        if (collisionBlockCoroutine != null){
            StopCoroutine(collisionBlockCoroutine);
            collisionBlockCoroutine = null;
        }
        if (movePlayerCoroutine != null){
            StopCoroutine(movePlayerCoroutine);
            movePlayerCoroutine = null;
        }
    }

    private void SetBlock(List<Runtime_Block> objs){
        if (collisionBlockCoroutine != null){
            StopCoroutine(collisionBlockCoroutine);
            collisionBlockCoroutine = null;
        }
        
        blocks.Clear();
        blocks.AddRange(objs);
        collisionBlockCoroutine = StartCoroutine(CoCollisionBlock());
    }

    public void SetCoin(Runtime_Coin _coin){
        coin = _coin;
    }

    public void Initialize(Direction dir, SpriteRenderer _mapSr, float _size, float _speed){
        ResetCoroutine();
        gameResult = false;
        isMove     = false;

        moveSpeed = _speed;
        mapSr     = _mapSr;
        arrow.gameObject.SetActive(false);
        var tr = transform;
        tr.position   = Vector3.zero;
        tr.localScale = new Vector3(_size, _size, 0);
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
        if (gameResult) return;
        
        if (angleCoroutine != null){
            StopCoroutine(angleCoroutine);
            angleCoroutine = null;
        }

        if (movePlayerCoroutine != null){
            StopCoroutine(movePlayerCoroutine);
            movePlayerCoroutine = null;
        }
        
        arrow.gameObject.SetActive(false);
        movePlayerCoroutine = StartCoroutine(CoMovePlayer());
    }

    IEnumerator CoMovePlayer(){
        isMove = true;
        var tr          = transform;
        var isLeft      = tr.position.x <= 0;
        var targetAngle = arrow.rotation.eulerAngles.z;
        targetAngle = isLeft ? fixedAngle + targetAngle : fixedAngle - targetAngle;
        var speed = isLeft ? moveSpeed : moveSpeed * -1f;
        dirVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * targetAngle) * speed,
                                    Mathf.Sin(Mathf.Deg2Rad * targetAngle) * moveSpeed, 0);

        var mapBounds    = mapSr.bounds;
        var playerBounds = sr.bounds;
        var minVector    = mapBounds.min;
        var maxVector    = mapBounds.max;
        minVector.Set(minVector.x + playerBounds.size.x / 2, minVector.y + playerBounds.size.y / 2, 0);
        maxVector.Set(maxVector.x - playerBounds.size.x / 2, maxVector.y - playerBounds.size.y / 2, 0);


        var resultPos = Vector3.zero;
        while (true){
            tr.position += dirVector;

            if (CoinCollision(coin)){
                coin = null;
                GameManager.OnCollisionCoinEvent();
            }

            if (tr.position.x <= minVector.x){
                resultPos.x = minVector.x;
                resultPos.y = tr.position.y;
                if (tr.position.y <= minVector.y){
                    resultPos.y = minVector.y;
                }
                if (tr.position.y >= maxVector.y){
                    resultPos.y = maxVector.y;
                }
                break;
            }
            
            if (tr.position.x >= maxVector.x){
                resultPos.x = maxVector.x;
                resultPos.y = tr.position.y;
                if (tr.position.y <= minVector.y){
                    resultPos.y = minVector.y;
                }
                if (tr.position.y >= maxVector.y){
                    resultPos.y = maxVector.y;
                }
                break;
            }

            if (tr.position.y <= minVector.y){
                resultPos.y = minVector.y;
                resultPos.x = tr.position.x;
                if (tr.position.x <= minVector.x){
                    resultPos.x = minVector.x;
                }
                if (tr.position.x >= maxVector.x){
                    resultPos.x = maxVector.x;
                }
                break;
            }
            
            if (tr.position.y >= maxVector.y){
                resultPos.y = maxVector.y;
                resultPos.x = tr.position.x;
                if (tr.position.x <= minVector.x){
                    resultPos.x = minVector.x;
                }
                if (tr.position.x >= maxVector.x){
                    resultPos.x = maxVector.x;
                }
                break;
            }
            yield return null;
            
        }
        tr.position = resultPos;
        isMove      = false;
    }
    
    IEnumerator CoCollisionBlock(){
        while (true){
            foreach (var block in blocks){
                switch (block.blockinfo.type){
                    case BlockType.Circle:{
                        if (CircleAndCircleCollision(sr.bounds, transform.position, 
                                                     block.controller.blockSr.bounds, block.gameObject.transform.position)){
                            ResetCoroutine();
                            GameManager.OnCollisionBlockEvent();
                            yield break;
                        }
                    }
                        break;
                    case BlockType.Rect:
                        break;
                }
            }

            yield return null;
        }
    }

    private bool CircleAndCircleCollision(Bounds lbounds, Vector3 lpos, Bounds rbounds, Vector3 rpos){
        if (Vector3.Distance(lpos, rpos) < lbounds.size.x * 0.5f + rbounds.size.x * 0.5f){
            return true;
        }
        return false;
    }

    public bool CoinCollision(Runtime_Coin _coin){
        if (_coin == null) return false;
        return CircleAndCircleCollision(sr.bounds, transform.position,
                                        _coin.sr.bounds, _coin.gameObject.transform.position);
    }

    public bool CreateCoinCollision(Runtime_Coin _coin){
        if (_coin == null) return false;
        var tr       = transform;
        var position = tr.position;
        var coinPos  = _coin.gameObject.transform.position;
        var vVecotr  = position + (DirVector.normalized * Vector3.Distance(position, coinPos));
            
        return CircleAndCircleCollision(sr.bounds, position,
                                        _coin.sr.bounds, coinPos) ||
               CircleAndCircleCollision(sr.bounds, vVecotr,
                                        _coin.sr.bounds, coinPos);
    }

    private void OnMouseDown(){
        if (isMove || gameResult) return;
        if (angleCoroutine != null){
            StopCoroutine(angleCoroutine);
            angleCoroutine = null;
        }
        
        arrow.gameObject.SetActive(true);
        arrow.rotation = new Quaternion();

        var tr          = arrow;
        var dirPos      = tr.position;
        var targetAngle = Mathf.Acos(dirPos.x / dirPos.magnitude) * Mathf.Rad2Deg;
        var dotResult   = Vector3.Dot(tr.up, Vector3.zero - dirPos);
        var resultAngle = dotResult >= 0 ? fixedAngle - targetAngle : fixedAngle + targetAngle;

        angleCoroutine = StartCoroutine(CoMoveAngle(resultAngle));
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
                arrow.Rotate(new Vector3(0, 0, deltaSpeed));

                if (Mathf.Abs(sumAngel) >= angle){
                    if (sumAngel < 0){
                        arrow.rotation = Quaternion.Euler(0, 0, startAngle);
                    }
                    else{
                        arrow.rotation = Quaternion.Euler(0, 0, endAngle);
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