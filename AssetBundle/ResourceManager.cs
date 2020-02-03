using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ashe
{
    public class ResourceManager : ObjectBase
    {
        // アセットバンドルを保持するディクショナリのデフォルトのキャパシティ 
        const int DEFAULT_RESOURCES_CAPACITY = 200;
        // AssetBundle名をキーとしたアセットのディクショナリ  
        Dictionary<string, ReferenceCountedBundle> resources = new Dictionary<string, ReferenceCountedBundle>(DEFAULT_RESOURCES_CAPACITY);
        // バージョンが書き込まれたファイルの拡張子 
        const string VERSION_FILE_EXT = ".version";
        // パス生成用 
        StringBuilder stringBuilder = new StringBuilder(128);
        // シングルマニフェスト 
        AssetBundleManifest singleManifest;

        // システムクラスなのでオーダーはシステム 
        public override int order
        {
            get
            {
                return ObjectOrder.ORDER_SYSTEM;
            }
        }

        // 依存関係のためにアセットバンドルを保持し続ける必要がある場合はkeepAssetBundleをtrueにする 
        public void Load(string assetBundleName, System.Action<ResourceHolder> onComplete, bool keepAssetbundle = false)
        {
            Debug.Assert(singleManifest != null, "SingleManifest has not loaded yet");

            ReferenceCountedBundle resource;
            // すでにロード済みかどうか 
            if (resources.TryGetValue(assetBundleName, out resource))
            {
                if (onComplete != null)
                {
                    onComplete(new ResourceHolder(resource));
                }
                return;
            }

            // ロード済みでなかった場合、依存関係を解決するリクエストを行う 
            string[] dependAssetBundleNames = singleManifest.GetAllDependencies(assetBundleName);

            // TODO: 依存関係のあるリソースがすべて読み込まれているかのチェック 
            // TODO: 上記に足りない場合依存関係の構築 


            stringBuilder.Length = 0;
            string cachePath = stringBuilder.AppendFormat("{0}/{1}", Application.persistentDataPath, assetBundleName).ToString();
            string versionPath = stringBuilder.Append(VERSION_FILE_EXT).ToString();

            bool isCached = false;
            // キャッシュに存在するかどうか 
            if (File.Exists(cachePath) && File.Exists(versionPath))
            {
                string cachedHash = File.ReadAllText(versionPath);
                string assetBundleHash = singleManifest.GetAssetBundleHash(assetBundleName).ToString();
                // バージョン比較 
                isCached = assetBundleHash == cachedHash;
                // バージョンが一致していなければキャッシュを削除 
                if (!isCached)
                {
                    File.Delete(cachePath);
                }
            }

            // ロード 
            // 暗号化解除 
        }
    }
}