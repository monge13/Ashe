using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using Ashe.Audio;

/// <summary>
/// Animaton, Audio, Effectなどを格納して一つのアクションとして再生するためのデータ
/// </summary>
[CreateAssetMenu(menuName = "Ashe/ActionClip")]
public class ActionClip : ScriptableObject
{
    // アクションで再生するAnimation
    [SerializeField]
    AnimationClip _animationClip;
    public AnimationClip animationClip { get { return _animationClip; } }
    
    // サウンド再生関係のイベント
    [SerializeField]
    List<AudioEvent> _audioEventList;
    public List<AudioEvent> audioEventList {  get { return _audioEventList;} }

    // Animation再生用のグラフ
    PlayableGraph _graph;

    // 初期化処理グラフを作ったりする
    public void Initialize(Animator target)
    {
        InitializeAnimation(target);
    }

    // Animationの初期化
    private void InitializeAnimation(Animator target)
    {
        if(_graph.IsValid()) {
            _graph.Destroy();
        }
        _graph = PlayableGraph.Create();
        // TODO: 将来的にはMANUALにしてDeltaTimeを渡すようにする。ポーズ対応
        _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);        
        // Graphにターゲットを設定
        var playableOutput = AnimationPlayableOutput.Create(_graph, "Animation", target);        
        // clip playableを作成
        var clipPlayable = AnimationClipPlayable.Create(_graph, _animationClip);
        // Targetにクリップを接続
        playableOutput.SetSourcePlayable(clipPlayable);
    }

    public void Play()
    {
        if(!_graph.IsValid()) return;
        _graph.Play();
    }

    // 更新処理
    // 経過時間によってイベントを発動する
    public void Execute(float currentTime)
    {
        foreach(var e in _audioEventList){
            e.Update(currentTime);
        }
    }

    // 終了処理リソースの開放を行う
    public void Finalize()
    {
        if(_graph.IsValid()) _graph.Destroy();
    }

    /// <summary>
    /// 指定した時間移行のイベントを初期化する
    /// </summary>
    /// <param name="fromTime">この時間移行のイベントを初期化する</param>
    public void Reset(float fromTime = -1.0f)
    {
        foreach(var e in _audioEventList){
            e.Update(fromTime);
        }
    }

    /// <summary>
    /// AnimationがLoopした際にイベントの再生状況をリセットする
    /// </summary>
    public void Loop()
    {
        foreach(var e in _audioEventList){
            e.Loop();
        }        
    }

    /// <summary>
    /// SE再生イベント
    /// </summary>
    [Serializable]
    public class AudioEvent : KeyEvent
    {
        // 再生対象のSE
        [SerializeField]
        AudioClip _audioClip;

        // SEの再生処理を行う
        protected override void OnPlay()
        {
            AudioManager.I.PlaySE(_audioClip);
        }
    }

    [Serializable]
    public class EffectEvent : KeyEvent
    {
        // 再生対象のエフェクト
        [SerializeField]
        ParticleSystem _ps;

        // SEの再生処理を行う
        protected override void OnPlay()
        {
            
        }
    }

    /// <summary>
    /// 再生タイミングのKeyとなる時間を持ったEvent
    /// </summary>
    [Serializable]
    public class KeyEvent
    {
        // S再生開始される時間
        [SerializeField]
        float _time;
        // 再生されたかどうか
        bool isPlayed;

        // AnimationがLoopしたのでSEを再生していないことにする
        public void Loop()
        {
           isPlayed = false;
        }

        // 時間を与えて更新する
        public void Update(float currentTime)
        {
            if(isPlayed) return;
            if(currentTime < _time) return;
            OnPlay();
            isPlayed = true;
        }

        // 継承して再生する処理を行う
        protected virtual void OnPlay()
        {

        }
    }

}