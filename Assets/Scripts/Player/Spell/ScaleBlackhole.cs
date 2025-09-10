using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

public class ScaleBlackhole : MonoBehaviour, VFX_Interface
{
    [SerializeField] Material m_bh;
    Material copy;
    Light ligth;
    [SerializeField] GameObject bh;
    public float scale;
    float maxScale;
    float timer;
    bool isDestroying;
    [SerializeField] float speed;
    [SerializeField] float sizeModifer;
    public float lifeTime;

    void Start()
    {
        copy = Instantiate(m_bh);
        bh.GetComponent<MeshRenderer>().material = copy;
        scale = 0;
        ligth = GetComponentInChildren<Light>();
        transform.rotation = Random.rotation;
    }

    void Update()
    {
        lifeTime -= Time.deltaTime;

        if (lifeTime < 0 && !isDestroying)
        {
            Summon(100, 0, true);
            ligth.intensity = 0;
        }
        ChangeSize();
    }

    public void Summon(float newScale, float duration, bool destroy)
    {
        maxScale = Mathf.Abs(newScale) * sizeModifer;

        lifeTime = duration;
        timer = 0;

        isDestroying = destroy;
    }

    void ChangeSize()
    {
        timer += Time.deltaTime * speed;

        if (isDestroying)
            scale = Mathf.Lerp(scale, -0.1f, timer);

        if (scale < 0)
        {
            scale = 0;
            GetComponent<PoolObject>().ReturnToPool();
        }
            
        if (!isDestroying && lifeTime > 0)
            scale = Mathf.Lerp(0, maxScale, timer);

        copy.SetFloat("_GConst", scale / 35);
        bh.transform.localScale = Vector3.one * scale;
    }

    void VFX_Interface.Show(float scale, float duration)
    {
        Summon(scale, duration, false);
    }
}
