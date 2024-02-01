using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Ashe.GameAction;
using UnityEditorInternal;

namespace Ashe
{
    namespace Editor
    {
        [CustomEditor(typeof(ActionStatusNames))]
        public class ActionStatusNamesInspector : UnityEditor.Editor
        {
            List<ActionStatusName> _names = null;
            private ReorderableList _reorderableList;

            //要素を追加する
            private void AddElement(ReorderableList list){
                _names.Add(new ActionStatusName());
            }

            // 名前だけ取得して更新する
            void UpdateNames()
            {
                var statusNames = target as ActionStatusNames;
                if(statusNames != null){
                    _names = statusNames.list;
                }
                //ReorderableListを作成
                _reorderableList = new ReorderableList(
                    elements            : _names,    //要素
                    elementType         : typeof(ActionStatusName), //要素の種類
                    draggable           : true,           //ドラッグして要素を入れ替えられるか
                    displayHeader       : true,           //ヘッダーを表示するか
                    displayAddButton    : true,           //要素追加用の+ボタンを表示するか
                    displayRemoveButton : true            //要素削除用の-ボタンを表示するか
                );
                _reorderableList.onAddCallback += AddElement;
                _reorderableList.drawElementCallback += DrawElement;
            }

            //要素の描画
            private void DrawElement(Rect rect, int index, bool isActive, bool isFocused){
                 //要素を書き換えられるようにフィールドを表示
                _names[index].SetName(EditorGUI.TextField (rect, _names[index].name));
                // Debug用
                //EditorGUILayout.LabelField(_names[index].uuid.ToString());
            }

            public override void OnInspectorGUI()
            {
                if(_names == null) {
                    UpdateNames();
                }
                ActionStatusNames actionStatusNames = target as ActionStatusNames;
                actionStatusNames.child = (ActionStatusNames)EditorGUILayout.ObjectField(actionStatusNames.child, typeof(ActionStatusNames), false);
                if(_reorderableList != null){
                    _reorderableList.DoLayoutList();
                }
            }

        }
    }
}