//#define RESOURCE_DEBUG
using UnityEngine;
using System;

// ユーザーがさわることのできるリソース
public class ResourceHolder : IDisposable {

    ReferenceCountedBundle refCountObj;
    bool disposed;

    // ReferenceCountedResourceはResourceManagerのみが持っているため 
    // ResourceHolderはResourceManager以外では生成不可能とする 
    public ResourceHolder(ReferenceCountedBundle refObj)
    {
#if UNITY_EDITOR && RESOURCE_DEBUG
        bool isCalledFromResourceManager = StackTraceUtility.ExtractStackTrace().Contains("ResourceManager");
        Debug.Assert(isCalledFromResourceManager, "ResourceHolder is created from not res manager!!");
#endif
        refCountObj = refObj;
        refCountObj.Add();
    }

    // デストラクタ 
    ~ResourceHolder()
    {
        if (!disposed)
        {
            Dispose();
        }
    }

    public void Dispose()
    {
        refCountObj.Sub();
        refCountObj = null;
        disposed = true;
    }
}

// 参照カウンタで管理されるオブジェクト 
public class ReferenceCountedBundle
{
    // 参照カウンタ 
    int refCount;
    // アセットバンドル名  
    public string assetBundleName
    {
        get; private set;
    }
    // Resource 
    public AssetBundle assetbundle
    {
        get; private set;
    }

    public ReferenceCountedBundle(string assetBundleName, AssetBundle obj)
    {
        this.assetBundleName = assetBundleName;
        assetbundle = obj;
    }

    public void Add()
    {
        ++refCount;
    }

    public void Sub()
    {
        --refCount;
        if( refCount <= 0)
        {
            // TODO : ResourceManagerからの破棄 

        }
    }
}