using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Ashe
{
    namespace Editor
    {
        [CustomEditor(typeof(Ashe.Animation.PointsInfo))]
        public class PointsInfoEditor : UnityEditor.Editor
        {
            Ashe.Animation.PointsInfo GetTarget()
            {
                return target as Ashe.Animation.PointsInfo;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
            }

            void OnEnable()
            {
                SceneView.duringSceneGui += OnSceneGUI;
            }
            
            void OnDisable()
            {
                SceneView.duringSceneGui -= OnSceneGUI;
            }

            public void OnSceneGUI(SceneView sceneView)
            {
                var pointsInfo = GetTarget();
                bool isDirty = false;                
                for (int i = 0; i < pointsInfo.points.Length; ++i)
                {
                    DrawPointOnScene(i, ref pointsInfo);           
                }
                Handles.DrawPolyLine(pointsInfo.points);
            }

            void DrawPointOnScene(int index, ref Ashe.Animation.PointsInfo pointInfo)
            {
                Handles.Label(pointInfo.points[index], index.ToString());
                EditorGUI.BeginChangeCheck();
                var pointHandle = Handles.PositionHandle(
                    pointInfo.points[index],
                    Quaternion.identity
                );
                if (EditorGUI.EndChangeCheck())
                {
                   pointInfo.points[index] = pointHandle;
                }
            }
        }
    }
}

