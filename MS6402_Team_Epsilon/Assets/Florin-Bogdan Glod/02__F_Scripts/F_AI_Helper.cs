using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class F_AI_Helper : MonoBehaviour
{
    public List<F_Door_Controller> allDoors = new List<F_Door_Controller>();
    public float range_manipulate = 20;
    public bool b_Stuck = false;
    public float cooldown = 10;
    [HideInInspector] public float nextCooldown = 0;
    [Space]
    public string[] messagesTexts;
    public GameObject uiPanel_Message;
    public Text uiTXT;
    public bool b_textVisible = false;
    public float cooldownText = 5;
    [HideInInspector] public float next_cooldownText = 0;
    [Space]
    public GameObject player;
    public float playerProximity = 10;
    [Space]
    public int patientNumber;



    void Start()
    {
        patientNumber = Random.Range(258, 872);

        nextCooldown = Random.Range(3, 5);
        player = GameObject.FindGameObjectWithTag("Player");
        uiPanel_Message.SetActive(false);
        uiTXT.text = " ";
        if (uiTXT == null) Debug.LogError($"Ai uitat sa pui un Text din UI pe  -  {gameObject.name}  -  in scriptul  -  'F_AI_Helper'");
        if (uiPanel_Message == null) Debug.LogError($"Ai uitat sa pui un Panel din UI pe  -  {gameObject.name}  -  in scriptul  -  'F_AI_Helper'");
        if(messagesTexts.Length == 0) Debug.LogError($"Array-ul de pe  -  {gameObject.name}  -   nu are nici un continut  -  in scriptul  -  'F_AI_Helper'. Adauga macar 1 item");

    }//Start



    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= playerProximity)
        {
            if (Time.time > nextCooldown)
            {
                nextCooldown = Time.time + cooldown;

                if (b_textVisible)
                {
                    uiPanel_Message.SetActive(true);
                    uiTXT.text = "Patient #" + patientNumber + ": " + messagesTexts[Random.Range(0, messagesTexts.Length)].ToString();
                }

                allDoors.Clear();
                CastSphere_GetDoors();
                Manipulate_Doors();
            }
        }
        

        if(b_textVisible && Vector3.Distance(player.transform.position, transform.position) > playerProximity)
        {
            uiPanel_Message.SetActive(false);
            uiTXT.text = " ";
        }


    }//Update



    void CastSphere_GetDoors()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range_manipulate);
        foreach (Collider hitC in hitColliders)
        {
            if (hitC.gameObject.GetComponent<F_Door_Controller>())
            {
                if(!allDoors.Contains(hitC.gameObject.GetComponent<F_Door_Controller>())) allDoors.Add(hitC.gameObject.GetComponent<F_Door_Controller>());
            }

        }
    }//CastSphere_GetDoors






    void Manipulate_Doors()
    {
        if (b_Stuck)
        {
            foreach (F_Door_Controller cra in allDoors)
            {
                cra.StuckDoor();
            }
            return;
        }


        foreach (F_Door_Controller cra in allDoors)
        {
            if (cra.b_doorOpen == true)
            {
                if (cra.anim.GetInteger("Pain") == 5) cra.anim.SetInteger("Pain", 4);
                if (cra.anim.GetInteger("Pain") == 1) cra.anim.SetInteger("Pain", 2);
                cra.b_doorOpen = false;
            }
            else if (cra.b_doorOpen == false)
            {
                if (cra.b_I_am_v1) cra.anim.SetInteger("Pain", 5);
                else cra.anim.SetInteger("Pain", 1);
                cra.b_doorOpen = true;
            }
        }




    }//Manipulate_Doors



}//END
