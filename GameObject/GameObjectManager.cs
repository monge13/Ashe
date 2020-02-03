using UnityEngine;
using UnityEngine.Profiling;
using System.Collections.Generic;

namespace Ashe
{
    public class GameObjectManager : MonoBehaviour
    {
        private static GameObjectManager instance;
        public static GameObjectManager I
        {
            get
            {
                Debug.Assert(instance != null, "GameObjectManager is Null!!!!!");
                return instance;
            }
        }

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

            private List<ObjectBase> objects = new List<ObjectBase>(100);

            // ObjectBaseを登録する
            public void Add(ObjectBase obj)
            {
                obj.OnRegistered();
                objects.Add(obj);
            }

            // 削除予定のオブジェクトを削除する 
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

            // オブジェクトの更新(Update)
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

            // オブジェクトの更新(LateUpdate)
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
        /// 消されないようにしインスタンスを取得する 
        /// </summary>
        void Awake()
        {
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
            instance = this;
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