using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Ashe.GameAction;

namespace Ashe
{
    namespace Editor
    {
        [CustomEditor(typeof(ActionData))]
        public class ActionDataInspector : UnityEditor.Editor
        {
            bool _showStatus;
            bool _showBlockList;

            ActionStatusNamePopupList _popupListForStatus = new ActionStatusNamePopupList();
            ActionStatusNamePopupList _popupListForBlock = new ActionStatusNamePopupList();

            public override void OnInspectorGUI()
            {
                var data = target as ActionData;
                base.OnInspectorGUI();
                if(data.statusNames == null){
                    return;
                } 
                _popupListForStatus.Initialize(data.statusNames);
                _popupListForBlock.Initialize(data.statusNames);

                _showStatus = EditorGUILayout.Foldout(_showStatus, "付与Status");
                if(_showStatus){
                    _popupListForBlock.DrawAndUpdatePopupList(data.status);
                }
                _showBlockList = EditorGUILayout.Foldout(_showBlockList, "このステーテスのときは遷移不可");
                if(_showBlockList){
                    _popupListForBlock.DrawAndUpdatePopupList(data.blockStatusList);
                }
            }
        }
    }
}
