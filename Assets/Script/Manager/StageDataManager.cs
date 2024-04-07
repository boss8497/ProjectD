using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class StageDataManager {
    public static    StageDataManager           Instance;
    private          Dictionary<int, StageInfo> levelDic;
    private readonly string                     dataPath = "Assets/Data/StageDatas.asset";

    private async Task LoadData(){
        var stageDatas = await AddressableManager.Instance.LoadAsset<StageDatas>(dataPath);
        levelDic = stageDatas.stageInfos.ToDictionary(r => r.level);
    }

    public async Task Initialize(){
        await LoadData();
    }

    public StageInfo GetStageInfo(int level){
        if (levelDic.ContainsKey(level)){
            return levelDic[level];
        }
        return null;
    }
}