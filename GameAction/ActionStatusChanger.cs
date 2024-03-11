using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ashe
{
    namespace GameAction
    {
        /// <summary>
        /// パラメーターを変化させステータス名の付与を行う
        /// バフをかける際などは自身のActionClipで生成する
        /// ヒットの際はヒットのイベントで生成する
        /// 特殊条件がある場合は継承して各関数をoverrideする
        /// </summary>
        [Serializable]
        public class ActionStatusChanger
        {
            // 計算式の種類
            public enum CALC_TYPE
            {
                None,
                Add,
                Sub,
                Multi,              // 現在値に乗算する
                Div,                // 現在値に除算する
                Multi_Base,         // ベース値に乗算する
                Div_Base,           // ベース値に除算する
            }

            // 状態変化の持続時間
            public enum DURATION_TYPE
            {
                Instant,    // 即時発動1回のみ（ダメージとか）
                Duration,   // 持続時間があるもの
                Infinite,   // 常時継続するもの（削除できる）
                LimitedNum, // 回数による発動の場合
            }

            public ActionStatusChanger(
                List<uint> statusIdList = null,
                List<uint> RemoveStatusList = null,
                string targetParameter = null,
                float value = 0.0f,
                CALC_TYPE calcType = CALC_TYPE.None,
                DURATION_TYPE durationType = DURATION_TYPE.Instant
            )
            {
                _statusIds = statusIdList;
                _removeStatusIds = removeStatusIds;
                _targetParameter = targetParameter;
                _value = value;
                _calcType = calcType;
                _durationType = durationType;
            }

            // パラメーター変化先
            protected ActionComponent _targetComponent;

            // 付与させるステータス名
            [HideInInspector]
            [SerializeField]
            protected List<uint> _statusIds;
            public List<uint> statusIds { get { return _statusIds; } }

            // 削除するステータス名のリスト
            [HideInInspector]
            [SerializeField]
            protected List<uint> _removeStatusIds;
            public List<uint> removeStatusIds { get { return _removeStatusIds; } }
            
            // 変更対象となるパラメーター
            [SerializeField]
            protected string _targetParameter;
            
            // 変更に使う値
            [SerializeField]
            protected float _value;
            // valueをどのように計算するか
            [SerializeField]
            protected CALC_TYPE _calcType = CALC_TYPE.None;

            // パラメーター適応周期(0.0fのときは判定に使用しない)
            [SerializeField]
            float _period = 0.0f;
            
            // ステータス変化を時間的にどう扱うか
            [SerializeField]
            protected DURATION_TYPE _durationType = DURATION_TYPE.Instant;
            public DURATION_TYPE durationType { get { return _durationType; } }
            // ステータス変化の時間（回数設定にも使う）
            [SerializeField]
            protected float _duration = 0.0f;            

            // Durationの場合のタイマー
            protected float _timer;
            // 変化させるパラメーター
            protected ActionStatusParameter targetParam;
            
            // パラメーター適用回数
            int _applyNum;

            // パラメーターの変更を適応する
            // 戻り値で追加するステータスのIDを指定する
            public virtual void Apply(ActionComponent actionComponent)
            {
                _targetComponent = actionComponent;
                _targetComponent.AddStatusChanger(this);

                _timer = 0.0f;
                _applyNum = 0;

                // parameterのキャッシュ
                var actionStatusParameters = actionComponent.parameters;
                targetParam = actionStatusParameters.GetParam(_targetParameter);
                ApplyParameter();
            }

            // パラメーター反映
            public void ApplyParameter()
            {
                switch(_calcType)
                {
                    case CALC_TYPE.Add: 
                        targetParam.currentValue += _value;
                        break;

                    case CALC_TYPE.Sub:
                        targetParam.currentValue -= _value;
                        break;

                    case CALC_TYPE.Multi:
                        targetParam.currentValue *= _value;
                        break;

                    case CALC_TYPE.Div:
                        targetParam.currentValue /= _value;
                        break;

                    case CALC_TYPE.Multi_Base:
                        targetParam.currentValue = targetParam.baseValue * _value;
                        break;

                    case CALC_TYPE.Div_Base:
                        targetParam.currentValue = targetParam.baseValue / _value;
                        break;                                                
                }
                ++_applyNum;
            }

            public virtual void Update(float deltaTime)
            {
                _timer += deltaTime;

                // 周期のあるパラメーター処理
                if(_period > Const.Float.EPSILON){
                    int num = (int)(_timer / _period);
                    while(_applyNum < num){
                        ApplyParameter();
                    }
                }

                // 持続時間のあるタイプの場合は
                switch(_durationType){
                    case DURATION_TYPE.Duration:
                        if(_timer >= _duration) _targetComponent.RemoveStatusChanger(this);
                        break;
                
                    case DURATION_TYPE.LimitedNum:
                        if(_applyNum > (int)_duration) _targetComponent.RemoveStatusChanger(this);
                        break;
                }
            }            
        }
    }
}