using UnityEngine;
using UnityEngine.VFX;

public class VFX_Play : MonoBehaviour, VFX_Interface
{
    ParticleSystem system;
    VisualEffect effect;
    bool isParticle;

    void Awake()
    {
        if (GetComponent<ParticleSystem>() != null)
        {
            isParticle = true;
            system = GetComponent<ParticleSystem>();
        }
        else
        {
            isParticle = false;
            effect = GetComponent<VisualEffect>();
        }
    }

    void VFX_Interface.Show(float scale, float duration)
    {
        if (isParticle)
            system.Play();
        else
            effect.Play();
    }
}
