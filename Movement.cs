using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Movement : MonoBehaviour
{
    [SerializeField] private float MaxSpeed = 10f; // Max speed obtainable by walking
    [SerializeField] private float Acceleration = 3f;
    [SerializeField] private float Deceleration = 3f;
    [SerializeField] private float IsometricRatioCoef = 2f;

    //initialize the grid to disable collisions for dash
    [SerializeField] private GameObject enviroment;

    private float forceToAdd;

    private Rigidbody2D rb;
    private playerInput playerInput;

    //CanMove, True berarti player bisa gerak, False ya ngga
    public bool CanMove = true;

    //DASH RELATED STUFF, might be moved to a different script
    public float DashCD = 3.0f;
    public float ActiveDashCD = 0.0f;
    public float DashDuration = 0.1f;
    public float DashPower = 20.0f;
    private float ActiveDashDuration = 0.0f;
    private float dashspeed;
    private float dashdistance;
    public Vector2 dashBeginPos;

    //also dash related
    private bool hasFallen = false;
    private float startfalltime = 1f;
    private float falltime = 1f;
    private Vector3 originalSize;

    //DASH EVENT, used to temporarily disable specific collisions
    public UnityEvent dashStart;
    public UnityEvent dashEnd;

    private bool bothSameDir(float x, float y)
    {
        if ((x >= 0 && y >= 0) || (x <= 0 && y <= 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Awake()
    {
        playerInput = GetComponent<playerInput>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Disable gravity
        originalSize = transform.lossyScale;
    }

    private void FixedUpdate()
    {

        // Calculate movement vector
        Vector2 movement = new Vector2(playerInput.inputX, playerInput.inputY);
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }


        //vt = vo + at^2 therefore a = vt-vo/t^2

        //apply acceleration when there is active input AND velocity is under max
        //apply decceleration when there is no active input OR velocity is over max

        //Debug.Log(rb.velocity);

        if (CanMove)
        {
            if (Mathf.Abs(movement.x) > 0f && Mathf.Abs(rb.velocity.x) <= Mathf.Abs(MaxSpeed * movement.x))
            {
                if (Mathf.Abs(rb.velocity.x) + (Acceleration * (Time.deltaTime * Time.deltaTime)) > Mathf.Abs(MaxSpeed * movement.x))
                {
                    forceToAdd = 0;
                }
                else if (!bothSameDir(movement.x, rb.velocity.x))
                {
                    forceToAdd = Acceleration + Deceleration;
                }
                else
                {
                    forceToAdd = Acceleration;
                }
                rb.AddForce(forceToAdd * new Vector2(movement.x, 0f));
            }
            else //no input or velocity over max
            {
                if (Mathf.Abs(rb.velocity.x) - (Deceleration * (Time.deltaTime * Time.deltaTime)) < 0f)
                {
                    forceToAdd = Mathf.Abs((0f - rb.velocity.x) / (Time.deltaTime * Time.deltaTime));
                }
                else
                {
                    forceToAdd = Deceleration;
                }
                rb.AddForce(forceToAdd * new Vector2(rb.velocity.normalized.x, 0f) * (-1f));
            }

            //MOVEMENT FOR Y
            if (Mathf.Abs(movement.y) > 0f && Mathf.Abs(rb.velocity.y) <= Mathf.Abs(MaxSpeed * movement.y) / IsometricRatioCoef)
            {
                if (Mathf.Abs(rb.velocity.y) + (Acceleration * (Time.deltaTime * Time.deltaTime)) > Mathf.Abs(MaxSpeed * movement.y) / IsometricRatioCoef)
                {
                    forceToAdd = 0;
                }
                else if (!bothSameDir(movement.y, rb.velocity.y))
                {
                    forceToAdd = Acceleration + Deceleration;
                }
                else
                {
                    forceToAdd = Acceleration;
                }
                rb.AddForce(forceToAdd * new Vector2(0f, movement.y) / IsometricRatioCoef);
            }
            else //no input or velocity over max
            {
                if (Mathf.Abs(rb.velocity.y) - (Deceleration * (Time.deltaTime * Time.deltaTime)) < 0f)
                {
                    forceToAdd = Mathf.Abs((0f - rb.velocity.y) / (Time.deltaTime * Time.deltaTime));
                }
                else
                {
                    forceToAdd = Deceleration;
                }
                rb.AddForce(forceToAdd * new Vector2(0f, rb.velocity.normalized.y) * (-1f) / IsometricRatioCoef);
            }
        }


        // some sort of dash.
        if ((playerInput.utilityUsed && ActiveDashCD == 0.0f) || (ActiveDashDuration > 0.0f && (ActiveDashDuration < DashDuration)))
        {
            if (ActiveDashDuration == 0.0f)
            {
                float dashdistance = DashDuration * DashPower / 2;
                dashBeginPos = transform.position;
                // MATIIN COLIDER AIR SELAMA DASH
                dashStart.Invoke();
            }


            float dashspeed = Mathf.Sin(ActiveDashDuration / DashDuration * Mathf.PI) * Mathf.Sin(ActiveDashDuration / DashDuration * Mathf.PI) * DashPower;
            //math, integral utk dapet dash distance

            rb.MovePosition(rb.position + movement * dashspeed * Time.fixedDeltaTime);

            //dash duration tick
            ActiveDashDuration += Time.fixedDeltaTime;

            //apply CD when dash is done
            if (ActiveDashDuration > DashDuration)
            {
                ActiveDashCD = DashCD;
                dashEnd.Invoke();

                if ((Physics2D.Raycast(transform.position, Vector2.up, 0.1f, LayerMask.GetMask("Water")).collider) != null)
                {
                    falltime = startfalltime;
                    Debug.Log("Drown");
                    hasFallen = true;
                }

            }

        }
        else
        {
            ActiveDashCD -= Time.fixedDeltaTime;
            //Debug.Log(ActiveDashCD);
            //Debug.Log(ActiveDashDuration);
            if (ActiveDashCD <= 0.0f) //END OF DASH
            {
                ActiveDashCD = 0.0f;
                ActiveDashDuration = 0.0f;
            }
        }

        //falling
        if (hasFallen)
        {

            if (falltime > 0f)
            {
                CanMove = false;
                falltime -= Time.deltaTime;
                if (falltime < 0f)
                {
                    falltime = 0f;
                }
                transform.localScale = new Vector3(originalSize.x * (falltime + 0.1f) / startfalltime, originalSize.y * (falltime + 0.1f) / startfalltime, originalSize.z);

            }
            else
            {
                transform.localScale = originalSize;
                transform.position = dashBeginPos;
                CanMove = true;
                hasFallen = false;
            }

        }

    }

}