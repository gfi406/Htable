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

public class HashTable<K, V>
{
    private class LinkedListNode
    {
        public KeyValue<K, V> Item { get; }
        public LinkedListNode Next { get; set; }

        public LinkedListNode(KeyValue<K, V> item)
        {
            Item = item;
            Next = null;
        }
    }

    private LinkedListNode[] slots;
    private const int InitialCapacity = 16;
    private const double LoadFactor = 0.80d;
    private int count;

    public HashTable()
    {
        slots = new LinkedListNode[InitialCapacity];
        count = 0;
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
        slots = new LinkedListNode[slots.Length * 2];
        count = 0;

        foreach (var slot in tempSlots)
        {
            var currentNode = slot;
            while (currentNode != null)
            {
                Add(currentNode.Item.Key, currentNode.Item.Value);
                currentNode = currentNode.Next;
            }
        }
    }

    public int ShowCollisions()
    {
        int collisionCount = 0;
        foreach (var slot in slots)
        {
            int nodeCount = 0;
            var currentNode = slot;
            while (currentNode != null)
            {
                nodeCount++;
                currentNode = currentNode.Next;
            }

            if (nodeCount > 1)
            {
                collisionCount++;
            }
        }
        return collisionCount;
    }

    public void Add(K key, V value)
    {
        GrowIfNeeded();
        int slotNumber = GetSlotNumber(key);

        if (slots[slotNumber] == null)
        {
            slots[slotNumber] = new LinkedListNode(new KeyValue<K, V>(key, value));
        }
        else
        {
            var currentNode = slots[slotNumber];
            while (currentNode.Next != null)
            {
                if (currentNode.Item.Key.Equals(key))
                {
                    Console.WriteLine("Element with this key already exists in the hash table.");
                    return;
                }
                currentNode = currentNode.Next;
            }
            if (currentNode.Item.Key.Equals(key))
            {
                Console.WriteLine("Element with this key already exists in the hash table.");
                return;
            }
            currentNode.Next = new LinkedListNode(new KeyValue<K, V>(key, value));
        }
        count++;
    }

    // Remaining methods remain unchanged...
}

class Program
{
    static void Main(string[] args)
    {
        HashTable<string, int> hashtable = new HashTable<string, int>();

        for (int i = 0; i < 100000; i++)
        {
            string key = Guid.NewGuid().ToString();
            int value = i;
            hashtable.Add(key, value);
        }

        Console.WriteLine(hashtable.ShowCollisions());
    }
}
