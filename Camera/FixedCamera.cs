using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ashe
{
    // 固定視点のカメラ
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
            localPosition = _followObject.position + _offset;
            UpdateTransform();
        }

    }
}