using UnityEngine;
using DG.Tweening;

namespace Ashe
{
    namespace Audio
    { 
        public class BGMPlayer : MonoBehaviour
        {   
            /// <summary>
            /// AudioMixerのグループ
            /// </summary>
            enum GROUP : int
            {
                Music1 = 0,
                Music2 = 1,
                Ambient = 2,
                NOT_PLAYING = 3,
            }

            /// <summary>
            /// AudioSource
            /// </summary>
            [SerializeField] 
            AudioSource[] source;
            
            /// <summary>
            /// 再生中のGROUP
            /// </summary>
            GROUP playingGroup = GROUP.NOT_PLAYING;

            /// <summary>
            /// BGMの再生処理
            /// </summary>
            /// <param name="clip"></param>
            /// <param name="fadeDuration"></param>
            public void Play(AudioClip clip, float fadeDuration)
            {
                int currentPlayingGroup = (int)playingGroup;
                int newPlayGroup = ((int)playingGroup+1) % 2;
                D.Log.I(newPlayGroup.ToString());
                source[newPlayGroup].clip = clip;
                if(fadeDuration > 0f) {
                    var sequence = DOTween.Sequence();
                    source[newPlayGroup].volume = 0f;
                    sequence.Append(source[newPlayGroup].DOFade(1f, fadeDuration));
                    if(playingGroup != GROUP.NOT_PLAYING) {                
                        sequence.Join(source[currentPlayingGroup].DOFade(0f, fadeDuration));
                    }
                    sequence.Play().OnComplete(()=> source[currentPlayingGroup].Stop());
                }
                source[newPlayGroup].Play();
                playingGroup = (GROUP)newPlayGroup;
            }
        }
    }
}