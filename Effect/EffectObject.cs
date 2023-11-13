using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Ashe
{
    namespace Effect
    {
        [RequireComponent(typeof(ParticleSystem))]
        public class EffectObject : MonoBehaviour
        {            
            public Transform cachedTransform;

            // ParticleSystem
            [SerializeField]
            ParticleSystem _ps;

            // エフェクト再生のKeyID
            [HideInInspector]
            [SerializeField]
            uint _effectId;
            public uint effectId { get { return _effectId; } }

#if UNITY_EDITOR
            // Editorでのセットアップ用にGUIDを更新する
            public void UpdateGUID()
            {
                if(_ps != null) {
                    string s = AssetDatabase.GetAssetPath(_ps);
                    _effectId = (uint)AssetDatabase.AssetPathToGUID(s).GetHashCode();          
                }
            }      
#endif

            /// <summary>
            /// エフェクトの再生が止まったら自動で回収する
            /// </summary>
            [SerializeField]
            bool _autoReturn = true;
            public bool autoReturn { get { return _autoReturn; } }


            // Effectの再生
            public void Play()
            {
                _ps.Play();                
            }

            // Effectの
            public void Stop()
            {
                _ps.Stop();

            }

            void Awake() 
            {
                cachedTransform = transform;
            }

            // Particle停止時の処理
            void OnParticleSystemStopped () 
            {
                // TODO: StopActionの設定変更を矯正化
                if(_autoReturn){
                    EffectManager.I.Return(effectId, this);                
                }
            }
        }
    }
}