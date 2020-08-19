using UnityEngine;
using UnityEngine.Profiling;
using System.Collections.Generic;

namespace Ashe
{
    /// <summary>
    /// GameObjectを独自に定義し更新順番やグルーピングによる一括操作を提供する
    /// </summary>
    public class GameObjectManager : Pattern.SingletonMonoBehaviour<GameObjectManager>
    {
        // オーダーごとのGameObjectを保持して更新や破棄を行う 
        public class GameBaseOrderControl
        {
            // オーダー単位の更新設定 
            // 設置物など表示だけのものには更新処理がいらないのでオーダー単位ではじくことが可能 
            public class Setting
            {
                // Executeを行うかどうか 
                bool _enableExecute = true;
                public bool enableExecute
                {
                    get { return _enableExecute; }
                    set { _enableExecute = value; }
                }

                // LateExecuteを行うかどうか 
                bool _enableLateExecute = true;
                public bool enableLateExecute
                {
                    get { return _enableLateExecute; }
                    set { _enableLateExecute = value; }
                }

                // DeltaTimeにかけるスケール値(スローモーションなどに使う) 
                float _deltaTimeScale = 1.0f;
                public float deltaTimeScale
                {
                    get { return _deltaTimeScale; }
                    set { _deltaTimeScale = value; }
                }
            }

            private Setting _setting = new Setting();
            public Setting setting
            {
                get { return _setting; }
            }
            static readonly int DEFAULT_CAPACITY_FOR_GAMEOBJECTS = 100;
            private List<ObjectBase> objects = new List<ObjectBase>(DEFAULT_CAPACITY_FOR_GAMEOBJECTS);

            /// <summary>
            /// ObjectBaseを登録する
            /// </summary>
            /// <param name="obj">登録したObject</param>
            public void Add(ObjectBase obj)
            {
                obj.OnRegistered();
                objects.Add(obj);
            }

            /// <summary>
            /// 削除予定のオブジェクトを削除する 
            /// </summary>
            public void Remove()
            {
                for (int i = 0; i < objects.Count;)
                {
                    if (objects[i].destroyed)
                    {
                        Destroy(objects[i]);
                        objects.RemoveAt(i);
                        continue;
                    }
                    ++i;
                }
            }

            /// <summary>
            /// オブジェクトの更新(Update)
            /// </summary>
            /// <param name="deltaTime"></param>
            public void Execute(float deltaTime)
            {
                if (!_setting.enableExecute)
                {
                    return;
                }

                float orderDeltaTime = deltaTime * _setting.deltaTimeScale;

                int count = objects.Count;
                for (int i = 0; i < count; ++i)
                {
                    if (objects[i].enabled && objects[i].gameObject.activeInHierarchy)
                    {
                        objects[i].Execute(orderDeltaTime);
                    }
                }
            }

            /// <summary>
            /// オブジェクトの更新(LateUpdate)
            /// </summary>
            /// <param name="deltaTime"></param>
            public void LateExecute(float deltaTime)
            {
                if (!_setting.enableLateExecute)
                {
                    return;
                }

                float orderDeltaTime = deltaTime * _setting.deltaTimeScale;

                int count = objects.Count;
                for (int i = 0; i < count; ++i)
                {
                    if (objects[i].enabled && objects[i].gameObject.activeInHierarchy)
                    {
                        objects[i].LateExecute(orderDeltaTime);
                    }
                }
            }
        }

        // オブジェクトオーダー 
        [SerializeField]
        ObjectOrder objectOrder;

        // CustomEditorでのみセットする 
#if UNITY_EDITOR
        public ObjectOrder ObjectOrder
        {
            get { return objectOrder; }
            set { objectOrder = value; }
        }
#endif

        /// <summary>
        /// 初期化済みかどうか 
        /// </summary>
        public bool initialized
        {
            get; set;
        }

        public const int MAX_OBJECTBASE_ORDER_NUM = 64;
        GameBaseOrderControl[] objectBaseList = new GameBaseOrderControl[MAX_OBJECTBASE_ORDER_NUM];
        // オーダー単位の設定を取得 
        public GameBaseOrderControl.Setting GetOrderSetting(int order)
        {
            return objectBaseList[order].setting;
        }

        // プレハブとオーダーを指定してObjectBaseを作成する 
        public ObjectBase Create(Object prefab, Transform parent, int order = ObjectOrder.ORDER_INVALID)
        {
            GameObject obj = (GameObject)Instantiate(prefab, parent);
            ObjectBase objBase = obj.GetComponent<ObjectBase>();
            Debug.Assert(objBase != null, string.Format("Prefab({0}) cannot have ObjectBase!!!!", prefab.name));

            if (order == ObjectOrder.ORDER_INVALID)
            {
                order = objBase.order;
            }

            objectBaseList[order].Add(objBase);
            return objBase;
        }

        // プレハブと名前を指定してObjectBaseを作成する 
        public ObjectBase Create(Object prefab, Transform parent, string orderName)
        {
            Debug.Assert(objectOrder != null);
            int order = objectOrder.NameToOrder(orderName);
            return Create(prefab, parent, order);
        }

        // 未初期化（Initializedがfalse)のオブジェクトを登録する 
        public void Add(ObjectBase objBase, int order = 0)
        {
            Debug.Assert(!objBase.Initialized, string.Format("{0} is already initialized!!!", objBase.name));
            objectBaseList[order].Add(objBase);
        }

        // 未初期化（Initializedがfalse)のオブジェクトを登録する 
        public void Add(ObjectBase objBase, string orderName)
        {
            int order = objectOrder.NameToOrder(orderName);
            objectBaseList[order].Add(objBase);
        }

        /// <summary>
        /// オブジェクトの格納スペースを確保する 
        /// </summary>
        protected override void Init()
        {
            base.Init();
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
            for (int i = 0; i < MAX_OBJECTBASE_ORDER_NUM; ++i)
            {
                objectBaseList[i] = new GameBaseOrderControl();
            }
            initialized = true;
        }


        /// <summary>
        /// Unityのライフサイクルに合わせて独自の更新をかける 
        /// </summary>
        void Update()
        {
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < MAX_OBJECTBASE_ORDER_NUM; ++i)
            {
                objectBaseList[i].Execute(deltaTime);
            }
        }

        /// <summary>
        /// Unityのライフサイクルに合わせて独自の更新をかける 
        /// </summary>
        void LateUpdate()
        {
            float deltaTime = Time.deltaTime;
            for (int i = 0; i < MAX_OBJECTBASE_ORDER_NUM; ++i)
            {
                objectBaseList[i].LateExecute(deltaTime);
            }

            for (int i = 0; i < MAX_OBJECTBASE_ORDER_NUM; ++i)
            {
                objectBaseList[i].Remove();
            }
        }
    }
}