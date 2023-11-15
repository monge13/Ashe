using System.Collections.Specialized;
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

        // カメラの向いている方向ベクトル
        public Vector3 forward {
            get { return _cachedTransform.forward; }
        }
        // カメラの右方向ベクトル
        public Vector3 right {
            get { return _cachedTransform.right; }
        }

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
            if(isDirtyPosition || isDirtyLocalRotation) _cachedTransform.SetLocalPositionAndRotation(_position, _localRoation);
            isDirtyPosition = false;
            isDirtyLocalRotation = false;
        }
    }
}