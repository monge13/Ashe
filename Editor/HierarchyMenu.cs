using UnityEditor;
using UnityEngine;

namespace Ashe
{
    namespace Editor
    {
        /// <summary>
        /// ヒエラルキービューに右クリックメニューを追加
        /// </summary>
        public class HierarchyMenu {

            [MenuItem("Edit/CopyTransformName", false, 0)]
            public static void CopyTransformName()
            {
                var gameObject = Selection.activeGameObject;
                GUIUtility.systemCopyBuffer = GetPath(gameObject.transform);
            }

            public static string GetPath(Transform self)
            {
                string path = self.gameObject.name;
                var root = PrefabUtility.GetOutermostPrefabInstanceRoot(self);

                Transform parent = self.parent;

                while (parent != null)
                {
                    path = parent.name + "/" + path;            
                    parent = parent.parent;            
                    if(parent.gameObject == root.gameObject) break; 
                }    
                return path;
            }
        }
    }
}