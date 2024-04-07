using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction{
    Top,
    Bottom,
    Left,
    Right,
}

public enum MoveDirection{
    TopToBottom,
    BottomToTop,
    LeftToRight,
    RightToLeft,
}

public enum BlockType{
    Circle,
    Rect
}

[Serializable]
public class Block{
    public PoolingKey    poolingKey;
    public BlockType     type;
    public Direction     direction;
    public MoveDirection moveDirection;
    public float         size;
    public float         speed;
}