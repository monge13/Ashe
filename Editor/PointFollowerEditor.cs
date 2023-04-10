using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Ashe
{
    namespace Editor
    {
        [CustomEditor(typeof(Ashe.Animation.PointFollower))]
        public class PointFollowerEditor : UnityEditor.Editor
        {
            Ashe.Animation.PointFollower GetTarget()
            {
                return target as Ashe.Animation.PointFollower;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
            }

            // ポイントのGUI
            void BuildPointWidget(int inex)
            {
                var follower = GetTarget();
            }

            public void OnSceneGUI()
            {
                var follower = GetTarget();
                var points = follower.points;
                if(points == null) return;
                bool isDirty = false;                
                for (int i = 0; i < points.Length; ++i)
                {
                    DrawPointOnScene(i, ref points[i], ref isDirty);           
                }
                if (isDirty) follower.points = points;

                var positions = points.Select(point => point.position).ToList();
                if(follower.loop) positions.Add(points[0].position);
                Handles.DrawPolyLine(positions.ToArray());
            }

            void DrawPointOnScene(int index, ref Ashe.Animation.PointFollower.PointInfo pointInfo, ref bool isDirty)
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

