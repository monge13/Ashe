using UnityEngine;

namespace Ashe
{
    // 衛星カメラ
    // targetに対して追従し一定の距離をもって旋回する 
    public class OrbitalCamera : CameraBase
    {
        // 注視点となるTransform 
        [SerializeField]
        Transform _target = default;
        // _targetから_lookAtOffsetを足した分が注視点となる
        [SerializeField]
        Vector3 _lookAtOffset = default;

        // ターゲットからの距離 
        [SerializeField]
        float _distance = 5.0f;

        // 回転を合成する順番 
        public enum RotationOrder
        {
            XYZ,
            YXZ,
        }
        [SerializeField]
        RotationOrder _rotationOrder = RotationOrder.YXZ;

        // ターゲットを中心に回っている角度 
        [SerializeField]
        Vector3 _angle;
        public Vector3 angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        // Y軸回転を加える
        public void AddRotationY(float degree)
        {
            _angle.y += degree;
        }
        // 任意軸回転を加える 
        public void AddRotation(Vector3 addAngle)
        {
            _angle.x += addAngle.x;
            _angle.y += addAngle.y;
            _angle.z += addAngle.z;
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
            Vector3 lookAt = _target.position + _lookAtOffset;

            Quaternion x = Quaternion.AngleAxis(_angle.x, Const.Vector3.right);
            Quaternion y = Quaternion.AngleAxis(_angle.y, Const.Vector3.up);
            Quaternion z = Quaternion.AngleAxis(_angle.z, Const.Vector3.forward);

            Quaternion current = Quaternion.identity;
            switch (_rotationOrder)
            {
                case RotationOrder.XYZ:
                    current = x * y * z;
                    break;

                case RotationOrder.YXZ:
                    current = y * x * z;
                    break;
            }
            localPosition = lookAt - current * Vector3.forward * _distance;
            localRotation = current;
        }

        // CustomInspectorから呼び出す用の更新処理 
#if UNITY_EDITOR
    public void UpadateInEditorFromInspector()
        {
            UpdateLookAtAndPostion();
            transform.position = localPosition;
            transform.localRotation = localRotation;
        }
#endif
}
}
