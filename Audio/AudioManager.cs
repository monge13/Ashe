using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Ashe
{
    /// <summary>
    /// Audioの再生管理を行う
    /// 同時SE制限やBGMの切り替えなど
    /// </summary>
    public class AudioManager : Pattern.SingletonMonoBehaviour<AudioManager>
    {
        /// <summary>
        /// 使用するミキサー
        /// </summary>
        [SerializeField]
        AudioMixer mixer;

        /// <summary>
        /// AudioObjectのPool
        /// </summary>
        private PooledObjectManager<AudioObject> objectPool = new PooledObjectManager<AudioObject>();

        /// <summary>
        /// AudioObjectのプレハブ
        /// </summary>
        [SerializeField]
        private AudioObject audioObjectPrefab;

        /// <summary>
        /// AudioClipを保存する
        /// </summary>
        Collection.UIntKeyDictionary<AudioClip> clips = new Collection.UIntKeyDictionary<AudioClip>();

        /// <summary>
        /// BGM再生システムのプレハブ
        /// </summary>
        [SerializeField]
        private BGMPlayer bgmPlayerPrefab;

        /// <summary>
        /// 再生中のAudioObjec
        /// </summary>
        List<AudioObject> playingAudioObjects = new List<AudioObject>(10);

        /// <summary>
        /// BGM再生の管理
        /// </summary>
        /// <returns></returns>
        BGMPlayer bgmPlayer;

        /// <summary>
        /// 初期にAudioObjectをプールする数
        /// </summary>
        private int initialAudioObjectNum = 10;

        const string POOLED_OBJECT_NAME = "AudioObject";
        uint pooledObjectKey;

        /// <summary>
        /// 初期化処理
        /// </summary>
        protected override void Init()
        {
            bgmPlayer = Instantiate<BGMPlayer>(bgmPlayerPrefab, transform);
            pooledObjectKey = (uint)POOLED_OBJECT_NAME.GetHashCode();
            objectPool.Pool(pooledObjectKey, audioObjectPrefab, initialAudioObjectNum, parent:transform);
        }

        /// <summary>
        /// SEを再生する
        /// </summary>
        /// <param name="name">再生するClip名</param>
        /// <param name="loop">ループSEかどうか</param>
        /// <returns>再生中のAudioObject</returns>
        public AudioObject PlaySE(string name, bool loop = false)
        {
            AudioClip clip = getOrLoadClip(name);
            if(clip == null) return null;

            AudioObject audioObject = objectPool.Get(pooledObjectKey);
            audioObject.gameObject.SetActive(true);
            if(loop) audioObject.Play(clip);
            else audioObject.PlayOneShot(clip);

            playingAudioObjects.Add(audioObject);
            return audioObject;
        }

        /// <summary>
        /// BGMを再生する
        /// </summary>
        /// <param name="name">再生するClip名</param>
        /// <param name="fadeDuration">フェード時間</param>
        public void PlayBGM(string name, float fadeDuration = 0.0f)
        {
            AudioClip clip = getOrLoadClip(name);
            if(clip == null) return;
            bgmPlayer.Play(clip, fadeDuration);
        }

        /// <summary>
        /// ロード済みクリップを取得もしくはロードする
        /// </summary>
        /// <param name="name">取得したいClip名</param>
        /// <returns>AudioClip</returns>
        private AudioClip getOrLoadClip(string name)
        {
            uint hash = (uint)name.GetHashCode();
            AudioClip clip;
            // AudioClipがロードされてない場合はロード処理を行う
            if(!clips.TryGetValue(hash, out clip)){
                string path = Path.Join("Audio", name);

                D.Log.I(path);
                // TODO: ResourceManager
                clip = Resources.Load<AudioClip>(path);
                clips.Add(hash, clip);
            }
            return clip;
        }


        void Update()
        {
            int i = playingAudioObjects.Count - 1;
            while(i >= 0){
                var obj = playingAudioObjects[i];
                if(!obj.isPlaying && !obj.loop) {
                    objectPool.Return(pooledObjectKey, obj);          
                    obj.gameObject.SetActive(false);          
                    playingAudioObjects.RemoveAt(i);
                }
                --i;
            }
        }
    }
}