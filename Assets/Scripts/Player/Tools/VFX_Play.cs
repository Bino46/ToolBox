using Mono.Cecil.Cil;
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

    void VFX_Interface.Show(float scale, float duration, bool activateChildren, int damage)
    {
        if (isParticle)
        {
            system.Play();
            system.transform.GetChild(0).gameObject.SetActive(activateChildren);
        }
        else
        {
            effect.Play();
            effect.transform.GetChild(0).gameObject.SetActive(activateChildren);
        }

        //poor code but eh
        if (activateChildren)
            transform.GetChild(0).GetComponent<SlashHit>().slashDamage = damage;
    }
}
