using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Door_Helper : MonoBehaviour
{
    public bool b_v1 = false;
    public bool b_none = false;

    public F_Door_Controller __scriptDoorCont;


    private void Start()
    {
        //__scriptDoorCont = gameObject.transform.root.transform.gameObject.GetComponent<F_Door_Controller>();

    }


    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == __scriptDoorCont.interestTag)
        {

            __scriptDoorCont.b_inRange = true;

            if (b_none) return;
            if (b_v1) __scriptDoorCont.b_I_am_v1 = true;
            else __scriptDoorCont.b_I_am_v1 = false;
        }
        
    }




    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == __scriptDoorCont.interestTag)
        {
            __scriptDoorCont.uiPan_txt.text = " ";
            __scriptDoorCont.uiPan_Feedback.SetActive(false);
            __scriptDoorCont.b_inRange = false;
        }
           

    }

}
