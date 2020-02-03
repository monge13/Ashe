/// <summary>
/// 定数の定義
/// </summary>
namespace Ashe
{
    namespace Const
    {
        /// <summary>
        /// Unityの同等の物はPropertyであり速度が遅いので定数にして返す 
        /// </summary>
        public static class Vector3
        {
            public static readonly UnityEngine.Vector3 up = new UnityEngine.Vector3(0f, 1f, 0f);
            public static readonly UnityEngine.Vector3 down = new UnityEngine.Vector3(0f, -1f, 0f);
            public static readonly UnityEngine.Vector3 right = new UnityEngine.Vector3(1f, 0f, 0f);
            public static readonly UnityEngine.Vector3 left = new UnityEngine.Vector3(-1f, 0f, 0f);
            public static readonly UnityEngine.Vector3 forward = new UnityEngine.Vector3(0f, 0f, 1f);
            public static readonly UnityEngine.Vector3 back = new UnityEngine.Vector3(0f, 0f, -1f);
            public static readonly UnityEngine.Vector3 one = new UnityEngine.Vector3(1f, 1f, 1f);
            public static readonly UnityEngine.Vector3 zero = new UnityEngine.Vector3(0f, 0f, 0f);
        }

        /// <summary>
        /// Unityの同等の物はPropertyであり速度が遅いので定数にして返す 
        /// </summary>
        public static class Vector2
        {
            public static readonly UnityEngine.Vector2 one = new UnityEngine.Vector2(1f, 1f);
            public static readonly UnityEngine.Vector2 zero = new UnityEngine.Vector2(0f, 0f);
        }

        /// <summary>
        /// Mathf.EPSILONは計算機イプシロンではないので自分で定義する 
        /// </summary>
        public static class Float
        {
            public const float EPSILON = 1.192093E-07f;
        }

        /// <summary>
        /// よく使いそうなSin, Cosを定数化 
        /// </summary>
        public static class SinCos
        {
            public const float Sin0 = 0f;
            public const float Sin15 = 0.342f;
            public const float Sin30 = 0.5f;
            public const float Sin45 = 0.7071f;
            public const float Sin60 = 0.866f;
            public const float Sin75 = 0.9659f;
            public const float Sin90 = 1f;
            public const float Sin105 = 0.9659f;
            public const float Sin120 = 0.866f;
            public const float Sin135 = 0.7071f;
            public const float Sin150 = 0.5f;
            public const float Sin160 = 0.342f;
            public const float Sin180 = 0f;

            public const float Cos0 = 1f;
            public const float Cos15 = 0.9659f;
            public const float Cos30 = 0.866f;
            public const float Cos45 = 0.7071f;
            public const float Cos60 = 0.5f;
            public const float Cos75 = 0.2588f;
            public const float Cos90 = 0f;
            public const float Cos105 = 0.2588f;
            public const float Cos120 = 0.5f;
            public const float Cos135 = 0.7071f;
            public const float Cos150 = 0.866f;
            public const float Cos165 = 0.9659f;
            public const float Cos180 = 1f;
        }
    }
}