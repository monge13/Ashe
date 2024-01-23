using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Ashe
{
    namespace Editor
    {
        [CustomEditor(typeof(Ashe.Clip.ActionClip))]
        public class EffectObjectInspector : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                EditorGUI.BeginChangeCheck();
                base.OnInspectorGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    var eo = target as Effect.EffectObject;
                    if(eo != null) eo.UpdateGUID();
                }
            }
        }
    }
}