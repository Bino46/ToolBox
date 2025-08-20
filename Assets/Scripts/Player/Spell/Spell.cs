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
    CompliedSpell currData;

    [Header("Base value")]
    bool canGo;
    bool touch;
    float speed;
    float currlifetime;
    [Header("Delay values")]
    bool isDelaying;
    float delayTime;
    [Header("Explosion values")]
    bool resetAfterExplosion;
    List<Rigidbody> explosionBodyList = new List<Rigidbody>();
    [SerializeField] GameObject bh;

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
            //Debug.Log("delay remaining = " + delayTime);

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
        //Debug.Log("Reset");
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
            //Debug.Log("Reset Behavior " + indexCurrentBehaviour);

            explosionBodyList.Clear();
            currData.followEffects[indexCurrentBehaviour].modStrengthValue = 1;
            currData.followEffects[indexCurrentBehaviour].modDurationValue = 1;
        }
    }
    #endregion

    #region Spell construction
    public void Init(CompliedSpell data)
    {
        //copies the SO for all the script to use, then sets the base projectile values
        currData = Instantiate(data);

        ReadProjectileData((SimpleProjectileData)currData.followEffects[0]);

        gameObject.SetActive(true);
        body.isKinematic = false;
        canGo = true;
    }

    void GetNextAction()
    {
        //increases the index in the list of AddedBehavior from the SO and does something if the new behavior is an action
        //Debug.Log("Get new action");
        indexCurrentBehaviour++;

        if (indexCurrentBehaviour < currData.followEffects.Count && currData.followEffects[indexCurrentBehaviour].currtType == AddedBehavior.dataType.Behaviour)
            ReadNewBehavior();
    }

    void ReadProjectileData(SimpleProjectileData data)
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
                //Debug.Log("Explosion");
                lastAction = SpellExplosion;
                lastAction();
                break;
            case 2:
                //Debug.Log("Delay");
                SetDelay();
                break;
            case 3:
                //Debug.Log("Lock");
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
        BaseModifier currModifier = (BaseModifier)currData.followEffects[indexCurrentBehaviour + gap];
        //Applies the modifier(s) following the behavior
        switch (currModifier.id)
        {
            case 1:
                //Increase strength
                //Debug.Log("Increase");
                ChangeModValue(currModifier.modStrengthValue, currModifier.operation, true);
                break;

            case 2:
                //Decrease strength
                //Debug.Log("Decrease");
                ChangeModValue(currModifier.modStrengthValue, currModifier.operation, true);
                break;

            case 3:
                //wait
                //Debug.Log("Wait");
                needWait = true;
                waitTime = currModifier.modDurationValue;
                break;

            case 4:
                //Increase duration
                //Debug.Log("Increase");
                ChangeModValue(currModifier.modDurationValue, currModifier.operation, false);
                break;

            case 5:
                //Decrease Duration
                //Debug.Log("Decrease");
                ChangeModValue(currModifier.modDurationValue, currModifier.operation, false);
                break;
            case 6:
                GameObject newBh = Instantiate(bh, transform.position, bh.transform.rotation);
                newBh.GetComponent<ScaleBlackhole>().Summon(currData.followEffects[indexCurrentBehaviour].modDurationValue, waitTime, false);
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
            Debug.Log(spell.f_explosionStrength * spell.modStrengthValue + " strength");
            Debug.Log(spell.f_explosionRadius * spell.modDurationValue + " radius");
            obj.AddExplosionForce(spell.f_explosionStrength * spell.modStrengthValue, transform.position, spell.f_explosionRadius * spell.modDurationValue);
        }

        if (resetAfterExplosion)
        {
            ResetBehaviors();
            Reset();
        }
    }

    void SetDelay()
    {
        //Sets the delay
        DelaySpell spell = (DelaySpell)currData.followEffects[indexCurrentBehaviour];
        delayTime = spell.delayTime * Mathf.Abs(spell.modDurationValue);
        isDelaying = true;
    }

    void SetLockOnTouch()
    {
        //Sets the collision interaction
        LockOnTouch onTouch = (LockOnTouch)currData.followEffects[indexCurrentBehaviour];

        currlifetime = onTouch.f_timeBeforeDestruction * Mathf.Abs(onTouch.modDurationValue);
        mustLock = onTouch.b_lockOnTouch;
        resetAfterExplosion = false;
    }

    #endregion

    #region Modifiers

    void ChangeModValue(float mod, BaseModifier.Operation op, bool strength)
    {
        if (strength)
        {
            switch (op)
            {
                case BaseModifier.Operation.Add:
                    currData.followEffects[indexCurrentBehaviour].modStrengthValue = currData.followEffects[indexCurrentBehaviour].modStrengthValue + mod;
                    break;
                case BaseModifier.Operation.Multiply:
                    currData.followEffects[indexCurrentBehaviour].modStrengthValue = currData.followEffects[indexCurrentBehaviour].modStrengthValue * mod;
                    break;
                case BaseModifier.Operation.Substract:
                    currData.followEffects[indexCurrentBehaviour].modStrengthValue = currData.followEffects[indexCurrentBehaviour].modStrengthValue - mod;
                    break;
                case BaseModifier.Operation.Divide:
                    currData.followEffects[indexCurrentBehaviour].modStrengthValue = currData.followEffects[indexCurrentBehaviour].modStrengthValue / mod;
                    break;
            }
        }
        else
        {
            switch (op)
            {
                case BaseModifier.Operation.Add:
                    currData.followEffects[indexCurrentBehaviour].modDurationValue = currData.followEffects[indexCurrentBehaviour].modDurationValue + mod;
                    break;
                case BaseModifier.Operation.Multiply:
                    currData.followEffects[indexCurrentBehaviour].modDurationValue = currData.followEffects[indexCurrentBehaviour].modDurationValue * mod;
                    break;
                case BaseModifier.Operation.Substract:
                    currData.followEffects[indexCurrentBehaviour].modDurationValue = currData.followEffects[indexCurrentBehaviour].modDurationValue - mod;
                    break;
                case BaseModifier.Operation.Divide:
                    currData.followEffects[indexCurrentBehaviour].modDurationValue = currData.followEffects[indexCurrentBehaviour].modDurationValue / mod;
                    break;
            }
        }
    }
    #endregion
}
