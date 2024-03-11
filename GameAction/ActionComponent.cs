using System.Collections;
using System.Collections.Generic;
using Ashe.Collection;
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
            // 所有しているステータス変化
            List<ActionStatusChanger> _statusChangers = new List<ActionStatusChanger>();

            [SerializeField]
            ActionStatusParameters _parameterList = new ActionStatusParameters();
            public ActionStatusParameters parameters { get { return _parameterList;} }
            // 初期化完了したかどうか
            bool _initialized;

            // Start is called before the first frame update
            void Start()
            {
                _parameterList.Initialize();
                _actionPlayer.Initialize(_target, this);
                _initialized = true;
            }

            void OnDestroy()
            {
                _actionPlayer.Dispose();
            }
        
            // Update is called once per frame
            void Update()
            {
                _actionPlayer.Execute(Time.deltaTime);
            }

            // 再生できるかどうかをチェックして再生を行う
            public bool TryPlay(ActionData data, float blendTime=0.0f)
            {
                if(!_initialized) return false;
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

            // Statusを付与する
            public void AddStatus(uint id)
            {
                if(!_statusIds.Contains(id)){
                    _statusIds.Add(id);
                }
            }

            // Statusを削除する
            public void RemoveStatus(uint id)
            {
                _statusIds.Remove(id);
            }

            // StatusChangerを積む
            public void AddStatusChanger(ActionStatusChanger changer)
            {
                _statusChangers.Add(changer);
                foreach(var id in changer.statusIds) {
                    if(!_statusIds.Contains(id)){
                        _statusIds.Add(id);
                    }
                }
                foreach(var id in changer.removeStatusIds) {
                    _statusIds.Remove(id);
                }
            }

            // StatusChangerを削除する
            public void RemoveStatusChanger(ActionStatusChanger changer)
            {
                _statusChangers.Remove(changer);
                foreach(var id in changer.statusIds) {
                    _statusIds.Remove(id);
                }
            }
        }
    }
}