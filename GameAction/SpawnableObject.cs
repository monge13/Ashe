using System;
using UnityEngine;

namespace Ashe {
    namespace GameAction {
        /// <summary>
        /// SpaenEventで呼び出せるオブジェクト
        /// </summary>
        public class SpawnableObject : MonoBehaviour
        {
            // Poolに返却する際のKey
            // 外部から与える必要がある
            public uint key { set; get; }

            // このオブジェクトをキャッシュする数
            [SerializeField]
            int _cacheNum = 1;
            public int cacheNum { get { return _cacheNum; } }
        }
    }
}
