using UnityEngine;
using System.Collections.Generic;
using Ashe.Collection;

namespace Ashe
{
    /// <summary>
    /// オブジェクトを事前にプールし必要なときに貸し出す機能を提供します
    /// 大量にランタイムに生成が必要なエフェクトなどで使用しインスタンスのコストを排除する
    /// 複数の型に対してプールが必要な場合は型ごとに定義が必要 
    /// </summary>
    /// <typeparam name="T">プールしたいオブジェクトの型</typeparam>
    public class PooledObjectManager<T> : Pattern.Singleton<PooledObjectManager<T>> where T : MonoBehaviour
    {
        /// <summary>
        /// プレハブを指定された回数Instantiateしてプールする
        /// 取り出すときは指定した名前をもとに取り出す
        /// </summary>
        /// <param name="name">Keyを作成する元となる名前</param>
        /// <param name="prefab">プールしたいオブジェクトのPrefab</param>
        /// <param name="num">プールする数</param>
        /// <param name="active">初期値をアクティブにしておくか</param>
        /// <param name="parent">親になるTransform</param>
        public void Pool(string name, T prefab, int num, bool active = false, Transform parent = null)
        {
            Pool((uint)name.GetHashCode(), prefab, num, active, parent);
        }

        /// <summary>
        /// プレハブを指定された回数Intantiateしてプールする
        /// 取り出すときは指定したKeyをもとに取り出す
        /// </summary>
        /// <param name="key">プールしたオブジェクトを取り出すときに必要なKey</param>
        /// <param name="prefab">プールしたいオブジェクトのPrefab</param>
        /// <param name="num">プールする数</param>
        /// <param name="active">初期値をアクティブにしておくか</param>
        /// <param name="parent">親になるTransform</param>
        public void Pool(uint key, T prefab, int num, bool active = false, Transform parent = null)
        {
            Queue<T> q;
            if (!dictionary.TryGetValue(key, out q))
            {
                q = new Queue<T>();
                dictionary.Add(key, q);
            }
            for(int i = 0; i < num; ++i)
            {
                T t = Object.Instantiate<T>(prefab, parent);
                t.gameObject.SetActive(active);
                q.Enqueue(t);
            }
        }

        /// <summary>
        /// プールしてあるオブジェクトを取得する　
        /// </summary>
        /// <param name="name">登録時に指定した名前</param>
        /// <returns>プールから取得したオブジェクト</returns>
        public T Get(string name)
        {
            return Get((uint)name.GetHashCode());
        }

        /// <summary>
        /// プールしてあるオブジェクトを取得する　
        /// </summary>
        /// <param name="key">登録時に指定したKey</param>
        /// <returns>プールから取得したオブジェクト</returns>
        public T Get(uint key)
        {
            Queue<T> q;
            if (!dictionary.TryGetValue(key, out q))
            {
                q = new Queue<T>();
                dictionary.Add(key, q);
            }

            if(q.Count == 0)
            {
                return null;
            }
            return q.Dequeue();
        }


        /// <summary>
        /// オブジェクトをプールするDictionary
        /// </summary>
        UIntKeyDictionary<Queue<T>> dictionary = new UIntKeyDictionary<Queue<T>>();                
    }
}
