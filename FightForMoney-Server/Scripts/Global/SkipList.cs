using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

delegate bool SkipListCompare(int one_key, int two_key);

class SkipNode
{
    public int Key;
    public SkipNode Forward;    //右节点
    public SkipNode Bottom;     //下节点

    public SkipNode(int key)
    {
        this.Key = key;
    }
}

class SkipUnit
{
    public SkipNode[] NodeList;

    public SkipUnit(int level)
    {
        this.NodeList = new SkipNode[level];
    }
}

class SkipList
{
    private int MaxLevel = 32;
    private int CurLevel = 0;
    private SkipUnit Header;
    private SkipListCompare Compare;
    private Dictionary<int, object> DataMap = new Dictionary<int, object>();

    public SkipList(SkipListCompare compare)
    {
        this.Header = new SkipUnit(this.MaxLevel);
        this.Compare = compare;
    }

    public void Insert(int key, object data)
    {
        this.Remove(key);

        this.DataMap.Add(key, data);
        SkipNode node = new SkipNode(key);

        int level = this.RandomLevel();
        for (int i = CurLevel; i <= level; i++)
        {
            this.Header.NodeList[i - 1] = new SkipNode(0);
        }
    }

    public void Remove(int key)
    {
        if (!this.DataMap.ContainsKey(key))
        {
            return;
        }

        this.DataMap.Remove(key);
    }

    private int RandomLevel()
    {
        int level = 1;
        Random random = new Random();
        while (random.Next(0, 1) == 1)
        {
            level++;
            if(level >= this.MaxLevel)
            {
                break;
            }
        }
        return level;
    }
}
