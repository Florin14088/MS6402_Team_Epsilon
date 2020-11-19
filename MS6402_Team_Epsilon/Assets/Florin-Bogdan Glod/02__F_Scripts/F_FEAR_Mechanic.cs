using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_FEAR_Mechanic : MonoBehaviour
{
    [Range(0, 100)] public float fear_meter = 0;//the current fear level
    [HideInInspector] public float containerFear;//last known value of fear_meter
    [Range(1, 100)] public float debuffIncrement = 2;//example: if the fear is supposed to increase by 10, the fear will increase by 10/debuffIncrement
    [Range(1, 100)] public float buffIncrement = 1.2f;//example: if the fear is supposed to increase by 10, the fear will increase by 10/debuffIncrement
    [Range(0, 100)] public float fear_decrease_bit = 1;//how much fear is deducted every cooldownDecreaseFear
    public float cooldownDecreaseFear = 1;//used with next_cooldownDecreaseFear for cooldown
    [HideInInspector] public float next_cooldownDecreaseFear = 0;//used with cooldownDecreaseFear for cooldown
    [Space]
    [Space]
    public GameObject playerTrack; //monitor rotation on Y axis
    public GameObject camTrack; //monitor rotation on X axis
    [Space]
    [Space]
    public float monitor_rotY;//current rotation is put into this variable
    public float monitor_rotX;//current rotation is put into this variable
    [Space]
    [Space]
    [HideInInspector] public float container_rotY;//here is saved the previous known value of monitor_rotY
    [HideInInspector] public float container_rotX;//here is saved the previous known value of monitor_rotX
    [Space]
    [Space]
    [HideInInspector] public float difference_monitor_container_Y;//the difference between monitor_rotY and container_rotY
    [HideInInspector] public float difference_monitor_container_X;//the difference between monitor_rotX and container_rotX
    [Space]
    [Space]
    public GameObject needleFear_UI;






    void Start()
    {
        monitor_rotY = playerTrack.transform.rotation.y;//getting the initial rotation on Y axis for the monitored object
        monitor_rotX = camTrack.transform.rotation.x;//getting the initial rotation on X axis for the monitored object

        fear_meter = containerFear;

        container_rotY = monitor_rotY;//saving the value in a container
        container_rotX = monitor_rotX;//saving the value in a container

        difference_monitor_container_Y = 0;//difference between monitor_rotY and container_rotY is 0 at the beginning
        difference_monitor_container_X = 0;//difference between monitor_rotX and container_rotX is 0 at the beginning

    }//Start





    void Update()
    {
        monitor_rotY = playerTrack.transform.rotation.y;//rotation is put into this variable
        monitor_rotX = camTrack.transform.rotation.x;//rotation is put into this variable


        if (container_rotY != monitor_rotY)//if the previous (and stored) value is different from the current value
        {
            difference_monitor_container_Y = monitor_rotY - container_rotY;//calculate the difference between tracked value and container
            if (difference_monitor_container_Y < 0) difference_monitor_container_Y *= -1;//if the difference is negative make *(-1)
            container_rotY = monitor_rotY;//container value is replaced with tracked value

            fear_meter += difference_monitor_container_Y / debuffIncrement * buffIncrement;//increase fear and use the debuff to diminuate the increment
            difference_monitor_container_Y = 0;//the difference has been accounted for, now set it back to 0

        }


        if(container_rotX != monitor_rotX)//if the previous (and stored) value is different from the current value
        {
            difference_monitor_container_X = monitor_rotX - container_rotX;//calculate the difference between tracked value and container
            if (difference_monitor_container_X < 0) difference_monitor_container_X *= -1;//if the difference is negative make *(-1)
            container_rotX = monitor_rotX;//container value is replaced with tracked value

            fear_meter += difference_monitor_container_X / debuffIncrement * buffIncrement;//increase fear and use the debuff to diminuate the increment
            difference_monitor_container_X = 0;//the difference has been accounted for, now set it back to 0

        }


        if(fear_meter > 0)
        {
            if(Time.time > next_cooldownDecreaseFear)
            {
                next_cooldownDecreaseFear = Time.time + cooldownDecreaseFear;
                fear_meter -= fear_decrease_bit;
            }

            if (fear_meter < 0) fear_meter = 0;
        }


        if(fear_meter != containerFear)//if current fear value is not the same with the last known fear value
        {
            containerFear = fear_meter;//change the last known value of fear_meter;

            needleFear_UI.transform.rotation = Quaternion.Euler(0 , 0 , -fear_meter * 180/100);

        }


    }//Update




}//END