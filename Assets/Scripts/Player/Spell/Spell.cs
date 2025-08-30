using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    LayerMask mask;
    PoolObject pool;
    //Rigidbody body;
    CompiledSpell currData;
    Projectile projMovement;

    [Header("Delay values")]
    bool isDelaying;
    float delayTime;
    [Header("Explosion values")]
    List<Rigidbody> explosionBodyList = new List<Rigidbody>();
    [SerializeField] GameObject bh;

    [Header("Wait effect")]
    bool needWait;
    float waitTime;
    bool forceWaitTime;
    [SerializeField] float forceWaitTimeValue;
    [Header("Lock on")]
    bool mustLock;

    [Header("Hidden Values")]
    int indexCurrentBehaviour;
    delegate void CurrentAction();
    CurrentAction lastAction;
    bool touch;

    #region System
    void Awake()
    {
        //First set some variables for later then deactivate the object for it to stay available in the pool
        mask = LayerMask.GetMask("Self");
        pool = GetComponent<PoolObject>();
        projMovement = GetComponent<Projectile>();
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

        //Changes in behavior when the projectile hits something
        if (touch)
        {   
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
            //I give just a small delay because otherwise the projectile gets destroyed before the behavior takes place
            if (forceWaitTime)
            {
                SetWaitTime(forceWaitTimeValue);

                lastAction = EmptyDelegate;
                forceWaitTime = false;

                return;
            }
            
            //Since im using SO, i need to reset them before going to the next, otherwise the modifiers will stay active next time ill use this behavior
            if (indexCurrentBehaviour >= currData.followEffects.Count && !mustLock)
            {
                FullReset();
                return;
            }
            //Debug.Log(indexCurrentBehaviour + " " + currData.followEffects.Count);

            GetNextAction();
        }
    }
    void Reset()
    {   
        touch = false;

        forceWaitTime = true;

        explosionBodyList.Clear();
        indexCurrentBehaviour = 0;

        pool.ReturnToPool();
    }

    void ResetBehaviors()
    {
        //Reset the behavior
        foreach (AddedBehavior bh in currData.followEffects)
        {
            if (bh.currtType == AddedBehavior.dataType.Behaviour)
            {
                bh.modStrengthValue = 1;
                bh.modDurationValue = 1; 
            }
        }

        explosionBodyList.Clear();
    }

    public void FullReset()
    {
        ResetBehaviors();
        projMovement.ResetMovement();
        Reset();
    }

    #endregion

    #region Spell construction

    public void SetCompiledSpell(CompiledSpell data)
    {
        //copies the SO for all the script to use, then sets the base projectile values
        currData = Instantiate(data);

    }
    public void Init(CompiledSpell data)
    {
        projMovement.InitMovement(data, (BaseProjectile)currData.followEffects[0]);
        gameObject.SetActive(true);
    }

    void GetNextAction()
    {
        //increases the index in the list of AddedBehavior from the SO and does something if the new behavior is an action
        //Debug.Log("Get new action");
        indexCurrentBehaviour++;

        if (indexCurrentBehaviour < currData.followEffects.Count && currData.followEffects[indexCurrentBehaviour].currtType == AddedBehavior.dataType.Behaviour)
            ReadNewBehavior();
    }

    void ReadNewBehavior()
    {
        //I first check for modifiers, then i call the modded behavior
        if ((indexCurrentBehaviour + 1) < currData.followEffects.Count && currData.followEffects[indexCurrentBehaviour + 1].currtType == AddedBehavior.dataType.Modifier)
            CheckModifiers(indexCurrentBehaviour + 1);

        forceWaitTime = true;

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
        }
    }
    void CheckModifiers(int startIndex)
    {
        //Check for any modifiers after the behavior then applies them
        for (int i = startIndex; i < currData.followEffects.Count; i++)
        {
            if ((indexCurrentBehaviour + i) < currData.followEffects.Count && currData.followEffects[indexCurrentBehaviour + i].currtType == AddedBehavior.dataType.Modifier)
                ReadBehaviorModifierData(i);
            else
                break;
        }
    }

    void ReadBehaviorModifierData(int gap)
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
                SetWaitTime(currModifier.modDurationValue);
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

    void OnCollisionEnter(Collision other)
    {
        //Unity's collision
        if (other.gameObject.layer != mask)
        {
            touch = true;
            TouchBehavior();
        }
    }
    void TouchBehavior()
    {
        //After the projectile touched, it will disappear after an extended time

        if (mustLock)
        {
            forceWaitTime = false;
            projMovement.LockProjectile(true);
        }
    }

    void SetWaitTime(float time)
    {
        needWait = true;
        waitTime = time;
    }

    void SpellExplosion()
    {
        //Simple explosion that pushes rigidbodies away, i keep the bodies in a list in case a Wait modifier loops the method
        if (currData.followEffects[indexCurrentBehaviour].currtType == AddedBehavior.dataType.Behaviour)
        {
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

            //Debug.Log("explosion");

            foreach (Rigidbody obj in explosionBodyList)
            {
                obj.AddExplosionForce(spell.f_explosionStrength * spell.modStrengthValue, transform.position, spell.f_explosionRadius * spell.modDurationValue);
            } 
        }
    }

    void SetDelay()
    {
        //Sets a pause in the spell actions
        DelaySpell spell = (DelaySpell)currData.followEffects[indexCurrentBehaviour];
        delayTime = spell.delayTime * Mathf.Abs(spell.modDurationValue);
        projMovement.ExtendLifetime(delayTime);
        
        //Debug.Log("delay");
        isDelaying = true;
    }

    void EmptyDelegate()
    {
    }

    #endregion

    #region Modifiers
    public void SetLockOnTouch(int val)
    {
        //Sets the collision interaction
        LockOnTouch onTouch = (LockOnTouch)currData.followEffects[val];

        projMovement.ExtendLifetime(onTouch.f_timeBeforeDestruction * Mathf.Abs(onTouch.modDurationValue));

        mustLock = onTouch.b_lockOnTouch;
        forceWaitTime = false;
    }

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
