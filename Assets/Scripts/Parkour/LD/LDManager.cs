using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class LDManager : MonoBehaviour
{
    [SerializeField] AIController player;
    [SerializeField] Transform spawnPoint;

    [Header("LD")]
    [SerializeField] int seed;
    List<GameObject> children = new List<GameObject>();
    public enum _Difficulty { Easy = 7, Medium = 14, Hard = 21 }
    public _Difficulty currDifficulty;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            children.Add(transform.GetChild(i).gameObject);
        }

        foreach (GameObject gm in children)
        {
            gm.GetComponent<Bang>().SetParent(this);
        }

        MakeNewLD();
    }

    [Button]
    void MakeNewLD()
    {
        GenerateSeed();

        for (int i = 0; i < children.Count; i++)
            children[i].SetActive(false);

        Random.InitState(seed);

        for (int i = 0; i <= (int)currDifficulty; i++)
        {
            int random = (int)(Random.value * 100) % children.Count;
            children[random].SetActive(true);
        }

    }

    void GenerateSeed()
    {
        seed = Random.Range(0, 500);
    }

    public void Respawn()
    {
        player.Reset();
        player.transform.position = spawnPoint.transform.position;
        MakeNewLD();
    }
}
