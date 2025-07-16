using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PushTest : MonoBehaviour
{
    [SerializeField] float force;
    [SerializeField] float radius;
    List<Rigidbody> pushList = new List<Rigidbody>();
    List<ControllerV2> playerList = new List<ControllerV2>();
    enum LaunchStyle { trampoline, sphere }
    [SerializeField] LaunchStyle currStyle;
    ControllerV2 player;

    [Button]
    void Push()
    {
        if (pushList.Count > 0)
        {
            foreach (Rigidbody body in pushList)
            {
                body.AddExplosionForce(force, transform.position, radius);
            }
        }

        if (playerList.Count > 0)
        {
            Explosion explosion = new Explosion();
            foreach (ControllerV2 body in playerList)
            {
                Debug.Log("hit player");
                body.Throw(explosion.SetExplosion(body.transform.position, transform.position, radius, force));
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody newBody = other.GetComponent<Rigidbody>();
        player = other.GetComponentInParent<ControllerV2>();

        if (player != null && !playerList.Contains(player))
        {
            playerList.Add(player);
            return;
        }

        if (newBody != null)
        {
            pushList.Add(newBody);

            if (currStyle == LaunchStyle.trampoline)
            {
                newBody.AddForce(Vector3.left * force, ForceMode.Impulse);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Rigidbody newBody = other.GetComponent<Rigidbody>();
        player = other.GetComponentInParent<ControllerV2>();

        if (player != null && playerList.Contains(player))
        {
            playerList.Remove(player);
            return;
        }

        if (newBody != null)
            pushList.Remove(newBody);
    }
}
