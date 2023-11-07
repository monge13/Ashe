using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ashe
{
    namespace Effect
    {
        /// <summary>
        /// Effectのキャッシュルートとしての仕組み
        /// 同時再生や同じエフェクトの管理などを行う
        /// </summary>
        public class EffectManager : Pattern.SingletonMonoBehaviour<EffectManager>
        {
            /// <summary>
            /// ParticleSystemをPoolしておくRoot
            /// nullの場合はComponentがついているオブジェクト下につく
            /// </summary>
            [SerializeField]
            Transform cacheRoot;

            PooledObjectManager<ParticleSystem> objectPool = new PooledObjectManager<ParticleSystem>();
       
            /// <summary>
            /// 初期化処理
            /// </summary>
            protected override void Init()
            {
                if(cacheRoot == null) cacheRoot = transform;
            }

            /// <summary>
            /// エフェクトをInstantiateしてPoolする
            /// </summary>
            /// <param name="name">登録するエフェクト名</param>
            /// <param name="effectObj">登録するエフェクト</param>
            /// <param name="num">プールする数</param>
            public void poolObject(string name, ParticleSystem effectObj, int num = 10)
            {
                poolObject((uint)name.GetHashCode(), effectObj, num);
            }

            /// <summary>
            /// エフェクトをInstantiateしてPoolする
            /// </summary>
            /// <param name="hash">登録するエフェクトハッシュ</param>
            /// <param name="effectObj">登録するエフェクト</param>
            /// <param name="num">プールする数</param>
            public void poolObject(uint hash, ParticleSystem effectObj, int num = 10)
            {
                objectPool.Pool(hash, effectObj, num, parent:cacheRoot);
            }

            /// <summary>
            /// Poolからエフェクトを取得する
            /// </summary>
            /// <param name="key">取得したいエフェクトのKey</param>
            public ParticleSystem Get(uint key)
            {
                return objectPool.Get(key);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            /// <param name="ps"></param>
            public void Return(uint key, ParticleSystem ps)
            {
                objectPool.Return(key, ps);
                ps.gameObject.SetActive(false);
            }
        }
    }
}