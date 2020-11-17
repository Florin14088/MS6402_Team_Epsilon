using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Camera_Stalker : MonoBehaviour
{
    public Transform targetPos;
    public Transform targetRot;

    


    void Update()
    {
        transform.position = targetPos.position;
        //transform.rotation = targetRot.rotation;

    }//Update


}//END
