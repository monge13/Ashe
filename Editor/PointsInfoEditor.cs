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
                var points = pointsInfo.points;
                if(points == null) return;
                bool isDirty = false;                
                for (int i = 0; i < points.Length; ++i)
                {
                    DrawPointOnScene(i, ref points[i], ref isDirty);           
                }
                if (isDirty) pointsInfo.points = points;

                var positions = points.Select(point => point.position).ToList();
                if(pointsInfo.loop) positions.Add(points[0].position);
                Handles.DrawPolyLine(positions.ToArray());
            }

            void DrawPointOnScene(int index, ref Ashe.Animation.PointsInfo.PointEvent pointInfo, ref bool isDirty)
            {
                Handles.Label(pointInfo.position, index.ToString());
                EditorGUI.BeginChangeCheck();
                var pointHandle = Handles.PositionHandle(
                    pointInfo.position,
                    Quaternion.identity
                );
                if (EditorGUI.EndChangeCheck())
                {
                    isDirty = true;
                    pointInfo.position = pointHandle;
                }
            }
        }
    }
}

