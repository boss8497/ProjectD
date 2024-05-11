using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectPoolingManager : MonoBehaviour{
    public  static   ObjectPoolingManager                      Instance;
    public           GameObject                                content;
    private          Dictionary<PoolingKey, Stack<GameObject>> pool = new ();
    private          Dictionary<PoolingKey, ObjectPoolingData> poolDataDic;
    private          bool                                      destroyObject = false;
    private readonly string                                    dataPath          = "Assets/Data/ObjectPoolingDatas.asset";
    private void Awake(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    private void OnDestroy(){
        destroyObject = true;
    }

    private async Task LoadData(){
        var poolDatas = await AddressableManager.Instance.LoadAsset<ObjectPoolingDatas>(dataPath);
        poolDataDic = poolDatas.objectPoolingDatas.ToDictionary(r => r.key);
    }

    public async Task Initialize(){
        await LoadData();
        await LoadObject();
    }

    public void Release(){
        foreach (var data in pool){
            var objectStack = data.Value;
            while (objectStack.Count > 0){
                var obj = objectStack.Pop();
                Destroy(obj);
            }
        }
    }

    private async Task LoadObject(){
        foreach (var poolData in poolDataDic){
            var poolList = new Stack<GameObject>();
            var path     = poolData.Value.path;
            var obj      = await AddressableManager.Instance.LoadAsset<GameObject>(path);
            for (var i = 0; i < poolData.Value.maxCount; ++i){
                var ins = Instantiate(obj, content.transform);
                ins.transform.localPosition = Vector3.zero;
                poolList.Push(ins);
                ins.SetActive(false);
            }
            pool[poolData.Key] = poolList;
        }
    }


    public ObjectPoolingData GetPoolingData(PoolingKey key){
        if (poolDataDic.ContainsKey(key)){
            return poolDataDic[key];
        }
        return null;
    }

    public GameObject Pop(PoolingKey key){
        return pool[key].Pop();
    }
    
    public GameObject Pop(PoolingKey key, Transform parent, bool active = true){
        var obj = pool[key].Pop();
        obj.SetActive(active);
        obj.transform.parent        = parent;
        obj.transform.localPosition = Vector3.zero;
        return obj;
    }
    
    public void Push(GameObject obj){
        if (obj == null) return;
        var poolObject = obj.GetComponent<PoolObject>();
        if (poolObject != null){
            Instance.Push(obj, poolObject.key);
        }
        else{
            Destroy(obj);
        }
    } 

    public void Push(GameObject obj, PoolingKey key){
        if (destroyObject) return;
        var tr = obj.transform;
        tr.parent        = content.transform;
        tr.localPosition = Vector3.zero;
        obj.SetActive(false);


        var poolinfo = GetPoolingData(key);
        if (poolinfo == null){
            GameObject.Destroy(obj);
            return;
        }

        if (poolinfo.maxCount <= pool[key].Count){
            GameObject.Destroy(obj);
        }
        else{
            pool[key].Push(obj);
        }
    }
}