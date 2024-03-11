using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// 攻撃用オブジェクト
/// 当たり判定を持つ衝突判定の後にイベントを呼び出す
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class AttackObject : MonoBehaviour
{
    // 攻撃用オブジェクトの判定のためのKey
    int _key;

    protected Transform _cachedTransform;

    void Awake()
    {
        _cachedTransform = transform;
        _key = gameObject.GetInstanceID();
    }

    public int Key
    {
        get {  
            return _key; 
        }
    }
}

