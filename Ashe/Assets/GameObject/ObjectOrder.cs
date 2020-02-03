using System;
using UnityEngine;

namespace Ashe
{
    /// <summary>
    /// ObjectBaseの更新順番を指定するための値を保持する 
    /// UnityでいうLayerに近い 
    /// </summary>
    [CreateAssetMenu]
    public class ObjectOrder : ScriptableObject
    {
        [SerializeField]
        private string[] names = new string[GameObjectManager.MAX_OBJECTBASE_ORDER_NUM];

        /// <summary>
        /// ctor
        /// </summary>
        ObjectOrder()
        {
            names[ORDER_SYSTEM] = "System";
            names[ORDER_DEFAULT] = "Default";
        }

        /// <summary>
        /// 文字列からOrderへの変換を行う 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int NameToOrder(string name)
        {
            return Array.IndexOf<string>(names, name);
        }

        /// <summary>
        /// Orderから文字列へ変換を行う 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public string OrderToName(int order)
        {
            return names[order];
        }

        public const int ORDER_INVALID = -1;        // 不正なオーダー値、この値が指定された場合はオブジェクトに指定されているオーダーを使う
        public const int ORDER_SYSTEM = 1;          // 入力などのシステム用のオーダー値
        public const int ORDER_DEFAULT = 10;        // 初期で指定されるオーダー値 
    }
}