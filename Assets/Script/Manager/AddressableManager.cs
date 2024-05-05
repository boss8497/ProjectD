using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableManager{
    public static   AddressableManager Instance;
    public readonly TimeSpan           TimeOut = new(TimeSpan.TicksPerSecond * 10);

    private bool initialized = false;

    public bool Valid(){
        if (!initialized){
            return false;
        }

        return true;
    }

    public void Initialize(){
        Addressables.InitializeAsync().Completed += Initialize_Completed;
    }

    private void Initialize_Completed(AsyncOperationHandle<IResourceLocator> obj){
        if (obj.IsDone == false)
            throw new Exception($"Addressable Init Failed");
        initialized = true;
    }

    public async Task<T> LoadAsset<T>(string path, Action<T> callback = null){
        if (!Valid()){
            throw new Exception("Addressable is not Init");
        }

        var handle = Addressables.LoadAssetAsync<T>(path);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Failed){
            throw new Exception("LoadAsset Failed");
        }

        var result = handle.Result;
        callback?.Invoke(result);

        //Addressables.Release(handle);
        return result;
    }

    public async void Download(IResourceLocator result){
        foreach (var key in result.Keys){
            var completed = false;

            Addressables.DownloadDependenciesAsync(key).Completed += handle => { completed = true; };

            var waitTime = 0.0f;
            while (completed == false){
                await Task.Delay(1);
                waitTime += Time.deltaTime;
                if (waitTime > TimeOut.Seconds){
                    break;
                }
            }
        }

        initialized = true;
    }
}