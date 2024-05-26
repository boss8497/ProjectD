using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction{
    TopLeft = 0,
    TopRight,
    BottomLeft,
    BottomRight,
}
public enum BlockType{
    Circle,
    Rect
}

[Serializable]
public class Block {
    public PoolingKey poolingKey;
    public BlockType  type;
    public Direction  startDirection;
    public Block Clone(){
        return new Block{ poolingKey = poolingKey, type = type, startDirection = startDirection };
    }
}