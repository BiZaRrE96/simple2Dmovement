using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 5f; // Movement speed
    
    private Rigidbody2D rb;
    private playerInput playerInput;

    //DASH RELATED STUFF, might be moved to a different script
    public float DashCD = 3.0f;
    public float ActiveDashCD = 0.0f;
    public float DashDuration = 0.1f;
    public float DashPower = 20.0f;
    private float ActiveDashDuration = 0.0f;

    private void Awake()
    {
        playerInput = GetComponent<playerInput>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Disable gravity
    }

    private void FixedUpdate()
    {

        // Calculate movement vector
        Vector2 movement = new Vector2(playerInput.inputX, playerInput.inputY);
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Apply movement with speed
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

        // some sort of dash.
        if (playerInput.utilityUsed && ActiveDashCD == 0.0f || (ActiveDashDuration > 0.0f && (ActiveDashDuration < DashDuration)))
        {
            float dashspeed = Mathf.Sin(ActiveDashDuration / DashDuration * Mathf.PI) * Mathf.Sin(ActiveDashDuration / DashDuration * Mathf.PI) * DashPower;
            rb.MovePosition(rb.position + movement * dashspeed * Time.fixedDeltaTime);
            ActiveDashDuration += Time.fixedDeltaTime;
            if (ActiveDashDuration > DashDuration)
            {
                ActiveDashCD = DashCD;
            }
            
        }
        else
        {
            ActiveDashCD -= Time.fixedDeltaTime;
            Debug.Log(ActiveDashCD);
            Debug.Log(ActiveDashDuration);
            if (ActiveDashCD < 0.0f)
            {
                ActiveDashCD = 0.0f;
                ActiveDashDuration = 0.0f;
            }
        }
        
    }
}