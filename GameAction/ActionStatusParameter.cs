using System;
using System.Collections.Generic;
using UnityEngine;
using Ashe.Collection;

namespace Ashe
{
    namespace GameAction
    {
        /// <summary>
        /// このキャラクターが持つパラメーターの管理を行う
        /// </summary>
        [Serializable]
        public class ActionStatusParameter
        {
            // Pameter名
            [SerializeField]
            string _name;
            public string name { get { return _name; } }

            // ベース値（変更しない）
            [SerializeField]
            float _baseValue;
            public float baseValue { get { return _baseValue; } }

            // 現在値
            float _currentValue;
            public float currentValue { get { return _currentValue; } set { _currentValue = value; } }
        
            // 現在値をベース値で初期化する
            public void Initialize()
            {
                _currentValue = _baseValue;
            }
        }

        /// <summary>
        /// パラメーターへのアクセスなどを管理する
        /// </summary>
        [Serializable]
        public class ActionStatusParameters 
        {
            // パラメーターの各数値
            [SerializeField]
            List<ActionStatusParameter> _parametes = new List<ActionStatusParameter>();
            // パラメーターの各数値（アクセス用)        
            UIntKeyDictionary<ActionStatusParameter> _parameterDict;

            // アクセス用のDictionary作成を行う
            public void Initialize()
            {
                // パラメータアクセスのためにDictionaryを作成する
                _parameterDict = new UIntKeyDictionary<ActionStatusParameter>(_parametes.Count);
                foreach(var s in _parametes){
                    _parameterDict.Add((uint)s.name.GetHashCode(), s);
                    s.Initialize();
                }
            }

            // パラメーターの取得
            public ActionStatusParameter GetParam(uint key)
            {
                ActionStatusParameter ret;
                if(_parameterDict.TryGetValue(key, out ret)) return ret;
                return null;
            }

            // パラメーターの取得
            public ActionStatusParameter GetParam(string name)
            {
                return GetParam((uint)name.GetHashCode());
            }
        }
    }
}
