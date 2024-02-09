using System.Collections;
using System.Collections.Generic;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer.Operations;
using UnityEngine;

namespace Ashe
{
    namespace GameAction
    {
        /// <summary>
        /// Game Actionの再生、ステータス変化、ステータス管理を行う
        /// </summary>
        public class ActionComponent : MonoBehaviour
        {            
            // 再生対象のAnimator
            [SerializeField]
            Animator _target;
            // アクションの再生クラス
            ActionPlayer _actionPlayer = new ActionPlayer();
            // 現在、付与されているステータス
            List<uint> _statusIds = new List<uint>();

            // Start is called before the first frame update
            void Start()
            {
                _actionPlayer.Initialize(_target);
            }

            // Update is called once per frame
            void Update()
            {
                _actionPlayer.Execute(Time.deltaTime);
            }

            // 再生できるかどうかをチェックして再生を行う
            public bool TryPlay(ActionData data, float blendTime=0.0f)
            {
                if(!checkStatusForPlayingAction(data)) return false;
                _actionPlayer.Play(data, blendTime);
                return true;
            }
            
            // 指定したアクションが再生できるかどうかチェックする
            bool checkStatusForPlayingAction(ActionData data)
            {
                foreach(var id in _statusIds){
                    if(data.blockStatusList.Contains(id)) return false;
                }
                return true;
            }
        }
    }
}