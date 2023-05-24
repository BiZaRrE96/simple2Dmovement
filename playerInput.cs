using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInput : MonoBehaviour
{
    public float inputX;
    public float inputY;
    public float aimAngle;
    public Camera mainCamera;
    public Vector2 aimDir;
    public Vector3 mousePos;

    //some other shit idk
    public bool utilityUsed;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        //Aimdir

        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        aimDir = new Vector2(rotation.x, rotation.y).normalized;

        aimAngle = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        utilityUsed = Input.GetKey(KeyCode.LeftShift);
    }
}
