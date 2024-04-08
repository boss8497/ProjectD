using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolingKey{
    None = 0,
    Block,
    Coin,
    Player,
}

[Serializable]
public class ObjectPoolingData{
    public PoolingKey key;
    public string     path;
    public int        maxCount;
}

[CreateAssetMenu(fileName = "ObjectPoolingDatas", menuName = "Scriptable Object/ObjectPoolingDatas")]
public class ObjectPoolingDatas : ScriptableObject{
    public List<ObjectPoolingData> objectPoolingDatas;
}