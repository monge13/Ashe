
using UnityEngine;

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
    public void play(AudioClip clip, float fadeDuration)
    {
        int newPlayGroup = ((int)playingGroup+1) % 2;
        Ashe.Debug.Log.I(newPlayGroup.ToString());
        source[newPlayGroup].clip = clip;
        if(fadeDuration > 0f) {
            // TODO: フェードを作る
        }
        source[newPlayGroup].Play();
        playingGroup = (GROUP)newPlayGroup;
    }
}
