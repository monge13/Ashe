using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;
using System.Linq;
using UnityEngine.Events; 

namespace Ashe
{
    namespace Animation
    {
        /// <summary>
        /// ポイントをおいてその上を辿る機能
        /// </summary>
        public class PointFollower : MonoBehaviour
        {
            [Serializable]
            public struct PointInfo
            {
                public Vector3 position;
                public UnityEvent<PointFollower> onReached;
            }

            // 操作対象のTransform。NULLの場合は自身のTransformを自動代入する。
            [SerializeField]
            Transform target;

            // 辿るポイント達
            [SerializeField]
            PointInfo[] _points;
            public PointInfo[] points
            {
                get { return _points; }
                #if UNITY_EDITOR
                set { _points = value; }
                #endif
            }


            // 終着点に到着した後に始点にループするかどうか
            [SerializeField]
            bool _loop;
            public bool loop {
                get { return _loop; }
            }
            // 追いかけるモード
            enum MOVEMENT_MODE
            {
                LINEAR,
                SPLINE,
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
            float lookAtRate = 0.3f;

            // 再生中かどうか
            public bool isPlaying
            {
                get { return tweener.IsPlaying(); }
            }

            // 起動時にPlayするかどうか
            [SerializeField]
            bool playOnAwake = false;

            // 現在の目的地インデックス
            int targetIndex = 0;

            // 全長
            float fullLength;

            // Tweener
            TweenerCore<Vector3, Path, PathOptions> tweener;

            void Start()
            {
                if (target == null) target = transform;
                Initialize();
                if(!playOnAwake) tweener.Pause();
            }

            // 初期化処理
            // 全長を計算して速度を求める
            void Initialize()
            {
                if (speedMode == SPEED_MODE.BY_SPEED)
                {
                    fullLength = 0.0f;
                    for (int i = 1; i < points.Length; ++i)
                    {
                        fullLength += Vector3.Magnitude(points[i].position - points[i - 1].position);
                    }
                    if (speed > Ashe.Const.Float.EPSILON)
                    {
                        duration = fullLength / speed;
                    }
                }
                // 初期座標の設定
                target.position = points[0].position;
                targetIndex = 1;
                if (targetIndex >= points.Length)
                {
                    Debug.Log.W("Points length is 1");
                    return;
                }

                tweener = target.DOPath(
                    path: points.Select(point => point.position).ToArray(),
                    duration: duration
                ).OnWaypointChange(pointNo =>
                { 
                    points[pointNo].onReached?.Invoke(this);
                });

                if(lookAtRate > Ashe.Const.Float.EPSILON) tweener.SetLookAt(lookAtRate, Vector3.forward);
                if(loop) tweener.SetLoops(-1);            
            }
        }
    }
}