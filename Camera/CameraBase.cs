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
        // こちらを更新することで座標変更を察知して最終更新する
        Vector3 _localPosition;
        bool isDirtyPosition;
        public Vector3 localPosition
        {
            get
            {
                return _localPosition;
            }
            set
            {
                _localPosition = value;
                isDirtyPosition = true;
            }
        }

        // 回転値
        // こちらを更新することで値の変更を最終的に反映する
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

        // 初期化処理
        protected virtual void OnInitialize()
        {
        }

        void Start()
        {
            _cachedTransform = transform;
            _localPosition = _cachedTransform.localPosition;
            _localRoation = _cachedTransform.localRotation;
            cachedCamera = GetComponent<Camera>();
            OnInitialize();
        }

        // 角度と座標の更新を行う 
        protected void UpdateTransform()
        {
            if(isDirtyPosition || isDirtyLocalRotation) _cachedTransform.SetLocalPositionAndRotation(_localPosition, _localRoation);
            isDirtyPosition = false;
            isDirtyLocalRotation = false;
        }
    }
}