using Ashe.Collection;

namespace Ashe.Pattern
{
    /// <summary>
    /// Stateの実行、保持、切り替えを行う 
    /// </summary> 
    public class StateController<T> where T : class
    {
        /// <summary>
        /// 内部的に保持しているステート 
        /// </summary>
        UIntKeyDictionary<State<T>> states = new UIntKeyDictionary<State<T>>(10);

        /// <summary>
        /// 現在実行中のState 
        /// </summary>
        State<T> currentState;

        /// <summary>
        /// Stateの登録 
        /// </summary>
        /// <param name="state"></param>
        public void AddState(State<T> state)
        {
            states.Add((uint)state.hash, state);
        }


        /// <summary>
        /// Stateの更新 
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            if (currentState != null)
            {
                currentState.Execute(deltaTime);
            }
        }

        /// <summary>
        /// Stateの変更 
        /// </summary>
        /// <param name="newState">新しいState</param>
        public void ChangeState(string name)
        {
            ChangeState((uint)name.GetHashCode());
        }


        /// <summary>
        /// Stateの変更 
        /// </summary>
        /// <param name="hash">新しいState名のGetHashCode</param>
        public void ChangeState(uint hash)
        {
            if(currentState != null)
            {
                currentState.Exit();
            }

            State<T> newState;
            if(states.TryGetValue((uint)hash, out newState))
            {
                newState.Enter();
            }
            currentState = newState;
        }

        /// <summary>
        /// 指定した名前のStateが現在のStateかどうか
        /// </summary>
        /// <param name="name">State名</param>
        /// <returns>nameで指定したStateが現在のStateかどうか</returns>
        public bool IsCurrentState(string name)
        {
            return currentState.name == name;
        }

        /// <summary>
        /// 現在実行中のStateを返す
        /// </summary>
        /// <returns>現在実行中のState</returns>
        public State<T> GetCurrentState()
        {
            return currentState;
        }
    }
}
