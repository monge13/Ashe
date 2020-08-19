namespace Ashe.Pattern
{
    /// <summary>
    /// 
    /// </summary>
    public class State<T> where T : class
    {
        /// <summary>
        /// Stateの保持者 
        /// </summary>
        protected T owner;

        /// <summary>
        /// State名 
        /// </summary>
        public string name
        {
            get; private set;
        }

        /// <summary>
        /// Steta名のGetHashCode 
        /// </summary>
        public uint hash
        {
            get; private set;
        }

        /// <summary>
        /// コンストラクタ 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        public State(T owner, string name)
        {
            this.owner = owner;
            this.hash = (uint)name.GetHashCode();

#if UNITY_EDITOR
            this.name = name;
#endif
        }

        /// <summary>
        /// Stateの初期化処理 
        /// </summary>
        public virtual void Enter() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void Execute(float deltaTime) { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Exit() { }
    }
}
