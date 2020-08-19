using System.Collections.Generic;

namespace Ashe.Pattern
{

    /// <summary>
    /// コマンドのインタフェース 
    /// 例えば下記のようにして使う
    /// public class Command : ICommand<Player>
    /// {
    ///     Parameter param;
    ///     
    ///     public Command(Parameter _param)
    ///     {
    ///         param = _param;
    ///     }
    ///     
    ///     public void Execute(Player receiver, float deltaTime)
    ///     {
    ///     
    ///     }
    /// }
    ///     
    /// </summary>
    public interface ICommand<T>
    {
        void Execute(T receiver, float deltaTime);
    }

    /// <summary>
    /// コマンドを蓄積して実行する  
    /// </summary>
    public class CommandStream<T>
    {
        /// <summary>
        /// コマンドを受け取る相手 
        /// </summary>
        T receiver;
        /// <summary>
        /// コマンドキュー 
        /// </summary>
        Queue<ICommand<T>> commandQueue;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="capacity">コマンドキューのキャパシティ</param>
        public CommandStream(T receiver, int capacity = 8)
        {
            this.receiver = receiver;
            commandQueue = new Queue<ICommand<T>>(capacity);
        }

        /// <summary>
        /// コマンドを追加する 
        /// </summary>
        /// <param name="command">追加するコマンド</param>
        public void Add(ICommand<T> command)
        {
            commandQueue.Enqueue(command);
        }

        /// <summary>
        /// コマンドを複数追加する
        /// </summary>
        /// <param name="commands">追加するコマンド</param>
        public void Add(ICommand<T>[] commands)
        {
            for (int i = 0; i < commands.Length; ++i)
            {
                commandQueue.Enqueue(commands[i]);
            }
        }


        /// <summary>
        /// コマンドをすべて実行するカウント数  
        /// </summary>
        public const int EXECUTE_ALL_COMMANDS = -1;

        /// <summary>
        /// 個数を指定してコマンドを実行する 
        /// </summary>
        /// <param name="count">実行するコマンド数 EXECUTE_ALL_COMMANDSが指定されたらすべて</param>
        public void Execute(float deltaTime, int count = EXECUTE_ALL_COMMANDS)
        {
            if (count == EXECUTE_ALL_COMMANDS || count >= commandQueue.Count)
            {
                count = commandQueue.Count;
            }

            for (int i = 0; i < count; ++i)
            {
                commandQueue.Dequeue().Execute(receiver, deltaTime);
            }
        }
    }
}