using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;



public class EnviromentChanger : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] public bool disableCollisionOnDash = true;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindWithTag("Player");
        player.GetComponent<Movement>().dashStart.AddListener(DashStart);
        player.GetComponent<Movement>().dashEnd.AddListener(DashEnd);
        
    }

    // Update is called once per frame
    public void DashStart()
    {
        Collider2D[] colliders = gameObject.GetComponents<Collider2D>();
        Collider2D[] playerColliders = player.GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            foreach (Collider2D bcollider in playerColliders)
            {
                Physics2D.IgnoreCollision(bcollider, collider, true);
                Physics2D.IgnoreCollision(collider, bcollider, true);
            }
        }
        Debug.Log("Start");
    }

    public void DashEnd()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        Collider2D[] playerColliders = player.GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            foreach (Collider2D bcollider in playerColliders)
            {
                Physics2D.IgnoreCollision(bcollider, collider, false);
                Physics2D.IgnoreCollision(collider, bcollider, false);
            }
        }
        Debug.Log("End");
    }
}
