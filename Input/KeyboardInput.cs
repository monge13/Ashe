using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Ashe
{
    /// <summary>
    /// キーボード入力の取得  
    /// </summary>
    public class KeyboardInput
    {
        /// <summary>
        /// マウス入力の際の座標を保持する
        /// </summary>
        public class MouseInfo
        {
            public Vector2 startPosition;
            public Vector2 currentPosition;
            public Vector2 deltaPosition;
        }
        const int MOUSE_BUTTON_NUM = 5;
        MouseInfo[] mouseInfo = new MouseInfo[MOUSE_BUTTON_NUM];

        /// <summary>
        /// キー入力のイベント 
        /// </summary>
        public class Event
        {
            /// <summary>
            /// コンストラクタ 
            /// </summary>
            /// <param name="_keyCode">Key code.</param>
            /// <param name="_onDown">キーが押された時に呼ばれるイベント</param>
            /// <param name="_onKey">キーが押されている間呼ばれるイベント</param>
            /// <param name="_onUp">キーが離された時に呼ばれるイベント</param>
            public Event(KeyCode _keyCode, Action<MouseInfo> _onDown = null, Action<MouseInfo> _onKey = null, Action<MouseInfo> _onUp = null)
            {
                keycode = _keyCode;
                onDown = _onDown;
                onKey = _onKey;
                onUp = _onUp;
            }

            /// <summary>
            /// このイベントが呼ばれるトリガーとなるキー 
            /// </summary>
            public KeyCode keycode;

            /// <summary>
            /// キー入力されたら実行されるコマンド 
            /// </summary>
            public Action<MouseInfo> onDown;
            public Action<MouseInfo> onKey;
            public Action<MouseInfo> onUp;
        }

        public KeyboardInput()
        {
            for (int i = 0; i < mouseInfo.Length; ++i)
            {
                mouseInfo[i] = new MouseInfo();
            }
        }

        /// <summary>
        /// キーボード入力をとるコマンド 
        /// </summary>
        List<Event> _eventList = new List<Event>();
        public void AddEvent(Event _event)
        {
            _eventList.Add(_event);
        }
        public void RemoveEvent(Event _event)
        {
            _eventList.Remove(_event);
        }

        private bool IsMouseButton(KeyCode key)
        {
            return key == KeyCode.Mouse0 ||
                key == KeyCode.Mouse1 ||
                key == KeyCode.Mouse2;
        }

        public void Update(float deltaTime, EventSystem eventSystem)
        {
            Vector2 newMousePosition = Input.mousePosition;
            for(int i = 0; i < mouseInfo.Length; ++i)
            {
                KeyCode key = KeyCode.Mouse0 + i;
                if(Input.GetKeyDown(key))
                {
                    mouseInfo[i].startPosition = newMousePosition;
                    mouseInfo[i].currentPosition = newMousePosition;
                    mouseInfo[i].deltaPosition = Ashe.Const.Vector2.zero;
                }
                else if (Input.GetKey(key))
                {
                    mouseInfo[i].deltaPosition = newMousePosition - mouseInfo[i].currentPosition;
                    mouseInfo[i].currentPosition = newMousePosition;
                }
            }

            int count = _eventList.Count;
            for (int i = 0; i < count; ++i)
            {
                var _event = _eventList[i];
                if(Input.GetKeyDown(_event.keycode))
                {
                    // マウス入力とUI要素が被っていたら無視する 
                    if (IsMouseButton(_event.keycode) && eventSystem.IsPointerOverGameObject())
                    {
                        continue;
                    }
                    if (_event.onDown != null)
                    {
                        MouseInfo mi = mouseInfo[0];
                        if(IsMouseButton(_event.keycode))
                        {
                            mi = mouseInfo[_event.keycode - KeyCode.Mouse0];
                        }
                        _event.onDown(mi);
                    }
                }
                else if (Input.GetKey(_event.keycode))
                {
                    // マウス入力とUI要素が被っていたら無視する 
                    if (IsMouseButton(_event.keycode) && eventSystem.IsPointerOverGameObject())
                    {
                        continue;
                    }
                    if (_event.onKey != null)
                    {
                        MouseInfo mi = mouseInfo[0];
                        if (IsMouseButton(_event.keycode))
                        {
                            mi = mouseInfo[_event.keycode - KeyCode.Mouse0];
                        }
                        _event.onKey(mi);

                    }
                }
                else if (Input.GetKeyUp(_event.keycode))
                {
                    if (_event.onUp != null)
                    {
                        MouseInfo mi = mouseInfo[0];
                        if (IsMouseButton(_event.keycode))
                        {
                            mi = mouseInfo[_event.keycode - KeyCode.Mouse0];
                        }
                        _event.onUp(mi);

                    }
                }
            }

        }

    }
}