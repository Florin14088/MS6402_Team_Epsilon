using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_FOV_Trigger_Increase : MonoBehaviour
{
    public Camera cam;
    public float fovIncrease;
    public float timeToIncrease = 1f;
    public float timeToDecrease = 1f;
    public AnimationCurve increaseCurve;
    public GameObject startFov_object;
    public GameObject endFov_object;
    [Space]
    [Space]
    public float originalFov;


    void Start()
    {
        originalFov = cam.fieldOfView;

    }//Start



    void Update()
    {

    }//Update



    public IEnumerator FOVKickUp()
    {
        float t = Mathf.Abs((cam.fieldOfView - originalFov) / fovIncrease);
        while (t < timeToIncrease)
        {
            cam.fieldOfView = originalFov + (increaseCurve.Evaluate(t / timeToIncrease) * fovIncrease);
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }//FOVKickUp


    public IEnumerator FOVKickDown()
    {
        float t = Mathf.Abs((cam.fieldOfView - originalFov) / fovIncrease);
        while (t > 0)
        {
            cam.fieldOfView = originalFov + (increaseCurve.Evaluate(t / timeToDecrease) * fovIncrease);
            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //make sure that fov returns to the original size
        cam.fieldOfView = originalFov;

    }//FOVKickDown



    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger Enter");
        if(other.gameObject == startFov_object)
        {
           // Debug.Log("Trigger increase");
            StopAllCoroutines();
            StartCoroutine(FOVKickUp());
        }

        if(other.gameObject == endFov_object)
        {
            //Debug.Log("Trigger decrease");
            StopAllCoroutines();
            StartCoroutine(FOVKickDown());
        }

    }



}//END
