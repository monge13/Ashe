using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace Ashe
{
    public class AssetBundleloader : ObjectBase
    {
        // 最大同時にロードする数
        int _maxLoadNum = 5;
        public int maxLoadNum
        {
            get { return _maxLoadNum; }
            set
            {
                _maxLoadNum = value;
                // TODO : Loadのコルーチンの増減 
            }
        }

        // タイムアウトする時間 
        float _timeoutTime = 60.0f;
        public float timeoutTime
        {
            get { return _timeoutTime; }
            set { _timeoutTime = value; }
        }

        // リトライインターバル時間 
        float _retryInterval = 3.0f;
        public float retryInterval
        {
            get { return _retryInterval; }
            set
            {
                _retryInterval = value;
                waitForInterval = new WaitForSeconds(_retryInterval);
            }
        }
        WaitForSeconds waitForInterval;

        // ロード失敗時のリトライ回数 
        int _maxRetryNum = 5;
        public int maxRetryNum
        {
            get { return _maxRetryNum; }
            set { _maxRetryNum = value; }
        }

        // キャッシュからのロードとWebからのダウンロードの両方を抽象化する 
        public class LoadAssetBundleRequest : System.IDisposable
        {
            AssetBundleCreateRequest loadFromCacheRequest;
            UnityWebRequest downloadRequest;
            string path;
            bool isCached;
            bool disposed;

            public LoadAssetBundleRequest(string path, bool isCached)
            {
                this.path = path;
                this.isCached = isCached;
                CreateLoadRequest();
            }

            // 現在の命令をアボートしてRetryを行う 
            public void Retry()
            {
                Abort();
                CreateLoadRequest();
            }

            void CreateLoadRequest()
            {
                if (isCached)
                {
                    loadFromCacheRequest = AssetBundle.LoadFromFileAsync(path);
                }
                else
                {
                    // hash128が指定されている場合はキャッシングシステムが使われてしまうので指定しない 
                    downloadRequest = UnityWebRequestAssetBundle.GetAssetBundle(path, 0);
                }
            }

            ~LoadAssetBundleRequest()
            {
                Dispose();
            }

            // ロードが完了したかどうか 
            public bool isDone()
            {
                if (loadFromCacheRequest != null)
                {
                    return loadFromCacheRequest.isDone;
                }
                return downloadRequest.isDone;
            }

            // ロードのプログレスの取得 
            public float GetProgress()
            {
                if (loadFromCacheRequest != null)
                {
                    return loadFromCacheRequest.progress;
                }
                return downloadRequest.downloadProgress;
            }

            // ロード処理の停止 
            public void Abort()
            {
                if (loadFromCacheRequest != null)
                {
                    loadFromCacheRequest = null;
                }
                else
                {
                    downloadRequest.Abort();
                }
                Dispose();
            }

            // ロード命令の破棄 
            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }
                disposed = true;

                if (loadFromCacheRequest != null)
                {
                    loadFromCacheRequest = null;
                    return;
                }
                downloadRequest.Dispose();
                downloadRequest = null;
            }
        }

        // Queueにリクエストが詰められるとコルーチンがスタートしてロードを行う 
        const int DEFUALT_REQUEST_QUEUE_CAPACITY = 100;
        Queue<LoadAssetBundleRequest> requestQueue = new Queue<LoadAssetBundleRequest>(DEFUALT_REQUEST_QUEUE_CAPACITY);


        protected override bool Initialize(uint callCount)
        {
            waitForInterval = new WaitForSeconds(_retryInterval);
            for (int i = 0; i < maxLoadNum; ++i)
            {
                StartCoroutine(Load());
            }
            return base.Initialize(callCount);
        }

        public void Load(string path, bool isCached)
        {

        }

        enum ERROR
        {
            NONE,
            TIMEOUT,
        }

        private IEnumerator Load()
        {
            WaitUntil waitUntil = new WaitUntil(() =>
            {
                return requestQueue.Count > 0;
            });
            while (true)
            {
                // requestQueueにリクエストがあるまで待機 
                yield return waitUntil;
                int retryCount = 0;
                LoadAssetBundleRequest request = requestQueue.Dequeue();
                float timer = 0f;

                while (!request.isDone())
                {
                    timer += Time.deltaTime;
                    // タイムアウトのチェック 
                    if (timer >= _timeoutTime)
                    {
                        if (retryCount < _maxRetryNum)
                        {
                            timer = 0f;
                            request.Retry();
                            ++retryCount;
                        }
                    }
                }
            }
        }

    }
}