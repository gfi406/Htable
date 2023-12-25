using System;
using System.Collections;
using System.Collections.Generic;

public class KeyValue<K, V>
{
    public K Key { get; }
    public V Value { get; }

    public KeyValue(K key, V value)
    {
        Key = key;
        Value = value;
    }

    public override int GetHashCode()
    {
        return CombineHashCodes(Key.GetHashCode(), Value.GetHashCode());
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        KeyValue<K, V> keyValue = (KeyValue<K, V>)obj;
        return EqualityComparer<K>.Default.Equals(Key, keyValue.Key) &&
               EqualityComparer<V>.Default.Equals(Value, keyValue.Value);
    }

    public override string ToString()
    {
        return $"{Key} -> {Value}";
    }

    private int CombineHashCodes(int h1, int h2)
    {
        return ((h1 << 5) + h1) ^ h2;
    }
}
public class HashTable<K, V> : IEnumerable<KeyValue<K, V>>
{
    private LinkedList<KeyValue<K, V>>[] slots;
    private const int InitialCapacity = 16;
    private const double LoadFactor = 0.80d;
    private int count;

    public HashTable()
    {
        this.slots = new LinkedList<KeyValue<K, V>>[InitialCapacity];
        this.count = 0;
    }

    private int GetSlotNumber(K key)
    {
        int hash = key.GetHashCode();
        return Math.Abs(hash % slots.Length);
    }

    private void GrowIfNeeded()
    {
        double loadFactor = (double)count / slots.Length;
        if (loadFactor > LoadFactor)
        {
            Grow();
        }
    }

    private void Grow()
    {
        var tempSlots = slots;
        slots = new LinkedList<KeyValue<K, V>>[slots.Length * 2];
        count = 0;

        foreach (var slot in tempSlots)
        {
            if (slot != null)
            {
                foreach (var keyValue in slot)
                {
                    Add(keyValue.Key, keyValue.Value);
                }
            }
        }
    }
    public int ShowCollisions()
    {
        int count = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].Count > 1)
            {
                count++;
                //Console.WriteLine($"Collision at slot {i}:");
                //foreach (var keyValue in slots[i])
                //{
                //    Console.WriteLine($"Key: {keyValue.Key}, Value: {keyValue.Value}");
                //}
            }
        }
        return count;
    }

    public void Add(K key, V value)
    {
        GrowIfNeeded();
        int slotNumber = GetSlotNumber(key);

        if (slots[slotNumber] == null)
        {
            slots[slotNumber] = new LinkedList<KeyValue<K, V>>();
        }

        var slot = slots[slotNumber];

        foreach (var keyValue in slot)
        {
            if (keyValue.Key.Equals(key))
            {
                //throw new ArgumentException("Элемент с таким ключом уже существует в хеш-таблице.");
                Console.WriteLine("Элемент с таким ключом уже существует в хеш-таблице.");
            }
        }

        var newPair = new KeyValue<K, V>(key, value);
        slot.AddLast(newPair);
        count++;
    }

    public int Size()
    {
        return count;
    }

    public int Capacity()
    {
        return slots.Length;
    }

    public bool AddOrReplace(K key, V value)
    {
        int slotNumber = GetSlotNumber(key);
        var slot = slots[slotNumber];

        if (slot == null)
        {
            slot = new LinkedList<KeyValue<K, V>>();
            slots[slotNumber] = slot;
        }

        foreach (var keyValue in slot)
        {
            if (keyValue.Key.Equals(key))
            {
                slot.Remove(keyValue);
                count--;
                break;
            }
        }

        Add(key, value);
        return false;
    }

    public V Get(K key)
    {
        int slotNumber = GetSlotNumber(key);
        var slot = slots[slotNumber];

        if (slot != null)
        {
            foreach (var keyValue in slot)
            {
                if (keyValue.Key.Equals(key))
                {
                    return keyValue.Value;
                }
            }
        }

        throw new KeyNotFoundException("Key not found in hashtable");
    }

    public KeyValue<K, V> Find(K key)
    {
        int slotNumber = GetSlotNumber(key);
        var slot = slots[slotNumber];

        if (slot != null)
        {
            foreach (var keyValue in slot)
            {
                if (keyValue.Key.Equals(key))
                {
                    return keyValue;
                }
            }
        }

        return null;
    }

    public bool ContainsKey(K key)
    {
        int slotNumber = GetSlotNumber(key);
        var slot = slots[slotNumber];

        if (slot != null)
        {
            foreach (var keyValue in slot)
            {
                if (keyValue.Key.Equals(key))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool Remove(K key)
    {
        int slotNumber = GetSlotNumber(key);
        var slot = slots[slotNumber];

        if (slot != null)
        {
            foreach (var keyValue in slot)
            {
                if (keyValue.Key.Equals(key))
                {
                    slot.Remove(keyValue);
                    count--;
                    return true;
                }
            }
        }

        return false;
    }

    public void Clear()
    {
        slots = new LinkedList<KeyValue<K, V>>[InitialCapacity];
        count = 0;
    }

    public IEnumerable<K> Keys()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                foreach (var keyValue in slot)
                {
                    yield return keyValue.Key;
                }
            }
        }
    }

    public IEnumerable<V> Values()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                foreach (var keyValue in slot)
                {
                    yield return keyValue.Value;
                }
            }
        }
    }

    public IEnumerator<KeyValue<K, V>> GetEnumerator()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                foreach (var keyValue in slot)
                {
                    yield return keyValue;
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
class Программа
{
    static void Main(string[] аrgs)
    {
        HashTable<string, int> hashtable = new HashTable<string, int>();

        // Цикл для добавления 100 тысяч элементов
        for (int i = 0; i < 100000; i++)
        {
            // Генерация случайного ключа и значения для добавления в таблицу
            string key = Guid.NewGuid().ToString(); // Генерация случайного ключа
            int value = i; // Просто используем значение i для примера

            // Добавление элемента в хеш-таблицу
            hashtable.Add(key, value);
        }

        // Показать коллизии после добавления элементов

        Console.WriteLine(hashtable.ShowCollisions());
    }
}

