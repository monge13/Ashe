using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ashe
{
    public class FixedCamera : CameraBase
    {
        // 追従する対象のTransform
        [SerializeField]
        Transform _followObject;
        // 追従対象からのオフセット
        [SerializeField]
        Vector3 _offset;
 
                // カメラの更新 
        void LateUpdate()
        {
            _cachedTransform.SetPositionAndRotation(
                _followObject.position + _offset,
                _cachedTransform.rotation
            );
            UpdateTransform();
        }

    }
}