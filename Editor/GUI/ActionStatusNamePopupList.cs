using System;
using System.Collections.Generic;
using Ashe.GameAction;
using UnityEngine;
using UnityEditor;

namespace Ashe
{
    namespace Editor
    {
        /// <summary>
        /// InspectorなどでActionStatusをポップアップ表示する
        /// </summary>
        public class ActionStatusNamePopupList 
        {
            ActionStatusNames _statusNames;
            string[] _statusNameStringArray;

            // 表示するポップアップを作る
            public void Initialize(ActionStatusNames statusNames)
            {
                if(statusNames == _statusNames) return;
                _statusNames = statusNames;
                _statusNameStringArray = _statusNames.GetStringList().ToArray();
            }


            public void DrawAndUpdatePopupList(List<uint> ids)
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