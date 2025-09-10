using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager _instance;
    public List<Pool> poolList = new List<Pool>();

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
}
