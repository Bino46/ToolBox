using System;
using UnityEngine;

public class SpellGlowMask : MonoBehaviour
{
    [SerializeField] float speed;
    public float maxSize;
    Vector3 currSize;
    bool grow;

    void Update()
    {
        if (grow && currSize.x < maxSize)
        {
            currSize += Vector3.one * speed * Time.deltaTime;
            currSize = Vector3.ClampMagnitude(currSize, maxSize);

            transform.localScale = currSize;
        }
        else if (!grow && currSize.x > 0)
        {
            currSize -= Vector3.one * speed * Time.deltaTime;

            transform.localScale = currSize;
        }

    }

    public void ActivateSpell()
    {
        grow = true;
    }

    public void DesactivateSpell()
    {
        grow = false;
    }

}
