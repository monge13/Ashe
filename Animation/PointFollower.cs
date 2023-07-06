using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core.PathCore;
using DG.Tweening.Plugins.Options;

namespace Ashe
{
    namespace Animation
    {
        /// <summary>
        /// ポイントをおいてその上を辿る機能
        /// </summary>
        public class PointFollower : MonoBehaviour
        {
            // 操作対象のTransform。NULLの場合は自身のTransformを自動代入する。
            [SerializeField]
            Transform target;

            // 辿るポイント達
            [SerializeField]
            PointsInfo _pointsInfo;
            #if UNITY_EDITOR
            public PointsInfo pointsInfo 
            {
                get { return _pointsInfo; }
            }
            #endif

            // 一個前に到着したポイント
            int oldReachedNo = -1;

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

            public void Play()
            {
                tweener.Play();
            }

            public void Pause()
            {
                tweener.Pause();
            }

            public void Restart()
            {
                tweener.Restart();
            }

            void Start()
            {
                if (target == null) target = transform;
                Initialize();
                if(!playOnAwake) tweener.Pause();
            }

            // 初期化処理
            // 全長を計算して速度を求める
            public void Initialize(PointsInfo info = null)
            {
                if(info != null) _pointsInfo = info;
                if(_pointsInfo == null) return;

                // 初期座標の設定
                target.position = _pointsInfo.points[0];
                targetIndex = 1;
                if (targetIndex >= _pointsInfo.points.Length)
                {
                    D.Log.W("Points length is 1");
                    return;
                }

                tweener = target.DOPath(
                    path: _pointsInfo.points,
                    duration: _pointsInfo.GetDuration()
                ).OnWaypointChange(pointNo =>
                { 
                    if(pointNo >= _pointsInfo.points.Length) {
                        return;
                    }
                    // ループした際に同じ番号が呼ばれることがある。
                    if(pointNo != oldReachedNo) {
                        onReachedPoint(pointNo);
                    }
                    oldReachedNo = pointNo;
                });

                if(_pointsInfo.lookAtRate > Ashe.Const.Float.EPSILON) tweener.SetLookAt(_pointsInfo.lookAtRate, Vector3.forward);
                if(_pointsInfo.loop) tweener.SetLoops(-1);            
            }

            protected virtual void onReachedPoint(int pointNo)
            {
            }
        }
    }
}