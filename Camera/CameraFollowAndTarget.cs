using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ashe
{
    namespace Camera 
    {
        /// <summary>
        /// Cameraの注視点とフォロー点の制御を行う
        /// </summary>
        public class CameraFollowAndTarget : MonoBehaviour
        {
            [HideInInspector]
            Transform _cachedTransform;
            public Transform cachedTransform { get { return _cachedTransform; } }

            /// <summary>
            /// 注視点用のTransform
            /// </summary>
            [SerializeField]
            Transform _lookAt;

            /// <summary>
            /// 回転感度
            /// </summary>
            [SerializeField]
            Vector2 _sensitivity = new Vector2(1.0f, 1.0f);
            public Vector2 sensitivity { get { return _sensitivity; } set { _sensitivity = value; } }

            void Awake()
            {
                _cachedTransform = transform;
            }

            public void UpdateRotation(Vector2 inputVector)
            {
                Quaternion x = Quaternion.AngleAxis(inputVector.x * _sensitivity.x * Time.deltaTime, cachedTransform.up);
                Quaternion z = Quaternion.AngleAxis(inputVector.y * _sensitivity.y * Time.deltaTime, cachedTransform.right);
                _cachedTransform.localRotation = _cachedTransform.localRotation * x * z;
            }

        }
    }
}