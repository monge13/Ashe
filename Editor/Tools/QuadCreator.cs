using UnityEditor;
using UnityEngine;

namespace Ashe 
{
    namespace Editor {

        public class QuadCreator : EditorWindow
        {        
            [MenuItem("Assets/Create/Ashe/Primitive/Quad", false, 0)]
            public static void ShowEditor()
            {
                var window = (QuadCreator)EditorWindow.GetWindow<QuadCreator>();
                window.Show();
            }
            
            Vector2 size;
            bool normal;
            void OnGUI()
            {
                GUILayout.Label("QuadCreator");
                size = EditorGUILayout.Vector2Field("Size", size);  
                normal = EditorGUILayout.Toggle("Normal", normal);              
                if(GUILayout.Button("Create")) CreateQuad(size.x, size.y, normal);
            }
            
            // Start is called before the first frame update
            static void CreateQuad(float w, float h, bool needNoraml)
            {
                Mesh m = new Mesh();
                m.vertices = new Vector3[] 
                {
                    new Vector3(-w * 0.5f, 0.0f, -h * 0.5f),
                    new Vector3( w * 0.5f, 0.0f, -h * 0.5f),
                    new Vector3(-w * 0.5f, 0.0f, h * 0.5f),
                    new Vector3( w * 0.5f, 0.0f, h * 0.5f),
                };
                m.uv = new Vector2[]{
                    new Vector2(0.0f, 0.0f),
                    new Vector2(1.0f, 0.0f),
                    new Vector2(0.0f, 1.0f),
                    new Vector2(1.0f, 1.0f),
                };
                if(needNoraml) {
                    m.normals = new Vector3[]{
                        new Vector3(0.0f, 1.0f, 0.0f),
                        new Vector3(0.0f, 1.0f, 0.0f),
                        new Vector3(0.0f, 1.0f, 0.0f),
                        new Vector3(0.0f, 1.0f, 0.0f),
                    };
                }

                m.triangles = new int[] {
                    0, 2, 1, 2, 3, 1,
                };

                // TODO: フォルダ指定
                AssetDatabase.CreateAsset(m, "Assets/newMesh.mesh");
            }

        }
    }
}
