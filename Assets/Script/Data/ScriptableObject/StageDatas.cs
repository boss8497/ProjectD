using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageInfo{
    public int       level;
    public int       patternRate;
    public string    stagePrefabPath;
    public Rect      coninPadding;
    public Pattern[] patterns;
}


[CreateAssetMenu(fileName = "StageDatas", menuName = "Scriptable Object/StageDatas")]
public class StageDatas : ScriptableObject{
    public List<StageInfo> stageInfos;
}