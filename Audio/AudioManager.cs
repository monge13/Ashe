using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Ashe
{
    /// <summary>
    /// Audioの再生管理を行う
    /// 同時SE制限やBGMの切り替えなど
    /// </summary>
    public class AudioManager : Pattern.SingletonMonoBehaviour<AudioManager>
    {
        /// <summary>
        /// AudioObjectのPool
        /// </summary>
        private PooledObjectManager<AudioObject> objectPool = new PooledObjectManager<AudioObject>();

        /// <summary>
        /// AudioObjectのプレハブ
        /// </summary>
        [SerializeField]
        private AudioObject prefab;

        /// <summary>
        /// AudioClipを保存する
        /// </summary>
        Collection.UIntKeyDictionary<AudioClip> clips = new Collection.UIntKeyDictionary<AudioClip>();

        /// <summary>
        /// 初期にAudioObjectをプールする数
        /// </summary>
        private int initialAudioObjectNum = 10;

        const string POOLED_OBJECT_NAME = "AudioObject";

        /// <summary>
        /// 初期化処理
        /// </summary>
        protected override void Init()
        {
            objectPool.Pool(POOLED_OBJECT_NAME, prefab, initialAudioObjectNum);
        }

        /// <summary>
        /// SEを再生する 
        /// </summary>
        public void PlaySE(string name)
        {
            uint hash = (uint)name.GetHashCode();
            AudioClip clip;
            // AudioClipがロードされてない場合はロード処理を行う
            if(!clips.TryGetValue(hash, out clip)){
                string path = Path.Join("Audio", name);

                Debug.Log.I(path);
                // TODO: ResourceManager
                clip = Resources.Load<AudioClip>(path);
                clips.Add(hash, clip);
            }
            if(clip == null) return;

            AudioObject audioObject = objectPool.Get(POOLED_OBJECT_NAME);
            audioObject.gameObject.SetActive(true);
            audioObject.Play(clip);
        }
    }
}