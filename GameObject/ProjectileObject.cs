using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileObject : MonoBehaviour
{
    Transform _cachedTransform;

    // 飛行スピード
    [SerializeField]
    float _speed;

    void Awake()
    {
        _cachedTransform = transform;
    }

    void Update()
    {
        _cachedTransform.position += _cachedTransform.forward * _speed * Time.deltaTime;
    }
}
