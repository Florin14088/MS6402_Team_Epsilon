using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Camera_Controller_FPS : MonoBehaviour
{
    public GameObject body;
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    [Space]
    [Space]
    public float minY;
    public float maxY;

    private float yaw = 0.0f;//stanga dreapta
    private float pitch = 0.0f;//sus jos
    private float usedYaw = 0.0f;
    private float usedpitch = 0.0f;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }//Start


    void Update()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        usedYaw = yaw;
        if (pitch <= maxY && pitch >= minY) usedpitch = pitch;

        if (pitch > maxY) pitch = maxY;
        if (pitch < minY) pitch = minY;

        Quaternion rot = gameObject.transform.rotation;
        rot.x = 0;
        rot.z = 0;
        body.transform.rotation = rot;
        transform.eulerAngles = new Vector3(usedpitch, usedYaw, 0.0f);
    }



}//END
