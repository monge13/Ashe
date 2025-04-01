﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace Ashe
{
    /// <summary>
    /// 入力の管理システム 
    /// </summary>
    public class InputSystem : Pattern.SingletonMonoBehaviour<InputSystem>
    {
        /// <summary>
        /// コマンドを登録するリストのデフォルトのCapacity 
        /// </summary>
        public const int DEFAULT_COMMANDS_CAPACITY = 16;

        /// <summary>
        /// Keyboard入力を使うかどうか 
        /// </summary>
        [SerializeField]
        bool _useKeyboard = default;
        public bool UseKeyboard => _useKeyboard;
        /// <summary>
        /// ジョイスティック入力を使うかどうか 
        /// </summary>
        [SerializeField]
        bool _useJoyStick = default;
        public bool UseJoyStick => _useJoyStick;
        /// <summary>
        /// タッチ入力を使うかどうか 
        /// </summary>
        [SerializeField]
        bool _useTouchInput = default;
        public bool UseTouchInput => _useTouchInput;

        /// <summary>
        /// キーボードからの入力を検知して入力情報をキャッシュする 
        /// </summary>
        KeyboardInput keyboard;
        /// <summary>
        /// JoyStickからの入力を検知して入力情報をキャッシュする 
        /// </summary>
        JoyStickInput joystick;
        /// <summary>
        /// Touchの入力を検知して入力情報をキャッシュする 
        /// </summary>
        TouchInput touch;

        /// <summary>
        /// 有効な入力デバイスに対応したクラスを作成する 
        /// </summary>
        override protected void Init()
        {
            keyboard = new KeyboardInput();
            joystick = new JoyStickInput();
            touch = new TouchInput();
        }

        /// <summary>
        /// 入力処理の更新 
        /// </summary>
        void Update()
        {
#if UNITY_EDITOR
            if (keyboard == null)
            {
                keyboard = new KeyboardInput();
            }
            if (joystick == null)
            {
                joystick = new JoyStickInput();
            }
            if (touch == null)
            {
                touch = new TouchInput();
            }
#endif
            float deltaTime = Time.deltaTime;
            if (_useKeyboard)
            {
                keyboard.Update(deltaTime, EventSystem.current);
            }
            if (_useJoyStick)
            {
                joystick.Update(deltaTime);
            }
            if (_useTouchInput)
            {
                touch.Update(deltaTime, EventSystem.current);
            }
        }


        /// <summary>
        /// ジョイスティックへの入力イベントの追加 
        /// </summary>
        /// <param name="_event"></param>
        public void AddEvent(JoyStickInput.Event _event)
        {
            if (joystick != null)
            {
               joystick.AddEvent(_event);
            }
        }

        /// <summary>
        /// ジョイスティックへの入力イベントの削除
        /// </summary>
        /// <param name="_event"></param>
        public void RemoveEvent(JoyStickInput.Event _event)
        {
            if (joystick != null)
            {
                joystick.RemoveEvent(_event);
            }
        }

        /// <summary>
        /// キーボードへの入力イベントの追加 
        /// </summary>
        /// <param name="_event"></param>
        public void AddEvent(KeyboardInput.Event _event)
        {
            if (keyboard != null)
            {
                keyboard.AddEvent(_event);
            }
        }

        /// <summary>
        /// キーボードへの入力イベントの削除 
        /// </summary>
        /// <param name="_event"></param>
        public void RemoveEvent(KeyboardInput.Event _event)
        {
            if (keyboard != null)
            {
                keyboard.RemoveEvent(_event);
            }
        }

        /// <summary>
        /// マウスへの入力イベントの追加 
        /// </summary>
        /// <param name="_event"></param>
        public void AddEvent(KeyboardInput.MouseEvent _event)
        {
            if (keyboard != null)
            {
                keyboard.AddEvent(_event);
            }
        }

        /// <summary>
        /// マウスへの入力イベントの削除 
        /// </summary>
        /// <param name="_event"></param>
        public void RemoveEvent(KeyboardInput.MouseEvent _event)
        {
            if (keyboard != null)
            {
                keyboard.RemoveEvent(_event);
            }
        }        

        /// <summary>
        /// タッチへの入力イベントの追加
        /// </summary>
        /// <param name="_event"></param>
        public void AddEvent(TouchInput.Event _event)
        {
            if(touch != null)
            {
                touch.AddEvent(_event);
            }
        }

        /// <summary>
        /// タッチへの入力イベントの削除 
        /// </summary>
        /// <param name="_event"></param>
        public void RemoveEvent(TouchInput.Event _event)
        {
            if(touch != null)
            {
                touch.RemoveEvent(_event);
            }
        }
    }
}