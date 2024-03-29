using UnityEngine;

namespace Ashe
{
    namespace Audio
    {     
        /// <summary>
        /// Audioを再生するObject
        /// </summary>
        [RequireComponent(typeof(AudioSource))]
        public class AudioObject : MonoBehaviour
        {
            [SerializeField]
            AudioSource source;

            public Transform cachedTransform;

            bool _loop = false;
            public bool loop 
            {
                get { return _loop; }
            }

            /// <summary>
            /// 再生中かどうかを取得する
            /// </summary>
            public bool isPlaying 
            {
                get {
                    return source.isPlaying;
                }
            }

            void Start()
            {
                cachedTransform = transform;
            }

            /// <summary>
            /// AudioObjectを作成する
            /// </summary>
            /// <param name="parent">再生位置</param>
            /// <returns></returns>
            static public AudioObject Create(Transform parent)
            {
                GameObject obj = new GameObject("AudioObject");                 
                obj.AddComponent<AudioSource>();
                obj.transform.parent = parent;
                return obj.AddComponent<AudioObject>();
            }

            /// <summary>
            /// 一度だけの再生をする
            /// </summary>
            /// <param name="clip">再生対象のAudioClip</param>
            public void PlayOneShot(AudioClip clip)
            {
                source.PlayOneShot(clip);
            }

            /// <summary>
            /// SEを再生する。自分で止める必要がある
            /// </summary>
            /// <param name="clip">再生対象のAudioClip</param>
            public void Play(AudioClip clip)
            {
                _loop = true;
                source.clip = clip;
                source.Play();
            }

            /// <summary>
            /// SEの再生を停止する
            /// </summary>
            public void Stop()
            {
                source.Stop();
            }

        }
    }
}