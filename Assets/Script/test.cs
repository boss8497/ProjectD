using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour{
    public GameObject parent;

    private List<GameObject> objects = new List<GameObject>();


    public async void CreateLoadAssets(){
        var obj = await AddressableManager.Instance.LoadAsset<GameObject>("Assets/Prefab/Circle.prefab");
        var circle = Instantiate(obj, parent.transform);
        circle.transform.localPosition = Vector3.zero;
        objects.Add(circle);
    }

    public void Clear(){
        foreach (var obj in objects){
            Destroy(obj);
        }
    }
}
