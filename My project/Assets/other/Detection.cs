using System.Collections;
using System.Collections.Generic;
using K_PathFinder.Samples;
using UnityEngine;

public class Detection : MonoBehaviour
{
    private GameObject PlayerExample;

    public bool playerFinded = false; 
    public Transform player; 
    
    
    // void OnCollisionEnter(Collision collision)
    // {
    //     // foreach (ContactPoint contact in collision.contacts)
    //     // {
    //         // Debug.DrawRay(contact.point, contact.normal, Color.white);
    //          Debug.Log("collision");
    //     // }
    // }
        
void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.name == "Player")
    {
        PlayerExample = GameObject.Find("Agent");

        PointOfView rb = PlayerExample.GetComponent<PointOfView>();
        rb.playerFinded = true;

        EnemyAI ex = PlayerExample.GetComponent<EnemyAI>();
        ex.playerFinded = true;
    }
}

    
    //
    // // Gets called during the collision
    // void OnCollisionStay(Collision collision)
    // {
    //     Debug.Log("Colliding with " + collision.gameObject.name);
    // }
    //
    // // Gets called when the object exits the collision
    // void OnCollisionExit(Collision collision)
    // {
    //     Debug.Log("Exited collision with " + collision.gameObject.name);
    // }
}
