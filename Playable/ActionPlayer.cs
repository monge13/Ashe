using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Audio;

public class ActionPlayer : MonoBehaviour
{
    [SerializeField]
    Animator _animationTarget;

    // 再生中のClip
    ActionClip _clip;

    // Animationのフレーム数
    float animationLength;

    // 再生中かどうか
    public bool isPlaying;

    // 現在の再生時間
    float _time;

    void Update()
    {
        if(!isPlaying) return;
        _time += Time.deltaTime;
        if(animationLength <= _time){
            _time -= animationLength;
            _clip.Loop();
        }
        _clip.Execute(_time);
    }

    // アクションを再生する
    public void Play(ActionClip clip)
    {
        _time = 0.0f;
        _clip = clip;
        animationLength = _clip.animationClip.length;  
        _clip.Initialize(_animationTarget);
        _clip.Play();
        isPlaying = true;
    }

    // アクション再生を停止する
    public void Stop()
    {

    }

    void OnDestroy()
    {
        if(_clip != null) {
            _clip.Finalize();
            _clip = null;
        }
    }
}
