using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Ashe.Collection;


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
            public void Init()
            {
                startPosition = Ashe.Const.Vector2.zero;
                currentPosition = Ashe.Const.Vector2.zero;
                deltaPosition = Ashe.Const.Vector2.zero;
                touchDuration = 0.0f;
                phase = TouchPhase.Ended;
                fingerId = -1;
                startOverGameObject = false;
            }

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
            public TouchPhase phase = TouchPhase.Ended;
            /// <summary>
            /// ゲームオブジェクトの上からタッチが開始されたかどうか 
            /// </summary>
            public bool startOverGameObject = false;
            /// <summary>
            /// タッチ入力されている指番号、配列のインデックスと一致する
            /// </summary>
            public int fingerId = -1;

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
                ON,         // 触れた瞬間 
                STAY,       // 触れている間 
                OFF,        // 離した瞬間 
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
        /// タッチ入力を取るコマンド 
        /// </summary>
        UIntKeyDictionary<List<Event>> _events = new UIntKeyDictionary<List<Event>>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_event"></param>
        public void AddEvent(Event _event)
        {
            List<Event> eventList;
            if (!_events.TryGetValue((uint)_event.type, out eventList))
            {
                eventList = new List<Event>();
                _events.Add((uint)_event.type, eventList);
            }
            eventList.Add(_event);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_event"></param>
        public void RemoveEvent(Event _event)
        {
            List<Event> eventList;
            if (_events.TryGetValue((uint)_event.type, out eventList))
            {
                eventList.Remove(_event);
            }
        }

        /// <summary>
        /// タッチ入力の情報を格納するオブジェクトを初期化する 
        /// </summary>
        public TouchInput()
        {
            for (int i = 0; i < touchInfo.Length; ++i)
            {
                touchInfo[i] = new TouchInfo();
            }
        }

        /// <summary>
        /// タッチ入力の更新処理
        /// 登録されたイベントに対応するタッチ入力があればイベントを実行する 
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="eventSystem"></param>
        public void Update(float deltaTime, EventSystem eventSystem)
        {
            for (int i = 0; i < Input.touchCount; ++i)
            {
                // タッチ情報の更新 
                Touch touch = Input.touches[i];
                int id = touch.fingerId;
                touchInfo[id].deltaPosition = touch.deltaPosition;
                touchInfo[id].currentPosition = touch.position;                
                touchInfo[id].phase = touch.phase;
                touchInfo[id].fingerId = id;

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchInfo[id].startPosition = touch.position;
                        touchInfo[id].touchDuration = 0.0f;
                        if (eventSystem.IsPointerOverGameObject(id))
                        {
                            touchInfo[id].startOverGameObject = true;
                            continue;
                        }

                        // ONのイベント発行 
                        DispatchEvent(Event.TYPE.ON, touchInfo[id]);
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        touchInfo[id].touchDuration += deltaTime;
                        // ボタンの上からタッチを開始した場合はイベントを発行しない
                        if (touchInfo[id].startOverGameObject) continue;                        
                        // STAYのイベント発行 
                        DispatchEvent(Event.TYPE.STAY, touchInfo[id]);
                        break;

                    case TouchPhase.Ended:
                        // ボタンの上からタッチを開始した場合はイベントを発行しない 
                        if (touchInfo[id].startOverGameObject)
                        {
                            touchInfo[id].startOverGameObject = false;
                            continue;
                        }

                        float distance = touchInfo[id].GetSqrDistanceFromStart();

                        // TAP判定 
                        if (touchInfo[id].touchDuration <= tapThresholdTime && distance <= flickThresholdDistance)
                        {
                            DispatchEvent(Event.TYPE. TAP, touchInfo[id]);
                        }
                        else
                        {
                            DispatchEvent(Event.TYPE.OFF, touchInfo[id]);
                        }

                        break;
                }

            }
        }

        /// <summary>
        /// タッチイベントを発行する 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="touchInfo"></param>
        private void DispatchEvent(Event.TYPE type, TouchInfo touchInfo)
        {
            List<Event> eventList;
            if (_events.TryGetValue((uint)type, out eventList))
            {
                for (int i = 0; i < eventList.Count; ++i)
                {
                    eventList[i].onTouch(touchInfo);
                }
            }
        }
    }
}