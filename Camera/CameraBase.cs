using UnityEngine;

namespace Ashe
{
    // カメラの基底クラス
    [RequireComponent(typeof(Camera))]
    public class CameraBase : ObjectBase
    {

        // カメラコンポーネント 
        protected Camera cachedCamera;

        // カメラ座標
        Vector3 _position;
        bool isDirtyPosition;
        public Vector3 position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                isDirtyPosition = true;
            }
        }

        Quaternion _localRoation;
        bool isDirtyLocalRotation;
        public Quaternion localRotation
        {
            get
            {
                return _localRoation;
            }
            set
            {
                _localRoation = value;
                isDirtyLocalRotation = true;
            }
        }


        protected override bool Initialize(uint callCount)
        {
            cachedCamera = GetComponent<Camera>();
            return base.Initialize(callCount);
        }

        // 角度と座標の更新を行う 
        protected void UpdateTransform()
        {
            if (isDirtyPosition)
            {
                cachedTransform.position = _position;
                isDirtyPosition = false;
            }
            if (isDirtyLocalRotation)
            {
                cachedTransform.localRotation = _localRoation;
                isDirtyLocalRotation = false;
            }
        }
    }
}