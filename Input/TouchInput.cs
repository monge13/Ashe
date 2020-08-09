using System;
using UnityEngine;

namespace Ashe
{
    /// <summary>
    /// タッチ入力の取得  
    /// </summary>
    public class TouchInput
    {
        /// <summary>
        /// タッチ情報 
        /// </summary>
        public class TouchInfo
        {
            /// <summary>
            /// タッチ開始地点座標
            /// </summary>
            public Vector2 startPosition; 
            /// <summary>
            /// 現在のタッチ座標
            /// </summary>
            public Vector2 currentPosition;
            /// <summary>
            /// 前回のタッチ座標から今回のタッチ座標への差分 
            /// </summary>
            public Vector2 deltaPosition;
            /// <summary>
            /// タッチ中の時間 
            /// </summary>
            public float touchDuration;
            /// <summary>
            /// タッチ状態 
            /// </summary>
            public TouchPhase pahese = TouchPhase.Ended;
            /// <summary>
            /// タッチ開始地点からの現在のタッチ位置へのベクトルを返す
            /// </summary>
            /// <returns></returns>
            public Vector3 GetDeltaFromStart()
            {
                return currentPosition - startPosition;
            }
            /// <summary>
            /// タッチ開始地点からの現在のタッチ位置への距離の二乗を返す
            /// </summary>
            /// <returns></returns>
            public float GetSqrDistanceFromStart()
            {
                return GetDeltaFromStart().sqrMagnitude;
            }
            /// <summary>
            /// タッチ開始地点からの現在のタッチ位置への距離を返す
            /// </summary>
            /// <returns></returns>
            public float GetDistanceFromStart()
            {
                return GetDeltaFromStart().magnitude;
            }

        }
        const int MAX_TOUCH_NUM = 5;
        TouchInfo[] touchInfo = new TouchInfo[MAX_TOUCH_NUM];

        /// <summary>
        /// この時間内にタッチオンされてオフされたらタップとみなす 
        /// </summary>
        float tapThresholdTime = 0.3f;

        /// <summary>
        /// この時間内にタッチオンされてオフされたことがフリックの条件の一つとする
        /// </summary>
        float flickThresholdTime = 0.3f;
        /// <summary>
        /// この距離以上原点からの入力距離があればフリックとみなす 
        /// </summary>
        float flickThresholdDistance = 1.0f;



        /// <summary>
        /// タッチ入力のイベント 
        /// </summary>
        public class Event
        {
            public enum TYPE
            {
                TAP,        // 一定の時間以下のタッチオンからのタッチオフをタップとする
                ON,
                OFF,

            }

            public Event(TYPE _type, Action<TouchInfo> _onTouch)
            {
                type = _type;
                onTouch = _onTouch;
            }

            public TYPE type;
            public Action<TouchInfo> onTouch;
        }

        /// <summary>
        /// タッチ入力の情報を格納するオブジェクトを初期化する 
        /// </summary>
        public TouchInput()
        {
            for(int i = 0; i < touchInfo.Length; ++i)
            {
                touchInfo[i] = new TouchInfo();
            }
        }

        public void Update(float deltaTime)
        {
            for(int i = 0; i <  Input.touchCount; ++i)
            {
                // タッチ情報の更新 
                Touch touch = Input.touches[i];
                touchInfo[i].deltaPosition = touch.deltaPosition;
                touchInfo[i].currentPosition = touch.position;
                touchInfo[i].pahese = touch.phase;
                switch(touch.phase)
                {
                    case TouchPhase.Began:
                        touchInfo[i].startPosition = touch.position;
                        touchInfo[i].touchDuration = 0.0f;
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        touchInfo[i].touchDuration += deltaTime;
                        break;

                    case TouchPhase.Ended:
                        float distance = touchInfo[i].GetSqrDistanceFromStart();

                        // TAP判定 
                        if(touchInfo[i].touchDuration <= tapThresholdTime && distance <= flickThresholdDistance)
                        {
                            Debug.Log.I("Tap");
                        }
                        
                        break;
                }



            }
        }

    }
}