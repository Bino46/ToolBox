using UnityEngine;

public interface VFX_Interface
{
    public void Show(float scale, float duration);
    public void Show(float scale, float duration, bool activateChildren, int damage);
}
