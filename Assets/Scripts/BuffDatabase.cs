using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffDatabase", menuName = "Game/BuffDatabase")]
public class BuffDatabase : ScriptableObject
{
    public List<Buff> buffList;
    // Start is called before the first frame update
    public Buff GetBuffByName(string name)
    {
        return buffList.Find(buff => buff.name == name);
    }
}
