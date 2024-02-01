using System;
using System.Collections;
using System.Collections.Generic;
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
            protected uint _uuid;
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
        [CreateAssetMenu(menuName = "Ashe/ActionStatusNames")]
        public class ActionStatusNames : ScriptableObject
        {
            [SerializeField]
            private ActionStatusNames _child;
#if UNITY_EDITOR
            public ActionStatusNames child {
                get; set;
            }
#endif

            [SerializeField]
            private List<ActionStatusName> _list;
#if UNITY_EDITOR
            public List<ActionStatusName> list {
                get { return _list; }
            }
#endif
        }
    }
}