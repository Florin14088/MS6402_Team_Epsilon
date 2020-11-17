using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Camera_Stalker : MonoBehaviour
{
    public Transform targetPos;

    


    void Update()
    {
        transform.position = targetPos.position;

    }//Update


}//END
