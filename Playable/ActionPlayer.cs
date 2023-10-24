using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Audio;

public class ActionPlayer : MonoBehaviour
{
    [SerializeField]
    Animator _animationTarget;

    [SerializeField]
    ActionClip _clip;

    float animationLength;

    // 再生中かどうか
    public bool isPlaying;

    // 現在の再生時間
    float _time;

    void Start()
    {
        animationLength = _clip.animationClip.length;  
        _clip.Initialize(_animationTarget);
        _clip.Play();
        isPlaying = true;
    }


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
}
