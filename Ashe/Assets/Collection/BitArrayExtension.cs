using System.Collections;

/// <summary>
/// BitArrayの拡張メソッド 
/// </summary>
public static class BitArrayExtension
{
    /// <summary>
    /// どれか一つのフラグが立っているかどうかを調べる 
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static bool Any(this BitArray array)
    {
        for(int i = 0; i < array.Length; ++i)
        {
            if (array.Get(i))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// すべてのフラグが立っているかどうかを調べる 
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static bool All(this BitArray array)
    {
        for (int i = 0; i < array.Length; ++i)
        {
            if (!array.Get(i))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// どのフラグもたっていないかどうかを調べる 
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static bool None(this BitArray array)
    {
        return !Any(array);
    }

    /// <summary>
    /// 特定のフラグを反転させる 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public static void Flip(this BitArray array, int index)
    {
        array.Set(index, !array.Get(index));
    }
}