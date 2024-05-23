using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class StageInfo {
    public int       level;
    public int       patternRate;
    public string    stagePrefabPath;
    public Rect      coinPadding;
    public float     coinSize;
    public float     playerSize;
    public float     playerSpeed;
    public float     enemySize;
    public float     enemySpeed;
    public Pattern[] patterns;
    
    public StageInfo Clone(){
        return new StageInfo{
                                level           = level,
                                patternRate     = patternRate,
                                stagePrefabPath = stagePrefabPath,
                                coinPadding     = coinPadding,
                                coinSize        = coinSize,
                                playerSize      = playerSize,
                                playerSpeed     = playerSpeed,
                                enemySize       = enemySize,
                                enemySpeed      = enemySpeed,
                                patterns        = patterns.Select(s => s.Clone()).ToArray()
                            };
    }
}


[CreateAssetMenu(fileName = "StageDatas", menuName = "Scriptable Object/StageDatas")]
public class StageDatas : ScriptableObject{
    public List<StageInfo> stageInfos;
}