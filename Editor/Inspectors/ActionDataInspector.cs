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

            ActionStatusNames _statusNames;
            string[] _statusNameStringArray;
            public override void OnInspectorGUI()
            {
                var data = target as ActionData;
                base.OnInspectorGUI();
                if(data.statusNames == null){
                    return;
                } 
                if(_statusNames != data.statusNames) {
                    _statusNames =data.statusNames;
                    _statusNameStringArray = _statusNames.GetStringList().ToArray();
                }
                


                _showStatus = EditorGUILayout.Foldout(_showStatus, "付与Status");
                if(_showStatus){
                    DrawStatusNames(data.status);
                }
                _showBlockList = EditorGUILayout.Foldout(_showBlockList, "このステーテスのときは遷移不可");
                if(_showBlockList){
                    DrawStatusNames(data.blockStatusList);
                }
            }

            public void DrawStatusNames(List<uint> ids)
            {
                int removeIndex = -1;
                for(int i = 0; i < ids.Count; ++i){
                    uint id = ids[i];
                    string text = _statusNames.GetName(id);
                    GUILayout.BeginHorizontal();
                    // 名前の更新
                    int currentIndex = Array.IndexOf(_statusNameStringArray, text);
                    int newIndex = EditorGUILayout.Popup(currentIndex, _statusNameStringArray);
                    if(currentIndex != newIndex) {
                        ids[i] = _statusNames.GetUUID(_statusNameStringArray[newIndex]);
                    }

                    // 要素の削除
                    if(GUILayout.Button("-")){
                        removeIndex = i;
                    }
                    GUILayout.EndHorizontal();
                }
                if(removeIndex != -1){
                    ids.RemoveAt(removeIndex);
                }
                
                if(GUILayout.Button("Add Element")){
                    ids.Add(ActionStatusName.INVALID_UUID);
                }
            }
        }
    }
}
