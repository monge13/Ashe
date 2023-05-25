using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ashe
{
    namespace Animation
    {
        /// <summary>
        /// 座標データと保管形式などを保持するデータクラス
        /// </summary>
        [CreateAssetMenu(menuName = "Ashe/Animation/PointFollowerData")]
        public class PointsInfo : ScriptableObject
        {
            [SerializeField]
            Vector3[] _points;
            public Vector3[] points
            {
                get { return _points; }
            }

            // 追いかけるときの速度を決める要因
            enum SPEED_MODE
            {
                BY_SPEED,       // 速度をそのまま使う
                BY_DURATION,    // DURATIONから速度を求める
            }
            [SerializeField]
            SPEED_MODE speedMode = SPEED_MODE.BY_SPEED;

            // 歩いていく速度
            [SerializeField]
            float speed = 1.0f;
            // 何秒で１ループするか
            [SerializeField]
            float duration = 1.0f;

            // どれくらい進行方向を向くか
            [SerializeField]
            float _lookAtRate = 0.3f;
            public float lookAtRate 
            {
                get { return _lookAtRate; }
            }

            // 終着点に到着した後に始点にループするかどうか
            [SerializeField]
            bool _loop;
            public bool loop
            {
                get { return _loop; }
            }

            // 全長を返す
            public float GetFullLength()
            {
                float fullLength = 0.0f;
                for (int i = 1; i < _points.Length; ++i)
                {
                    fullLength += Vector3.Magnitude(_points[i] - _points[i - 1]);
                }
                if(loop) fullLength += Vector3.Magnitude(_points[0] - _points[_points.Length-1]);
                return fullLength;
            }

            // 何秒で一周するかを返す
            public float GetDuration()
            {
                float fullLength = GetFullLength();  
                // SPEEDを使う場合は全長から計策する
                if (speedMode == SPEED_MODE.BY_SPEED)
                {
                    if (speed > Ashe.Const.Float.EPSILON)
                    {
                        return fullLength / speed;
                    }
                }
                return duration;
            }
        }
    }
}