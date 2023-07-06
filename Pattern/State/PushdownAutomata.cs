using System.Collections.Generic;

namespace Ashe.Pattern
{
    /// <summary>
    /// プッシュダウンオートマトン
    /// ステートを切り替える際にstackとしてPush, Popできるようにしたもの 
    /// 例えば座っている行動の状態でかつ、射撃というように二つのStateを行うときにこれを使うと便利 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PushdownAutomata<T> where T : class
    {
        /// <summary>
        /// StateのStack  
        /// </summary>
        Stack<State<T>> states;

        /// <summary>
        /// StackのDefaultのcapacity
        /// </summary>
        const int DEFAULT_STACK_CAPACITY = 32;

        /// <summary>
        /// ctor 
        /// </summary>
        /// <param name="capacity"></param>
        public PushdownAutomata(int capacity = DEFAULT_STACK_CAPACITY)
        {
            states = new Stack<State<T>>(capacity);
        }

        /// <summary>
        /// StateのPush 
        /// </summary>
        /// <param name="newState"></param>
        public void Push(State<T> newState)
        {
            if (states.Count != 0)
            {
                var state = states.Peek();
                state.Exit();
            }

            newState.Enter();
            states.Push(newState);
        }

        /// <summary>
        /// StateのPop 
        /// </summary>
        public void Pop()
        {
            var state = states.Pop();
            state.Exit();

            if(states.Count == 0)
            {
                return;
            }

            var current = states.Peek();
            current.Enter();
        }

        public void ChangeState(State<T> newState)
        {
            if(states.Count != 0)
            {
                var state = states.Pop();
                state.Exit();
            }
            newState.Enter();
            states.Push(newState);
        }

        public void Execute(float deltaTime)
        {
            if( states.Count == 0)
            {
                return;
            }

            var state = states.Peek();
            state.Execute(deltaTime);
        }
    }
}
