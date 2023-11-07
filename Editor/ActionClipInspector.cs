using UnityEngine;
using UnityEditor;

namespace Ashe
{
    namespace Editor
    {
        [CustomEditor(typeof(Ashe.Clip.ActionClip))]
        public class ActionClipInspector : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                EditorGUI.BeginChangeCheck();
                base.OnInspectorGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    var clip = target as Ashe.Clip.ActionClip;
                    if(clip != null) clip.UpdateGUIDs();
                }
            }
        }
    }
}