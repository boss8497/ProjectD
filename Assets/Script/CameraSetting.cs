using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraSetting : MonoBehaviour{
    public Camera camera;
    public int    baseWidth  = 1920;
    public int    baseHeight = 1080;

    public  float          resoultionDelay = 0.3f;
    private int            lastWidth;
    private int            lastHeight;
    private float          baseSize;
    private WaitForSeconds waitForSeconds;

    private void Awake(){
        if (camera == null){
            camera = GetComponent<Camera>();
        }

        if (camera == null){
            Debug.LogError($"Camera not found");
            return;
        }

        baseSize = camera.orthographicSize;
        StartCoroutine(CoResolutionChanged());
        waitForSeconds = new WaitForSeconds(resoultionDelay);
    }

    private void OnDestroy(){
        StopAllCoroutines();
    }

    private IEnumerator CoResolutionChanged(){
        while (true){
            if (lastWidth != Screen.width || lastHeight != Screen.height){
                lastWidth  = Screen.width;
                lastHeight = Screen.height;
                ResolutionChanged();
            }

            yield return waitForSeconds;
        }
    }

    public void ResolutionChanged(){
        lastWidth  = Screen.width;
        lastHeight = Screen.height;

        var baseRate = (float)baseWidth / baseHeight;
        var lastRate = (float)lastWidth / lastHeight;
        
        if (baseRate < lastRate){
            var newWidth = baseRate / lastRate;
            camera.rect = new Rect((1f - newWidth) / 2f, 0, newWidth, 1f);
        }
        else{
            var newHeight = lastRate / baseRate;
            camera.rect = new Rect(0, (1f - newHeight) / 2f, 1f, newHeight);
        }

        Screen.SetResolution(baseWidth, (int)((float)lastHeight / lastWidth * baseWidth),true);
    }
}