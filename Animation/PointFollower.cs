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
        public interface PointFollowerReachedEventListner
        {
            void onReached(int index, string name, float value);
        }

        /// <summary>
        /// ポイントをおいてその上を辿る機能
        /// </summary>
        public class PointFollower : MonoBehaviour
        {
            // 操作対象のTransform。NULLの場合は自身のTransformを自動代入する。
            [SerializeField]
            Transform target;

            // ポイント到着のイベントを受け取るオブジェクト
            List<PointFollowerReachedEventListner> eventReceiveres;
            void AddEventReceiver(PointFollowerReachedEventListner receiver)
            {
                eventReceiveres.Add(receiver);
            }
            void RemoveEventReceiver(PointFollowerReachedEventListner receiver)
            {
                eventReceiveres.Remove(receiver);
            }

            // 辿るポイント達
            [SerializeField]
            PointsInfo _pointsInfo;
            #if UNITY_EDITOR
            public PointsInfo pointsInfo 
            {
                get { return _pointsInfo; }
            }
            #endif

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
                target.position = _pointsInfo.points[0].position;
                targetIndex = 1;
                if (targetIndex >= _pointsInfo.points.Length)
                {
                    Debug.Log.W("Points length is 1");
                    return;
                }

                tweener = target.DOPath(
                    path: _pointsInfo.GetPositions(),
                    duration: _pointsInfo.GetDuration()
                ).OnWaypointChange(pointNo =>
                { 
                    if(pointNo >= _pointsInfo.points.Length) {
                        return;
                    }
                    
                    if(eventReceiveres == null) {
                        return;
                    }

                    // レシーバー全てにイベントを渡す
                    foreach (var receiver in eventReceiveres)
                    {
                        foreach(var pointEvent in _pointsInfo.points[pointNo].events){
                            receiver.onReached(pointNo, pointEvent.name, pointEvent.value);
                        }
                    }
                });

                if(_pointsInfo.lookAtRate > Ashe.Const.Float.EPSILON) tweener.SetLookAt(_pointsInfo.lookAtRate, Vector3.forward);
                if(_pointsInfo.loop) tweener.SetLoops(-1);            
            }
        }
    }
}