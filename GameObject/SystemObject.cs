using UnityEngine;

namespace Ashe
{
    /// <summary>
    /// Singletonとほぼ同意だが正しいシングルトンではないので別の名前にする
    /// Systemを管理するクラスはこれを継承することによりアクセスしやすくさせ
    /// さらにシステムオーダーでの更新を初期値として行う 
    /// </summary>
    /// <typeparam name="T">継承先オブジェクトの型</typeparam>
    public class SystemObject<T> : ObjectBase where T : class
    {
        private static T instance;
        public static T I
        {
            get
            {
                Debug.Assert(instance != null, "System is null");
                return instance;
            }
        }


        /// <summary>
        /// オーダーをシステムに変更する 
        /// </summary>
        /// <param name="callCount"></param>
        /// <returns></returns>
        protected override bool Initialize(uint callCount)
        {
            instance = this as T;
            Debug.Assert(_order == ObjectOrder.ORDER_SYSTEM, "SystemObject's order is not System");
            _order = ObjectOrder.ORDER_SYSTEM;               
            return base.Initialize(callCount);
        }
    }
}