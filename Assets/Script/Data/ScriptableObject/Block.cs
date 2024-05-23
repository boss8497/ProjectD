using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction{
    Top,
    Bottom,
    Left,
    Right,
}
public enum BlockType{
    Circle,
    Rect
}

[Serializable]
public class Block {
    public PoolingKey      poolingKey;
    public BlockType       type;
    public List<Direction> direction;
    public Block Clone(){
        return new Block{ poolingKey = poolingKey, type = type, direction = direction.ToList() };
    }
}