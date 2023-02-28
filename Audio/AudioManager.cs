using UnityEngine;
using System.IO;
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
        /// BGM再生の管理
        /// </summary>
        /// <returns></returns>
        BGMPlayer bgmPlayer;

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
            bgmPlayer = Instantiate<BGMPlayer>(bgmPlayerPrefab, transform);
            objectPool.Pool(POOLED_OBJECT_NAME, audioObjectPrefab, initialAudioObjectNum, parent:transform);
        }

        /// <summary>
        /// SEを再生する 
        /// </summary>
        public void PlaySE(string name)
        {
            AudioClip clip = getOrLoadClip(name);
            if(clip == null) return;

            AudioObject audioObject = objectPool.Get(POOLED_OBJECT_NAME);
            audioObject.gameObject.SetActive(true);
            audioObject.Play(clip);
        }

        /// <summary>
        /// BGMを再生する
        /// </summary>
        /// <param name="name">再生するClip</param>
        /// <param name="fadeDuration">フェード時間</param>
        public void PlayBGM(string name, float fadeDuration = 0.0f)
        {
            AudioClip clip = getOrLoadClip(name);
            if(clip == null) return;
            bgmPlayer.play(clip, fadeDuration);
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

                Debug.Log.I(path);
                // TODO: ResourceManager
                clip = Resources.Load<AudioClip>(path);
                clips.Add(hash, clip);
            }
            return clip;
        }
    }
}