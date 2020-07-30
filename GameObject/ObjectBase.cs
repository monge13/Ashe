using UnityEngine;

namespace Ashe
{
    /// <summary>
    /// 全てのオブジェクトが継承するオブジェクトクラス
    /// GameObjectManagerに登録されそこから更新処理が呼ばれる
    /// Initialize -> OnExecute -> OnLateExecuteの順番で呼ばれる
    /// Initialize処理は初期化処理を書いていく、trueを返すまではExecuteが呼ばれず
    /// initializeCallCountが渡されるので内部でcallCountごとに処理を分散させることで初期化処理の負荷分散を行うことができる  
    /// <summary>
    public class ObjectBase : MonoBehaviour
    {
        public Transform cachedTransform;
        /// <summary>
        /// OnRegisteredが呼ばれたかどうか
        /// </summary>
        public bool Registered
        {
            get;
            private set;
        }

        /// <summary>
        /// 初期化が終了したかどうか
        /// </summary>
        private bool initialized;
        public bool Initialized
        {
            get { return initialized; }
        }

        /// <summary>
        /// 削除予定かどうか
        /// </summary>
        public bool destroyed
        {
            get;
            private set;
        }

        /// <summary>
        /// 更新の優先順位　
        /// </summary>
        [SerializeField]
        protected int _order = ObjectOrder.ORDER_DEFAULT;
        public virtual int order
        {
            get
            {
                return _order;
            }
        }

        /// <summary>
        /// Initializeを呼び出した回数
        /// </summary>
        private uint initializeCallCount;

        /// <summary>
        /// Startでは
        /// </summary>
        void Start()
        {
            // 初期化されていなければシーンに置かれていたものなのでGameObjectManagerに登録する 
            if (!Registered)
            {
                GameObjectManager.I.Add(this, order);
            }
        }

        /// <summary>
        /// GameObjectManagerに登録された時点で呼び出される 
        /// </summary>
        public void OnRegistered()
        {
            cachedTransform = transform;
            Registered = true;
        }

        /// <summary>
        /// 継承先で初期化処理を書いてください
        /// Awakeの代わりになるものですがtrueを返すまで呼ばれ続けます
        /// 呼ばれるたびにcallCountが1増えて渡されるので任意のフレームに処理を分割できます。
        /// trueを返したあとからExecute, LateExecuteが呼ばれ始めます。それまでは更新処理は呼ばれないです。
        /// </summary>
        /// <param name="callCount">初期化処理が呼ばれた回数</param>
        /// <returns>trueを返すと初期化処理終了</returns>
        protected virtual bool Initialize(uint callCount)
        {
            return true;
        }

        /// <summary>
        /// GameObjectManagerから呼び出される更新処理 
        /// </summary>
        /// <param name="deltaTime">Time.deltaTime * deltaTimeScale</param>
        public void Execute(float deltaTime)
        {
            if (!initialized)
            {
                initialized = Initialize(initializeCallCount++);
                return;
            }
            OnExecute(deltaTime);
        }

        /// <summary>
        /// 更新処理 Updateの代わりになるものです。
        /// Time.deltaTimeを個別に取得するのではなく渡された引数を使用するようにしてください。
        /// 呼び出し元でdeltaTimeにscaleをかけることでスローモーションなどに対応します。
        /// </summary>
        /// <param name="deltaTime">Time.deltaTime * deltaTimeScale</param>
        protected virtual void OnExecute(float deltaTime)
        {
        }

        /// <summary>
        /// GameObjectManagerから呼び出される更新処理 
        /// </summary>
        /// <param name="deltaTime">Time.deltaTime * deltaTimeScale</param>
        public void LateExecute(float deltaTime)
        {
            if (!initialized)
            {
                return;
            }
            OnLateExecute(deltaTime);
        }

        /// <summary>
        /// 更新処理 LateUpdateの代わりになるものです。
        /// Time.deltaTimeを個別に取得するのではなく渡された引数を使用するようにしてください。
        /// 呼び出し元でdeltaTimeにscaleをかけることでスローモーションなどに対応します。
        /// </summary>
        /// <param name="deltaTime">Time.deltaTime * deltaTimeScale</param>
        protected virtual void OnLateExecute(float deltaTime)
        {
        }
    }
}