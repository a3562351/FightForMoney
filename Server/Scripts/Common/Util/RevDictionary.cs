using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class RevDictionary<Key, Value>
{
    private Dictionary<Key, Value> key_to_value = new Dictionary<Key, Value>();
    private Dictionary<Value, Key> value_to_key = new Dictionary<Value, Key>();

    public void Add(Key key, Value value)
    {
        this.key_to_value[key] = value;
        this.value_to_key[value] = key;
    }

    public void Remove(Key key)
    {
        if (this.HaveKey(key))
        {
            Value value = this.key_to_value[key];
            this.key_to_value.Remove(key);
            this.value_to_key.Remove(value);
        }
    }

    public void Remove(Value value)
    {
        if (this.HaveValue(value))
        {
            Key key = this.value_to_key[value];
            this.value_to_key.Remove(value);
            this.key_to_value.Remove(key);
        }
    }

    public bool HaveKey(Key key)
    {
        return this.key_to_value.ContainsKey(key);
    }

    public bool HaveValue(Value value)
    {
        return this.value_to_key.ContainsKey(value);
    }

    public Key GetKey(Value value)
    {
        if (!this.HaveValue(value))
        {
            return default(Key);
        }
        return this.value_to_key[value];
    }

    public Value GetValue(Key key)
    {
        if (!this.HaveKey(key))
        {
            return default(Value);
        }
        return this.key_to_value[key];
    }
}
