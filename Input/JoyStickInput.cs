using System;
using UnityEngine;
using System.Collections.Generic;

namespace Ashe
{
    /// <summary>
    /// ジョイスティックの入力 
    /// </summary>
    [System.Serializable]
    public class JoyStickInput
    {
        public class Event
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="T:Ashe.JoyStickInput.Event"/> class.
            /// </summary>
            /// <param name="_button">イベントのトリガーとなるボタン</param>
            /// <param name="_onDown">ボタンが押された時に呼ばれるイベント</param>
            /// <param name="_onKey">ボタンが押されている間に呼ばれるイベント</param>
            /// <param name="_onUp">ボタンが離れさてた時に呼ばれるイベント</param>
            public Event(BUTTON _button, Action _onDown = null, Action _onKey = null, Action _onUp = null)
            {
                joystickButton = _button;
                onDown = _onDown;
                onKey = _onKey;
                onUp = _onUp;
            }

            /// <summary>
            /// このイベントが呼ばれるトリガーとなるボタン 
            /// </summary>
            public BUTTON joystickButton;
            /// <summary>
            /// キー入力されたら実行されるコマンド 
            /// </summary>
            public Action onDown;
            public Action onKey;
            public Action onUp;
        }

        /// <summary>
        /// アナログスティック入力のイベント 
        /// </summary>
        public class StickEvent
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="T:Ashe.JoyStickInput.StickEvent"/> class.
            /// </summary>
            /// <param name="_onMoveL">左アナログスティックが動いたときのイベント</param>
            /// <param name="_onMoveR">On move r.</param>
            public StickEvent(Action<Vector2, Vector2> _onMoveL = null, Action<Vector2, Vector2> _onMoveR = null)
            {
                onMoveL = _onMoveL;
                onMoveR = _onMoveR;
            }

            /// <summary>
            /// Leftスティックの入力があったときに呼ばれるイベント 
            /// param1 : 入力値 / param2 : 前回からの差分
            /// </summary>
            public Action<Vector2, Vector2> onMoveL;
            /// <summary>
            /// Rightスティックの入力があったときに呼ばれるイベント 
            /// param1 : 入力値 / param2 : 前回からの差分
            /// </summary>
            public Action<Vector2, Vector2> onMoveR;
        }

        /// <summary>
        /// キー入力を判定しイベントを発行する 
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            analogLStickInput = Const.Vector2.zero;
            oldAnalogLStickInput = Const.Vector2.zero;

            // 左アナログスティックの更新処理 
			float x = Input.GetAxis(analogLStickKey_X);
			float y = Input.GetAxis(analogLStickKey_Y);

            Vector2 tempAnalogInput = new Vector2(x, y);
            if (tempAnalogInput.sqrMagnitude > threshold * threshold)
            {
                oldAnalogLStickInput = analogLStickInput;
                analogLStickInput = tempAnalogInput;

                int stickEventCount = _stickEventList.Count;
                for (int i = 0; i < stickEventCount; ++i)
                {
                    var stickEvent = _stickEventList[i];
                    if (stickEvent.onMoveL != null)
                    {
                        stickEvent.onMoveL(analogLStickInput, diffAnalogLStickInput);
                    }
                }
            }

            // TODO: 右アナログスティックの更新処理 



            /* TODO: R2L2の入力処理　On, Off, keepをとるようにする 
            float lt_rt = Input.GetAxis(LT_RT_Key);
            // R2ボタンの入力 
            if(lt_rt < -0.5f)
            {
            }
            else if (lt_rt > 0.5f) // L2ボタンの入力 
            {
            }
            */

            // ボタンの処理 
            int count = _eventList.Count;
            for(int i = 0; i < count; ++i)
            {
                var _event = _eventList[i];
                if(Input.GetButtonDown(ButtonNames[(int)_event.joystickButton]))
                {
                    if (_event.onDown != null)
                    {
                        _event.onDown();
                    }
                }
                if (Input.GetButton(ButtonNames[(int)_event.joystickButton]))
                {
                    if(_event.onKey != null)
                    {
                        _event.onKey();
                    }
                }
                if (Input.GetButtonUp(ButtonNames[(int)_event.joystickButton]))
                {
                    if(_event.onUp != null)
                    {
                        _event.onUp();
                    }
                }
            }

        }

        [SerializeField]
        [Tooltip("アナログスティック入力の閾値")]
        private float threshold = 0.1f;
		private readonly string analogLStickKey_X = "Horizontal";
		private readonly string analogLStickKey_Y = "Vertical";
        private readonly string LT_RT_Key = "LT_RT";


        /// <summary>
        /// 左アナログスティック入力 
        /// </summary>
        Vector2 analogLStickInput;
        /// <summary>
        /// １フレーム前の左アナログスティック入力 
        /// </summary>
        Vector2 oldAnalogLStickInput;
        /// <summary>
        /// 1フレーム前と現在の入力値の差分 
        /// </summary>
        Vector2 diffAnalogLStickInput
        {
            get { return analogLStickInput - oldAnalogLStickInput; }
        }

        /// <summary>
        /// 右アナログスティックの入力 
        /// </summary>
        Vector2 analogRStickInput;

        /// <summary>
        /// ジョイスティックのボタン入力 
        /// 各enum値のコメントはDefaultのKey 
        /// </summary>
        public enum BUTTON
        {
            None,
            L3,         // joystick button 8 
            R3,         // joystick button 9 
            A,          // joystick button 0 
            B,          // joystick button 1 
            X,          // joystick button 2 
            Y,          // joystick button 3 
            L1,         // joystick button 4 
            L2,         // 3rd Axis Positive 
            R1,         // joystick button 5 
            R2,         // 3rd Axis Negative 
            Back,       // joystick button 6 
            Start,      // joystick button 7             
            MAX,
        };

        /// <summary>
        /// ボタンの入力名 
        /// </summary>
        private readonly string[] ButtonNames = new string[]
        {
            "None",
            "L3",         // joystick button 8 
            "R3",         // joystick button 9 
            "A",          // joystick button 0 
            "B",          // joystick button 1 
            "X",          // joystick button 2 
            "Y",          // joystick button 3 
            "L1",         // joystick button 4 
            "L2",         // 3rd Axis Positive 
            "R1",         // joystick button 5 
            "R2",         // 3rd Axis Negative 
            "Back",       // joystick button 6 
            "Start",      // joystick button 7             
        };


        /// <summary>
        /// スティック入力をとるコマンド 
        /// </summary>
        List<StickEvent> _stickEventList = new List<StickEvent>();
        public List<StickEvent> stickEventList
        {
            get { return _stickEventList; }
        }

        /// <summary>
        /// ボタン入力をとるコマンド 
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
    }
}