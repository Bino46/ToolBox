using System.Collections.Generic;
using NaughtyAttributes.Test;
using UnityEngine;

public class RW_Spell : MonoBehaviour
{
    private RW_SO_DataSpell data;
    private RW_Projectile projecile;
    private PoolObject pool;
    private List<RW_Behavior> behaviors = new List<RW_Behavior>();
    [Header("Main Values")]
    public float PauseTime;
    public int RepeatCount;
    [SerializeField] float f_timeBetweenRepeats;
    [SerializeField] bool b_needRepeat;
    [Header("Hidden Values")]
    float pauseTimer;
    public bool touchedGround;
    int indexCurrentBehavior;
    delegate void CurrentAction();
    CurrentAction lastAction;
    int currRepeatCount;
    Vector3 hitPosition;

    void Awake()
    {
        for (int i = 0; i < gameObject.GetComponentCount(); i++)
        {
            if (gameObject.GetComponentAtIndex(i) is RW_Behavior)
            {
                behaviors.Add((RW_Behavior)gameObject.GetComponentAtIndex(i));
            }
        }

        projecile = GetComponent<RW_Projectile>();
        pool = GetComponent<PoolObject>();
    }

    public void InitSpell(RW_SO_DataSpell newData, Vector3 startPos, Vector3 dir)
    {
        data = newData;
        projecile.Init(data, startPos, dir);

        for (int i = 0; i < data.loadedBehaviorCount; i++)
        {
            InitSpecificBehavior(i, data.behaviorAndModifiers[i].behaviorID);
        }
    }

    void InitSpecificBehavior(int id, int bhId)
    {
        switch (bhId)
        {
            case 0:
                behaviors[id].GetComponent<RW_b_Explosion>().Init(data);
                break;
            case 1:
                behaviors[id].GetComponent<RW_b_Delay>().Init(data);
                break;
            case 2:
                behaviors[id].GetComponent<RW_b_Slash>().Init(data);
                break;
        }
    }

    void Update()
    {
        if (touchedGround)
            HandleTimers();
    }

    void HandleTimers()
    {
        Pause();

        if (!Pause() && !b_needRepeat)
            NextBehavior();
        else if (b_needRepeat)
            Repeat();
    }

    void NextBehavior()
    {
        //Activate the next behavior on the data list (modifiers have already been applied)
        //Projectiles should handle their lifetime, howev0er need a point where the projectile disappears once the list is empty
        if (data.loadedBehaviorCount > 0 && indexCurrentBehavior < data.loadedBehaviorCount)
        {
            lastAction = ActivateCurrentBehavior;
            lastAction();

            pauseTimer = PauseTime;
            indexCurrentBehavior++;
        }
        else if (indexCurrentBehavior >= data.loadedBehaviorCount)
        {
            ResetSpell();
        }
    }

    void ActivateCurrentBehavior()
    {
        int val = data.behaviorAndModifiers[indexCurrentBehavior].behaviorID;

        if (val >= 0)
            behaviors[val].UseAbility(hitPosition);
    }

    bool Pause()
    {
        //Pause between behaviors, can be increased with Delay
        if (pauseTimer <= 0)
            return false;

        pauseTimer -= Time.deltaTime;
        return true;
    }

    public void Repeat()
    {
        //Repeats the last behavior, reduced pause between repeats
        if (!b_needRepeat)
        {
            b_needRepeat = true;
            currRepeatCount = 0;
        }

        if (!Pause())
        {
            pauseTimer = f_timeBetweenRepeats;
            lastAction();

            currRepeatCount++;
        }

        if (currRepeatCount >= RepeatCount)
            b_needRepeat = false;
    }

    public void GetSignal(Vector3 pos)
    {
        touchedGround = true;
        hitPosition = pos;
    }

    void ResetSpell()
    {
        touchedGround = false;
        b_needRepeat = false;

        currRepeatCount = 0;
        indexCurrentBehavior = 0;

        projecile.ResetProjectile();

        pool.ReturnToPool();
    }
}
