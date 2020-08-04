using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using Ashe.Collection;

namespace Ashe
{
    namespace Animation
    {
        /// <summary>
        /// CharacterControllerなどでコンポジットして使用
        /// </summary>
        public class PlayableAnimation
        {
            ~PlayableAnimation()
            {
                playableGraph.Destroy();
            }

            /// <summary>
            /// Animationの設定データ 
            /// </summary>
            public class AnimationData
            {
                AnimationData(string _name, AnimationClip _clip)
                {
                    name = _name;
                    clip = _clip;
                }

                public string name;
                public AnimationClip clip;
                [HideInInspector]
                public AnimationClipPlayable clipPlayable;
            }

            /// <summary>
            /// Animationの出力先 
            /// </summary>
            public class AnimationOutput
            {
                public string name;
                public AnimationPlayableOutput output;
            }

            /// <summary>
            /// PlayableGraphに登録されたAnimationClip 
            /// </summary>
            UIntKeyDictionary<AnimationData> animationDatas = new UIntKeyDictionary<AnimationData>();

            /// <summary>
            /// PlayableGraphのAnimation出力先, 複数のターゲットを指定出来る。 
            /// </summary>
            UIntKeyDictionary<AnimationOutput> animationOutputs = new UIntKeyDictionary<AnimationOutput>();

            /// <summary>
            /// アニメーションのクロスフェード用 
            /// </summary>
            AnimationMixerPlayable mixer;

            /// <summary>
            /// Animation再生制御をPlayableで行う
            /// </summary>
            PlayableGraph playableGraph;

            /// <summary>
            /// 今再生中のアニメーション 
            /// </summary>
            AnimationData currentAnimationData;

            /// <summary>
            /// 今再生中のアニメーションが使用しているインデックス番号
            /// </summary>
            int currentMixerSlot;

            /// <summary>
            /// クロスフェードの経過秒数
            /// </summary>
            float fadeTime;

            /// <summary>
            /// クロスフェードに使う秒数　
            /// </summary>
            float duration;

            /// <summary>
            /// 初期化Playableに必要な物の作成 
            /// </summary>
            public void Initialize(int maxBlendNum = 2)
            {
                playableGraph = PlayableGraph.Create();
                mixer = AnimationMixerPlayable.Create(playableGraph, maxBlendNum, true);
            }

            /// <summary>
            /// Playableで再生するアニメーションリソースの追加
            /// </summary>
            /// <param name="animations">AnimationClipとアニメーション名のペア</param>
            public void AddAnimations(AnimationData[] animations)
            {
                if (animations == null) return;
                for (int i = 0; i < animations.Length; ++i)
                {
                    AddAnimation(animations[i]);
                }
            }

            /// <summary>
            /// Playableで再生するアニメーションリソースの追加
            /// </summary>
            /// <param name="animation">AnimationClipとアニメーション名のペア</param>
            public void AddAnimation(AnimationData animation)
            {
                if (animation == null || animation.clip == null) return;
                animation.clipPlayable = AnimationClipPlayable.Create(playableGraph, animation.clip);
                animationDatas.Add((uint)animation.name.GetHashCode(), animation);
            }

            /// <summary>
            /// Animationを再生する出力先の指定 
            /// 複数を指定すると同じAnimationを再生できる 
            /// </summary>
            /// <param name="animator"></param>
            /// <param name="name"></param>
            public void AddTarget(Animator animator, string name)
            {
                AnimationOutput animationOutput = new AnimationOutput();
                animationOutput.name = name;
                animationOutput.output = AnimationPlayableOutput.Create(playableGraph, name, animator);

                animationOutput.output.SetSourcePlayable(mixer);

                animationOutputs.Add((uint)name.GetHashCode(), animationOutput);
            }

            /// <summary>
            /// Animationの再生 
            /// </summary>
            /// <param name="name">Animation名</param>
            /// <param name="duration">クロスフェードの時間</param>
            public void Play(string name, float duration = 0.0f)
            {
                Play((uint)name.GetHashCode(), duration);
            }

            /// <summary>
            /// Animationの再生
            /// </summary>
            /// <param name="hash">AnimationのHash</param>
            /// <param name="duration">クロスフェードの時間</param>
            public void Play(uint hash, float duration = 0.0f)
            {
                fadeTime = 0.0f;
                this.duration = duration;
                if (!animationDatas.TryGetValue(hash, out currentAnimationData)) return;
                if (duration <= Ashe.Const.Float.EPSILON)
                {
                    mixer.ConnectInput(currentMixerSlot, currentAnimationData.clipPlayable, 0, 1.0f);
                }
                else
                {
                    mixer.ConnectInput(currentMixerSlot, currentAnimationData.clipPlayable, 0);

                    int prevSlot = currentMixerSlot;
                    currentMixerSlot = currentMixerSlot == 1 ? 0 : 1;

                    float t = fadeTime / duration;
                    mixer.SetInputWeight(currentMixerSlot, t);
                    mixer.SetInputWeight(prevSlot, 1.0f - t);
                }
                playableGraph.Play();
            }

            /// <summary>
            /// Animationの更新
            /// クロスフェードが指定されているならクロスフェードを行う 
            /// </summary>
            /// <param name="deltaTime"></param>
            public void Update(float deltaTime)
            {
                fadeTime += deltaTime;
                if (duration >= fadeTime && duration >= Ashe.Const.Float.EPSILON)
                {
                    float t = fadeTime / duration;
                    int prevSlot = currentMixerSlot == 1 ? 0 : 1;
                    mixer.SetInputWeight(currentMixerSlot, t);
                    mixer.SetInputWeight(prevSlot, 1.0f - t);
                }
            }
        }
    }
}