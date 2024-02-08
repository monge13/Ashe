using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using Ashe.Audio;
using Ashe.Effect;

namespace Ashe
{
    namespace GameAction
    {
        /// <summary>
        /// Animaton, Audio, Effectなどを格納して一つのアクションとして再生するためのデータ
        /// </summary>
        [CreateAssetMenu(menuName = "Ashe/Action/Clip")]
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
            
            // オブジェクトを発生させるイベント
            [SerializeField]
            List<SpawnEvent> _spawnEventList;
            public List<SpawnEvent> spawnEventList { get { return _spawnEventList; } }


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

                foreach(var e in spawnEventList)
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
                foreach(var e in _effectEventList){
                    e.Update(currentTime);
                }
                foreach(var e in _spawnEventList){
                    e.Update(currentTime);
                }
            }

            /// <summary>
            /// 指定した時間移行のイベントを初期化する
            /// </summary>
            /// <param name="fromTime">この時間移行のイベントを初期化する</param>
            public void ResetEvent(float fromTime = -1.0f)
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
                foreach(var e in _effectEventList){
                    e.Loop();
                }
                foreach(var e in _spawnEventList){
                    e.Loop();
                }                
            }
        
            /// <summary>
            /// SE再生イベント
            /// </summary>
            [Serializable]
            public class AudioEvent : PositionalEvent
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
            public class EffectEvent : PositionalEvent
            {
                // 再生対象のエフェクト
                [SerializeField]
                EffectObject _effect;
                
                // EffectManagerに登録する
                protected override void OnInitialize(Transform root)
                {
                    base.OnInitialize(root);
                    EffectManager.I.poolObject(_effect.effectId, _effect, 1);
                }

                // Effectの再生処理を行う
                protected override void OnPlay()
                {
                    var ef = EffectManager.I.Get(_effect.effectId);                    
                    if(ef != null) {
                        if(hasParent) ef.cachedTransform.parent = _parent;
                        ef.gameObject.SetActive(true);
                        ef.cachedTransform.SetLocalPositionAndRotation(_offset, Quaternion.Euler(_rotation));
                        ef.Play();
                    }
                }
            }

            /// <summary>
            /// オブジェクトをインスタンスする
            /// </summary>
            [Serializable]
            public class SpawnEvent : PositionalEvent
            {
                // 生成位置のTransform
                [SerializeField]
                string _startTransformName;
                Transform _startTransform;
                public bool hasStartTransform {
                    get {
                        return !string.IsNullOrEmpty(_startTransformName);
                    }
                }
                // 生成するオブジェクト名（キャッシュされている場合はKeyとなる）
                [SerializeField]
                string _spawnObjectName;
                // キャッシュされていない場合はこちらをInstanciateする
                [SerializeField]
                SpawnableObject _prefab;

                // キャッシュから引き出すときのKey
                uint _key;

                // キャッシュする
                protected override void OnInitialize(Transform root)
                {
                    base.OnInitialize(root);
                    if(hasStartTransform){
                        _startTransform = root.Find(_startTransformName);
                    }
                    _key = (uint)_spawnObjectName.GetHashCode();
                    int cacheNum = _prefab.cacheNum <= 0 ? 1 : _prefab.cacheNum; 
                    PooledObjectManager<SpawnableObject>.I.Pool(_key, _prefab, cacheNum);
                }

                // Objの再生処理を行う
                protected override void OnPlay()
                {
                    var obj = PooledObjectManager<SpawnableObject>.I.Get(_key);
                    if(obj != null) {
                        Transform t = obj.transform;
                        if(hasParent) t.parent = _parent;
                        if(hasStartTransform) t.SetPositionAndRotation(_startTransform.position + _offset, _startTransform.rotation * Quaternion.Euler(_rotation));
                        else t.SetLocalPositionAndRotation(_offset, Quaternion.Euler(_rotation));
                        obj.gameObject.SetActive(true);
                    }
                }
            }

            /// <summary>
            /// パラメーターに変化を与えるイベントを発生させる
            /// </summary>
            [Serializable]
            public class ParameterEvent
            {
                
            }

            // 座標情報を持つイベント
            public class PositionalEvent : KeyEvent
            {
                // Transformのパスを取得するのは"Edit/CopyTransformName"を使ってください
                // 再生する再の親オブジェクト
                [SerializeField]
                string _parentTransformName;
                // 再生位置のオフセット
                [SerializeField]
                protected Vector3 _offset;
                // 再生方向
                [SerializeField]
                protected Vector3 _rotation;

                // 再生する再親のTransform
                protected Transform _parent;
                protected bool hasParent {
                    get {
                        return !string.IsNullOrEmpty(_parentTransformName) && _parent != null;
                    }
                }
                // 座標情報の初期化
                protected override void OnInitialize(Transform root)
                {
                    if(hasParent){                    
                        _parent = root.Find(_parentTransformName);
                    }
                }
            }

            /// <summary>
            /// 再生タイミングのKeyとなる時間を持ったEvent
            /// </summary>
            [Serializable]
            public class KeyEvent
            {
                // 再生開始される時間
                [SerializeField]
                float _time;
                // 再生されたかどうか
                bool isPlayed;

                // AnimationがLoopしたので再生していないことにする
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
                    OnInitialize(root);
                }

                // 継承してイベントごとの初期化処理を行う
                protected virtual void OnInitialize(Transform root)
                {
                }

                // 継承してイベントごとの再生する処理を行う
                protected virtual void OnPlay()
                {

                }
            }
        }
    }
}