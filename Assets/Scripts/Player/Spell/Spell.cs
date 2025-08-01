using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Spell : MonoBehaviour
{
    LayerMask mask;
    PoolObject pool;
    Rigidbody body;
    FullSpell currData;

    [Header("Base value")]
    bool canGo;
    bool touch;
    float speed;
    float currlifetime;
    float deathTime;
    [Header("Delay values")]
    bool isDelaying;
    float delayTime;
    [Header("Explosion values")]
    bool resetAfterExplosion;
    List<Rigidbody> explosionBodyList = new List<Rigidbody>();

    [Header("Wait effect")]
    bool needWait;
    float waitTime;
    [Header("Lock on")]
    bool mustLock;

    [Header("Hidden Values")]
    int indexCurrentBehaviour;
    delegate void CurrentAction();
    CurrentAction lastAction;

    #region System
    void Awake()
    {
        //First set some variables for later then deactivate the object for it to stay available in the pool
        mask = LayerMask.GetMask("Self");
        body = GetComponent<Rigidbody>();
        pool = GetComponent<PoolObject>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        //The delay that skips the rest, used like a pause or smth
        if (isDelaying)
        {
            delayTime -= Time.deltaTime;
            Debug.Log("delay remaining = " + delayTime);

            if (delayTime <= 0)
                isDelaying = false;
            else
                return;
        }

        //bool for the projectile to move only when i want it to move (when active mostly)
        if (canGo)
            Move();
        
        //Changes in behavior when the projectile hits something
        if (touch)
        {
            if (mustLock)
                TimedDestroy();
            else
                resetAfterExplosion = true;

            //When touch = true, i check  if the Wait modifier is on or not
            WaitOrPass();
        }
    }
    void WaitOrPass()
    {
        //Check if the last behavior (using a delegate) has to loop of if it can go to the next one
        if (needWait)
        {
            waitTime -= Time.deltaTime;
            lastAction();

            if (waitTime <= 0)
                needWait = false;
        }
        else
        {
            //Since im using SO, i need to reset them before going to the next, otherwise the modifiers will stay active next time ill use this behavior
            if (indexCurrentBehaviour < currData.followEffects.Count)
                ResetBehaviors();

            GetNextAction();
        }
    }
    void Reset()
    {
        //Reset the projectile 
        Debug.Log("Reset");
        canGo = false;
        touch = false;

        body.isKinematic = true;
        resetAfterExplosion = true;

        explosionBodyList.Clear();
        indexCurrentBehaviour = 0;

        pool.ReturnToPool();
    }

    void ResetBehaviors()
    {
        //Reset the behavior
        if (currData.followEffects[indexCurrentBehaviour].currtType == AddedBehavior.dataType.Behaviour)
        {
            Debug.Log("Reset Behavior " + indexCurrentBehaviour);

            explosionBodyList.Clear();
            currData.followEffects[indexCurrentBehaviour].modValue = 1;
        }
    }
    #endregion

    #region Spell construction
    public void Init(FullSpell data)
    {
        //copies the SO for all the script to use, then sets the base projectile values
        currData = Instantiate(data);

        ReadProjectileData((SpellData)currData.followEffects[0]);

        gameObject.SetActive(true);
        body.isKinematic = false;
        canGo = true;
    }

    void GetNextAction()
    {
        //increases the index in the list of AddedBehavior from the SO and does something if the new behavior is an action
        Debug.Log("Get new action");
        indexCurrentBehaviour++;

        if (indexCurrentBehaviour < currData.followEffects.Count && currData.followEffects[indexCurrentBehaviour].currtType == AddedBehavior.dataType.Behaviour)
            ReadNewBehavior();
    }

    void ReadProjectileData(SpellData data)
    {
        //Base projectile stats
        speed = data.f_speed;
        transform.localScale = Vector3.one * data.f_size;
        currlifetime = data.f_lifetime;
        body.mass = data.f_mass;
    }

    void ReadNewBehavior()
    {
        //I first check for modifiers, then i call the modded behavior
        if ((indexCurrentBehaviour + 1) < currData.followEffects.Count && currData.followEffects[indexCurrentBehaviour + 1].currtType == AddedBehavior.dataType.Modifier)
            CheckModifiers();

        switch (currData.followEffects[indexCurrentBehaviour].id)
        {
            case 1:
                Debug.Log("Explosion");
                lastAction = SpellExplosion;
                lastAction();
                break;
            case 2:
                Debug.Log("Delay");
                SetDelay();
                break;
            case 3:
                Debug.Log("Lock");
                SetLockOnTouch();
                break;
        }
    }
    void CheckModifiers()
    {
        //Check for any modifiers after the behavior then applies them
        for (int i = 1; i < currData.followEffects.Count; i++)
        {
            if ((indexCurrentBehaviour + i) < currData.followEffects.Count && currData.followEffects[indexCurrentBehaviour + i].currtType == AddedBehavior.dataType.Modifier)
                ReadModifierData(i);
            else
                break;
        }
    }

    void ReadModifierData(int gap)
    {
        //Applies the modifier(s) following the behavior
        switch (currData.followEffects[indexCurrentBehaviour + gap].id)
        {
            case 1:
                //reverse
                Debug.Log("reverse " + currData.followEffects[indexCurrentBehaviour] + "base " + currData.followEffects[indexCurrentBehaviour].modValue);
                currData.followEffects[indexCurrentBehaviour].modValue = currData.followEffects[indexCurrentBehaviour].modValue * currData.followEffects[indexCurrentBehaviour + gap].modValue;
                break;
            case 2:
                //wait
                Debug.Log("Wait");
                needWait = true;
                waitTime = currData.followEffects[indexCurrentBehaviour + gap].modValue;
                break;
        }
    }
    #endregion


    #region SpellActions
    void Move()
    {
        //Simple movement
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter(Collision other)
    {
        //Unity's collision
        if (other.gameObject.layer != mask)
            touch = true;
    }

    void TimedDestroy()
    {
        //After the projectile touched, it will disappear after an extended time
        resetAfterExplosion = false;
        currlifetime -= Time.deltaTime;

        if (currlifetime <= 0)
            Reset();

        if (mustLock)
        {
            body.isKinematic = true;
            canGo = false;
        }
    }

    void SpellExplosion()
    {
        //Simple explosion that pushes rigidbodies away, i keep the bodies in a list in case a Wait modifier loops the method
        ExplosionSpell spell = (ExplosionSpell)currData.followEffects[indexCurrentBehaviour];

        RaycastHit[] ray = Physics.SphereCastAll(transform.position, spell.f_explosionRadius, Vector3.one);
        if (explosionBodyList.Count == 0)
        {
            for (int i = 0; i < ray.Length; i++)
            {
                if (ray[i].transform.tag == "PhysicsObjects")
                {
                    explosionBodyList.Add(ray[i].collider.GetComponent<Rigidbody>());
                }
            }
        }

        foreach (Rigidbody obj in explosionBodyList)
        {
            Debug.Log(spell.f_explosionStrength * spell.modValue + " " + spell.modValue);
            obj.AddExplosionForce(spell.f_explosionStrength * spell.modValue, transform.position, spell.f_explosionRadius);
        }

        if (resetAfterExplosion)
            Reset();
    }

    void SetDelay()
    {
        //Sets the delay
        DelaySpell spell = (DelaySpell)currData.followEffects[indexCurrentBehaviour];
        delayTime = spell.delayTime;
        isDelaying = true;
    }

    void SetLockOnTouch()
    {
        //Sets the collision interaction
        LockOnTouch onTouch = (LockOnTouch)currData.followEffects[indexCurrentBehaviour];

        currlifetime = onTouch.f_timeBeforeDestruction;
        mustLock = onTouch.b_lockOnTouch;
        resetAfterExplosion = false;
    }

    #endregion
}
