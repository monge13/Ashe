using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
            struct PointInfo
            {
                public Vector3 position;
                public Action<PointFollower> onReached;
            }

            // 操作対象のTransform。NULLの場合は自身のTransformを自動代入する。
            [SerializeField]
            Transform target;

            // 辿るポイント達
            [SerializeField]
            PointInfo[] points;

            // 終着点に到着した後に始点にループするかどうか
            [SerializeField]
            bool loop;

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

            // 再生中かどうか
            bool playing;
            public bool isPlaying
            {
                get { return playing; }
            }

            // 起動時にPlayするかどうか
            [SerializeField]
            bool playOnAwake = false;

            // 現在の目的地インデックス
            int targetIndex = 0;

            // 全長
            float fullLength;

            void Start()
            {
                playing = playOnAwake;
                if (target == null) target = transform;
                Initialize();
            }

            // 初期化処理
            // 全長を計算して速度を求める
            void Initialize()
            {
                // 速度をDURATIONで制御する場合は初期化で計算する
                if (speedMode == SPEED_MODE.BY_DURATION)
                {
                    fullLength = 0.0f;
                    for (int i = 1; i < points.Length; ++i)
                    {
                        fullLength += Vector3.Magnitude(points[i].position - points[i - 1].position);
                    }
                    if (duration > Ashe.Const.Float.EPSILON)
                    {
                        speed = fullLength / duration;
                    }
                }
                // 初期座標の設定
                target.position = points[0].position;
                targetIndex = 1;
                if(targetIndex >= points.Length) {
                    Debug.Log.W("Points length is 1");
                    playing = false;
                }
            }

            void Update()
            {
                if (!isPlaying) return;
                Vector3 toTarget = points[targetIndex].position - target.position;
                float sqrDistanceToNext = Vector3.SqrMagnitude(toTarget);
                float deltaTimedSpeed = speed * Time.deltaTime;
                float sqrSpeed = deltaTimedSpeed * deltaTimedSpeed;
                if (sqrDistanceToNext <= sqrSpeed)
                {
                    if (points[targetIndex].onReached != null)
                    {
                        points[targetIndex].onReached(this);
                        // onReachedの中でpauseされることがある
                        if (!isPlaying) return;
                    }

                    // ループする場合は先頭インデックスにターゲットを戻す
                    if (loop)
                    {
                        int next = (targetIndex + 1) % points.Length;
                        Vector3 direction = Vector3.Normalize(points[next].position - points[targetIndex].position);
                        target.position = points[targetIndex].position + direction * (deltaTimedSpeed - Mathf.Sqrt(sqrDistanceToNext));
                        targetIndex = next;
                    }
                    else
                    {
                        target.position = points[targetIndex].position;
                        // 最後の目標地点であれば移動を停止する
                        if(targetIndex == points.Length-1) playing = false;
                    }
                }
                else
                {
                    target.position += toTarget.normalized * deltaTimedSpeed;
                }
            }
        }
    }
}