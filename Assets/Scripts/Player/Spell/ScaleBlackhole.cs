using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

public class ScaleBlackhole : MonoBehaviour
{
    [SerializeField] Material m_bh;
    Material copy;
    Light ligth;
    [SerializeField] GameObject bh;
    public float scale;
    float maxScale;
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

        if (lifeTime < 0)
        {
            Summon(100, 0, true);
            ligth.intensity = 0;
        }

        if (isDestroying && scale > 0)
            scale -= Time.deltaTime * speed;

        scale = Mathf.Clamp(scale, 0, maxScale);
        
        if (!isDestroying && lifeTime > 0)
            scale += Time.deltaTime * speed;

        copy.SetFloat("_GConst", scale / 35);
        bh.transform.localScale = Vector3.one * scale;
    }

    public void Summon(float scale, float duration, bool destroy)
    {
        maxScale = Mathf.Abs(scale) * sizeModifer;
        lifeTime = duration;
        isDestroying = destroy;
    }
}
