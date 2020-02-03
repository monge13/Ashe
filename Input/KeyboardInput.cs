using System;
using UnityEngine;
using System.Collections.Generic;

namespace Ashe
{
    /// <summary>
    /// キーボード入力の取得  
    /// </summary>
    public class KeyboardInput
    {
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
            public Event(KeyCode _keyCode, Action _onDown = null, Action _onKey = null, Action _onUp = null)
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
            public Action onDown;
            public Action onKey;
            public Action onUp;
        }

        /// <summary>
        /// キーボード入力をとるコマンド 
        /// </summary>
        List<Event> _eventList = new List<Event>();
        public List<Event> eventList
        {
            get { return _eventList; }
        }


        public void Update(float deltaTime)
        {
            if(!Input.anyKey)
            {
                return;
            }

            int count = eventList.Count;
            for (int i = 0; i < count; ++i)
            {
                var _event = eventList[i];
                if(Input.GetKeyDown(_event.keycode))
                {
                    if (_event.onDown != null)
                    {
                        _event.onDown();
                    }
                }
                if (Input.GetKey(_event.keycode))
                {
                    if (_event.onKey != null)
                    {
                        _event.onKey();
                    }
                }
                if (Input.GetKeyUp(_event.keycode))
                {
                    if (_event.onUp != null)
                    {
                        _event.onUp();
                    }
                }
            }

        }

    }
}