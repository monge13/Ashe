using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ashe
{
    namespace GameAction
    {
        /// <summary>
        /// Action ClipとAction Statusをセットで格納する
        /// </summary>
        [CreateAssetMenu(menuName = "Ashe/Action/Data")]
        public class ActionData : ScriptableObject
        {
            // Actionに付随するステータスセット
            [SerializeField]
            ActionStatusNames _statusNames;
            public ActionStatusNames statusNames { get { return _statusNames; } }

            // このアクションによって付与されるステータス
            [SerializeField]
            [HideInInspector]
            List<uint> _status;
            public List<uint> status { get { return _status; } }

            // ここに指定されたリストへは遷移できない
            [SerializeField]
            [HideInInspector]
            List<uint> _blockStatusList;
            public List<uint> blockStatusList { get { return _blockStatusList; } }

            // Actionの動きを定義するClipデータ
            [SerializeField]
            ActionClip _clip;
            public ActionClip clip { get { return _clip; } }
        }
    }
}
