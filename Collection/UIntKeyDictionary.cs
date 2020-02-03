using UnityEngine;

namespace Ashe
{
    namespace Collection
    {
        /// <summary>
        /// IL2CPPになった際にコードサイズが大きくなってしまうので必要な機能だけ持ったUintをKeyとするDictionary 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class UIntKeyDictionary<T> where T : class
        {
            private struct Entry
            {
                public int next;
                public uint key;
                public T value;
            }

            private int[] buckets;    // 
            private Entry[] entries;    // 
            private int freeList;       // 
            private int count;          // 
            private int freeCount;      // 

            public UIntKeyDictionary(int capacity = 0)
            {
                Initalize(capacity);
            }

            // bucketsとentriesをcapacity分作る 
            private void Initalize(int capacity)
            {
                if (capacity <= 0)
                {
                    capacity = 1;
                }

                buckets = new int[capacity];
                for (int i = 0; i < capacity; ++i)
                {
                    buckets[i] = -1;
                }
                entries = new Entry[capacity];
                freeList = -1;
            }

            // 現在ディクショナリに詰まっている数を返す 
            public int Count
            {
                get { return count - freeCount; }
            }

            // キーを含んでいるかどうか 
            public bool ContainsKey(uint key)
            {
                return FindEntry(key) >= 0;
            }

            // 中身の削除 
            public void Clear()
            {
                if (count > 0)
                {
                    for (int i = 0; i < buckets.Length; ++i)
                    {
                        buckets[i] = -1;
                    }
                    System.Array.Clear(entries, 0, count);
                    freeList = -1;
                    count = 0;
                    freeCount = 0;
                }
            }

            // 削除 
            public bool Remove(uint key)
            {
                uint bucket = (uint)(key % buckets.Length);
                int last = -1;
                for (int i = buckets[bucket]; i >= 0; last = i, i = entries[i].next)
                {
                    if (entries[i].key == key)
                    {
                        if (last < 0)
                        {
                            buckets[bucket] = entries[i].next;
                        }
                        else
                        {
                            entries[last].next = entries[i].next;
                        }

                        entries[i].next = freeList;
                        entries[i].key = 0;
                        entries[i].value = null;
                        freeList = i;
                        ++freeCount;
                        return true;
                    }
                }
                return false;
            }

            public T this[uint key]
            {
                get
                {
                    int i = FindEntry(key);
                    if (i >= 0)
                    {
                        return entries[i].value;
                    }
                    Debug.Assert(i >= 0, "Key is not included : " + key);
                    return null;
                }
                set
                {
                    Insert(key, value, false);
                }
            }

            public void Add(uint key, T value)
            {
                Insert(key, value, true);
            }

            public bool TryGetValue(uint key, out T value)
            {
                int i = FindEntry(key);
                if (i >= 0)
                {
                    value = entries[i].value;
                    return true;
                }
                value = null;
                return false;
            }

            // keyに対応したEntryのインデックスを返す 
            private int FindEntry(uint key)
            {
                if (buckets != null)
                {
                    for (int i = buckets[key % buckets.Length]; i >= 0; i = entries[i].next)
                    {
                        if (entries[i].key == key)
                        {
                            return i;
                        }
                    }

                }
                return -1;
            }

            // key, valueをentriesに挿入する 
            private void Insert(uint key, T value, bool add)
            {
                if (buckets == null)
                {
                    Initalize(1);
                }

                uint targetBucket = (uint)(key % buckets.Length);

                // すでにkeyを持っている場合の処理 
                for (int i = buckets[targetBucket]; i >= 0; i = entries[i].next)
                {
                    if (entries[i].key == key)
                    {
                        Debug.Assert(!add, "Already contains key : " + key);
                        entries[i].value = value;
                        return;
                    }
                }

                // keyを持っていない場合keyを登録してvalueをセットする 
                int index;
                // 空きがある場合　
                if (freeCount > 0)
                {
                    index = freeList;
                    freeList = entries[index].next;
                    --freeCount;
                }
                else
                {
                    // 空きがない場合はリサイズをして元のサイズをインデックスとする 
                    if (count == entries.Length)
                    {
                        Resize();
                        targetBucket = (uint)(key % buckets.Length);
                    }
                    index = count;
                    ++count;
                }

                entries[index].key = key;
                entries[index].next = buckets[targetBucket];
                entries[index].value = value;
                buckets[targetBucket] = index;
            }

            private void Resize()
            {
                int newSize = entries.Length * 2;
                int[] newBuckets = new int[newSize];
                for (int i = 0; i < newBuckets.Length; ++i)
                {
                    newBuckets[i] = -1;
                }

                Entry[] newEntries = new Entry[newSize];
                System.Array.Copy(entries, 0, newEntries, 0, count);

                for (int i = 0; i < count; ++i)
                {
                    uint bucket = (uint)(newEntries[i].key % newSize);
                    newEntries[i].next = newBuckets[bucket];
                    newBuckets[bucket] = i;
                }
                buckets = newBuckets;
                entries = newEntries;
            }
        }
    }
}