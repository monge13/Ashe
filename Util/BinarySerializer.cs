using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ashe
{
    namespace Util
    {
        public class BinarySerializer
        {

            public static void Serialize<T>(string path, T obj)
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, obj);
                }
            }

            public static T Deserialize<T>(string path)
            {
                T obj;
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter f = new BinaryFormatter();
                    obj = (T)f.Deserialize(fs);
                }
                return obj;
            }
        }
    }
}