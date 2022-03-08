using UnityEngine;

namespace Ashe
{
    // カメラの基底クラス
    [RequireComponent(typeof(Camera))]
    public class CameraBase : MonoBehaviour
    {
        // カメラコンポーネント 
        protected Camera cachedCamera;
        protected Transform _cachedTransform;

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


        void Start()
        {
            _cachedTransform = transform;
            cachedCamera = GetComponent<Camera>();
        }

        // 角度と座標の更新を行う 
        protected void UpdateTransform()
        {
            if (isDirtyPosition)
            {
                _cachedTransform.position = _position;
                isDirtyPosition = false;
            }
            if (isDirtyLocalRotation)
            {
                _cachedTransform.localRotation = _localRoation;
                isDirtyLocalRotation = false;
            }
        }
    }
}