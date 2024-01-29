using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ashe 
{
    /// <summary>
    /// 肩越し視点のカメラ
    /// </summary>
    public class OverShoulderCamera : CameraBase
    {
        // 追従点Transform 
        [SerializeField]
        Transform _followObject;
        // Positionオフセット
        [SerializeField]
        Vector3 _offset;
        // ターゲットからの距離 
        [SerializeField]
        float _distance = 5.0f;

        Vector2 _angle;
        // Y軸回転を加える
        public void AddRotationY(float degree)
        {
            _angle.y += degree;
        }
        // 任意軸回転を加える 
        public void AddRotation(Vector2 addAngle)
        {
            _angle.x += addAngle.x;
            _angle.y += addAngle.y;
        }

        // カメラの更新 
        void LateUpdate()
        {
            UpdateLookAtAndPostion();
            UpdateTransform();
        }

        // 注視点と座標を更新する 
        private void UpdateLookAtAndPostion()
        {
            Quaternion x = Quaternion.AngleAxis(_angle.x, Const.Vector3.right);
            Quaternion y = Quaternion.AngleAxis(_angle.y, Const.Vector3.up);            
            Vector3 forward = x * y * Vector3.forward;

            _cachedTransform.SetPositionAndRotation(
                _followObject.position + y * _offset - forward * _distance,
                Quaternion.LookRotation(forward, Vector3.up)
            );
        } 
    }
}
