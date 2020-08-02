using UnityEngine;
using System.Collections.Generic;

namespace Ashe
{
    /// <summary>
    /// 入力の管理システム 
    /// </summary>
    public class InputSystem : SystemObject<InputSystem>
    {
        /// <summary>
        /// コマンドを登録するリストのデフォルトのCapacity 
        /// </summary>
        public const int DEFAULT_COMMANDS_CAPACITY = 16;

        /// <summary>
        /// Keyboard入力を使うかどうか 
        /// </summary>
        [SerializeField]
        bool useKeyboard;
        /// <summary>
        /// ジョイスティック入力を使うかどうか 
        /// </summary>
        [SerializeField]
        bool useJoyStick;
        /// <summary>
        /// タッチ入力を使うかどうか 
        /// </summary>
        [SerializeField]
        bool useTouchInput;

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
        /// <param name="callCount"></param>
        /// <returns></returns>
        protected override bool Initialize(uint callCount)
        {
            _order = ObjectOrder.ORDER_SYSTEM;
            if (useKeyboard)
            {
                keyboard = new KeyboardInput();
            }
            if (useJoyStick)
            {
                joystick = new JoyStickInput();
            }
            if (useTouchInput)
            {
                touch = new TouchInput();
            }

            return base.Initialize(callCount);
        }

        /// <summary>
        /// 入力処理の更新 
        /// </summary>
        /// <param name="deltaTime"></param>
        protected override void OnExecute(float deltaTime)
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
            if (useKeyboard)
            {
                keyboard.Update(deltaTime);
            }
            if (useJoyStick)
            {
                joystick.Update(deltaTime);
            }
            if (useTouchInput)
            {
                //touch.Update(deltaTime);
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
               joystick.eventList.Add(_event);
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
                joystick.eventList.Remove(_event);
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
                keyboard.eventList.Add(_event);
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
                keyboard.eventList.Remove(_event);
            }
        }
    }
}