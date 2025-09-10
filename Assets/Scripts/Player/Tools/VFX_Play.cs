using UnityEngine;

public class VFX_Play : MonoBehaviour, VFX_Interface
{
    ParticleSystem system;

    void Awake()
    {
        system = GetComponent<ParticleSystem>();
    }
    
    void VFX_Interface.Show(float scale, float duration)
    {
        system.Play();
    }
}
