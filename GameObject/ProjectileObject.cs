using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ashe {
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileObject : MonoBehaviour
    {
        Transform _cachedTransform;

        // 飛行スピード
        [SerializeField]
        float _speed;
        // 寿命（設定した時間が過ぎたら消滅する）
        // マイナスの値が設定された場合、寿命が存在しない
        [SerializeField]
        float _lifeTime = 5.0f;
        // 貫通できる数
        // この数だけ物体と衝突したら消滅する
        // マイナスの値の場合、無限に貫通する
        [SerializeField]
        int _penetrationNum = 1;

        Rigidbody _rigidBody;

        void Awake()
        {
            _cachedTransform = transform;
            _rigidBody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            _rigidBody.Move(
                _cachedTransform.position + _cachedTransform.forward * _speed * Time.deltaTime,
                _cachedTransform.rotation);
            
        }
    }
}
