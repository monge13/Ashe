using UnityEngine;

namespace Ashe {
    namespace GameAction {
        /// <summary>
        /// SpaenEventで呼び出せるオブジェクト
        /// </summary>
        public class SpawnableObject : MonoBehaviour
        {
            // このオブジェクトをキャッシュする数
            [SerializeField]
            int _cacheNum = 1;
            public int cacheNum { get { return _cacheNum; } }
        }
    }
}
