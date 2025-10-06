using System.Collections.Generic;
using UnityEngine;

public class VFX_Manager : MonoBehaviour
{
    [SerializeField] List<Pool> vfxPools = new List<Pool>();
    public static VFX_Manager _instance;

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    public GameObject GetVFX(AddedBehavior.vfx vfx)
    {
        return vfxPools[(int)vfx].GetItem();
    }
    public GameObject GetVFX(RW_Behavior.vfx vfx)
    {
        return vfxPools[(int)vfx].GetItem();
    }
}
