using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Pattern{
    public Block[] blocks;

    public Pattern Clone(){
        return new Pattern{
                              blocks = blocks.Select(s => s.Clone()).ToArray()
                          };
    }
}