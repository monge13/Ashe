using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using Ashe.Audio;

namespace Ashe
{
    namespace Clip
    {
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
            
            AnimationClipPlayable _playable;
            public AnimationClipPlayable playable { get { return _playable; } }

            // サウンド再生関係のイベント
            [SerializeField]
            List<AudioEvent> _audioEventList;
            public List<AudioEvent> audioEventList {  get { return _audioEventList;} }

            // エフェクト再生関係のイベント
            [SerializeField]
            List<EffectEvent> _effectEventList;
            public List<EffectEvent> effectEventList {  get { return _effectEventList;} }
            

            // 初期化処理
            public void Initialize(PlayableGraph graph, Transform root)
            {        
                if(_playable.IsValid()) return;
                _playable = AnimationClipPlayable.Create(graph, _animationClip);

                foreach(var e in audioEventList)
                {
                    e.Initialize(root);
                }

                foreach(var e in effectEventList)
                {
                    e.Initialize(root);
                }

            }

            // PlayableをDestroyする
            ~ActionClip()
            {
                if(_playable.IsValid()) {
                    _playable.Destroy();
                }
            }

            // 更新処理
            // 経過時間によってイベントを発動する
            public void Execute(float currentTime)
            {
                foreach(var e in _audioEventList){
                    e.Update(currentTime);
                }
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
                var obj = AudioManager.I.PlaySE(_audioClip);
                if(hasParent) obj.transform.parent = _parent;
            }
        }

        [Serializable]
        public class EffectEvent : KeyEvent
        {
            // 再生対象のエフェクト
            [SerializeField]
            ParticleSystem _ps;
            
            [HideInInspector]
            [SerializeField]
            uint key;

            protected override void OnInitialize()
            {
                
            }

            // Effectの再生処理を行う
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
            // 再生する再の親オブジェクト
            [SerializeField]
            string _parentTransformName;
            // 再生位置のオフセット
            [SerializeField]
            protected Vector3 _offset;
            // 再生方向
            [SerializeField]
            protected Vector3 _rotation;

            // 再生開始される時間
            [SerializeField]
            float _time;
            // 再生されたかどうか
            bool isPlayed;
            // 再生する再親のTransform
            protected Transform _parent;
            protected bool hasParent {
                get {
                    return !string.IsNullOrEmpty(_parentTransformName) && _parent != null;
                }
            }

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

            public void Initialize(Transform root)
            {
                if(string.IsNullOrEmpty(_parentTransformName)) return;
                _parent = root.Find(_parentTransformName);
                OnInitialize();
            }

            // 継承してイベントごとの初期化処理を行う
            protected virtual void OnInitialize()
            {
            }

            // 継承してイベントごとの再生する処理を行う
            protected virtual void OnPlay()
            {

            }
        }
    }
}