using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class OneKeyDictionart<Key, Value>
{
    private Dictionary<Key, Value> map = new Dictionary<Key, Value>();

    public Value Get(Key key)
    {
        if (!this.map.ContainsKey(key))
        {
            return default(Value);
        }
        return this.map[key];
    }

    public void Set(Key key, Value value)
    {
        this.map[key] = value;
    }

    public List<Value> ToList()
    {
        List<Value> list = new List<Value>();
        foreach (KeyValuePair<Key, Value> pair in this.map)
        {
            list.Add(pair.Value);
        }
        return list;
    }
}

class TwoKeyDictionary<Key1, Key2, Value>
{
    private Dictionary<Key1, OneKeyDictionart<Key2, Value>> map = new Dictionary<Key1, OneKeyDictionart<Key2, Value>>();

    public Value Get(Key1 key1, Key2 key2)
    {
        if (!this.map.ContainsKey(key1))
        {
            return default(Value);
        }
        return this.map[key1].Get(key2);
    }

    public void Set(Key1 key1, Key2 key2, Value value)
    {
        if (!this.map.ContainsKey(key1))
        {
            this.map[key1] = new OneKeyDictionart<Key2, Value>();
        }
        this.map[key1].Set(key2, value);
    }

    public List<Value> ToList()
    {
        List<Value> list = new List<Value>();
        foreach (KeyValuePair<Key1, OneKeyDictionart<Key2, Value>> pair in this.map)
        {
            list.AddRange(pair.Value.ToList());
        }
        return list;
    }
}

class ThreeKeyDictionary<Key1, Key2, Key3, Value>
{
    private Dictionary<Key1, TwoKeyDictionary<Key2, Key3, Value>> map = new Dictionary<Key1, TwoKeyDictionary<Key2, Key3, Value>>();

    public Value Get(Key1 key1, Key2 key2, Key3 key3)
    {
        if (!this.map.ContainsKey(key1))
        {
            return default(Value);
        }
        return this.map[key1].Get(key2, key3);
    }

    public void Set(Key1 key1, Key2 key2, Key3 key3, Value value)
    {
        if (!this.map.ContainsKey(key1))
        {
            this.map[key1] = new TwoKeyDictionary<Key2, Key3, Value>();
        }
        this.map[key1].Set(key2, key3, value);
    }

    public List<Value> ToList()
    {
        List<Value> list = new List<Value>();
        foreach (KeyValuePair<Key1, TwoKeyDictionary<Key2, Key3, Value>> pair in this.map)
        {
            list.AddRange(pair.Value.ToList());
        }
        return list;
    }
}

