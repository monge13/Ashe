using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ashe {
    namespace GameAction {
        /// <summary>
        /// Actionのステータスを管理するタグ
        /// 例えばPoison,Stunなど状態異常やJumpなどの行動まで
        /// </summary>
        [Serializable]
        public class ActionStatusName 
        {
            // 設定されてない場合などのID
            public const uint INVALID_UUID = 0;

            // ステータスの名前
            [SerializeField]
            protected string _name;
            public string name { get { return _name; } }
#if UNITY_EDITOR
            public void SetName(string n) { _name = n; }
#endif
            // 参照に使うuuid
            //[HideInInspector]
            [SerializeField]
            protected uint _uuid = INVALID_UUID;
            public uint uuid { get { return _uuid; } }
#if UNITY_EDITOR
            public ActionStatusName()
            {
                 _uuid = (uint)DateTime.Now.ToString().GetHashCode();               
            }            
#endif
        }

        /// <summary>
        /// ActionStatusNameを管理する
        /// </summary>
        [CreateAssetMenu(menuName = "Ashe/Action/StatusNames")]
        public class ActionStatusNames : ScriptableObject
        {
            // 子供の状態（共通の状態）
            // クラス的にいうと既定クラスにあたる
            // 例えば基本動作は子供に持たせ、プレイヤーならではの特殊状態は親に持たせる
            [SerializeField]
            private ActionStatusNames _child;
#if UNITY_EDITOR
            public ActionStatusNames child {
                get; set;
            }
#endif
            // 状態名のリスト
            [SerializeField]
            private List<ActionStatusName> _list;
#if UNITY_EDITOR
            public List<ActionStatusName> list {
                get { return _list; }
            }
#endif

            // 名前からUUIDを返す
            // 見つからない場合はINVALID_UUIDを返す
            public uint GetUUID(string name)
            {
                if(_child != null) {
                    uint id = _child.GetUUID(name);
                    if(id != ActionStatusName.INVALID_UUID) return id;
                }

                var e = list.Find((p) => { return p.name == name; });
                if(e != null) return e.uuid;
                return ActionStatusName.INVALID_UUID;
            }

            // UUIDから名前を返す
            public string GetName(uint uuid)
            {
                if(_child != null){
                    string name = _child.GetName(uuid);
                    if(!string.IsNullOrEmpty(name)) return name;
                }

                var e = list.Find((p) => { return p.uuid == uuid; });
                if(e != null) return e.name;
                return string.Empty;
            }

#if UNITY_EDITOR
            // すべてのステータス名をListにして返す
            public List<string> GetStringList()
            {
                List<string> ret = new List<string>();
                if(_child != null) ret = ret.Concat(_child.GetStringList()).ToList();                
                ret = ret.Concat(list.Select(e => { return e.name; }).ToList()).ToList();
                return ret;
            }
#endif
        }
    }
}