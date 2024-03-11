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
        protected float _speed;
        // 寿命（設定した時間が過ぎたら消滅する）
        // マイナスの値が設定された場合、寿命が存在しない
        [SerializeField]
        protected float _lifeTime = 5.0f;
        // 貫通できる数
        // この数だけ物体と衝突したら消滅する
        // マイナスの値の場合、無限に貫通する
        [SerializeField]
        protected int _penetrationNum = 1;

        // 生存時間
        float _timer;

        Rigidbody _rigidBody;

        void Awake()
        {
            _cachedTransform = transform;
            _rigidBody = GetComponent<Rigidbody>();
            Initialize();
        }

        // タイマーを初期化する
        public void Initialize()
        {
            _timer = 0.0f;
            OnInitialize();
        }

        // 継承して特定の初期化処理を追加する場合に使用する
        protected virtual void OnInitialize()
        {            
        }

        void Update()
        {
            _timer += Time.deltaTime;
            if(_timer >= _lifeTime) {
                OnDestroyByTime();
            }

            _rigidBody.Move(
                _cachedTransform.position + _cachedTransform.forward * _speed * Time.deltaTime,
                _cachedTransform.rotation);
            
        }

        // 時間によって消滅するときに呼ぶ
        public virtual void OnDestroyByTime()
        {

        }

    }
}
