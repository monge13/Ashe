using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Audio;
using UnityEngine.Animations;

namespace Ashe
{
    namespace Clip 
    {
        /// <summary>
        /// ActionClipを再生する
        /// </summary>
        public class ActionClipPlayer : MonoBehaviour
        {
            // Animationを再生するターゲット
            [SerializeField]
            Animator _animationTarget;

            // 再生対象のTransform
            Transform _targetTransform;

            // 再生中のClip
            ActionClip _clip;

            // Animationのフレーム数
            float animationLength;

            // 再生中かどうか
            public bool isPlaying;

            // 現在の再生時間
            float _time;

            // Animation再生用のグラフ
            PlayableGraph _graph;
            AnimationMixerPlayable _animationMixer;
            // Mixerで使用しているインデックス
            int currentUsedMixerIndex = 0;
            // Blendが必要な時間
            float _blendTime;
            // Blendが必要かどうか
            bool needBlend { get { return _blendTime >= Const.Float.EPSILON; } } 

            void Awake()
            {
                _targetTransform = _animationTarget.transform;
                // Animation関連の初期化
                _graph = PlayableGraph.Create();
                // TODO: 将来的にはMANUALにしてDeltaTimeを渡すようにする。ポーズ対応
                _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
                _animationMixer = AnimationMixerPlayable.Create (_graph, 2, true);
                var playableOutput = AnimationPlayableOutput.Create(_graph, "Animation", _animationTarget); 
                playableOutput.SetSourcePlayable(_animationMixer); 
            }

            void Update()
            {
                if(!isPlaying) return;            
                _time += Time.deltaTime;
                // Blend処理
                if(needBlend) {
                    float t  = _time / _blendTime;
                    if( t >= 1.0f) {
                        t = 1.0f;
                        _blendTime = 0.0f;
                    }
                    int preIndex = (currentUsedMixerIndex + 1) % 2;
                    _animationMixer.SetInputWeight(preIndex, 1.0f - t);
                    _animationMixer.SetInputWeight(currentUsedMixerIndex, t);                
                }
                // Loopモーション用のイベント管理処理
                if(animationLength <= _time){
                    _time -= animationLength;
                    _clip.Loop();
                }
                // SE,Effectなどのイベント処理
                _clip.Execute(_time);
            }

            // アクションを再生する
            public void Play(ActionClip clip, float blendTime=0.0f)
            {
                _time = 0.0f;
                _clip = clip;
                _clip.Initialize(_graph, _targetTransform);

                animationLength = _clip.animationClip.length;

                // 現在使用中のインデックスではない方を開けて再生する
                int nextMixerIndex = (currentUsedMixerIndex+1) % 2;
                _graph.Disconnect(_animationMixer, nextMixerIndex);
                _animationMixer.ConnectInput(nextMixerIndex, _clip.playable, 0);

                // Blend処理
                _blendTime = blendTime;
                if(needBlend) {
                    _animationMixer.SetInputWeight(currentUsedMixerIndex, 0);
                    _animationMixer.SetInputWeight(nextMixerIndex, 1);
                }
                else{
                    _animationMixer.SetInputWeight(currentUsedMixerIndex, 1);
                    _animationMixer.SetInputWeight(nextMixerIndex, 0);
                }

                currentUsedMixerIndex = nextMixerIndex;
                _graph.Play();
                isPlaying = true;
            }

            // アクション再生を停止する
            public void Stop()
            {

            }

            void OnDestroy()
            {
                if(_graph.IsValid()) {
                    _graph.Destroy();
                }
            }
        }
    }
}