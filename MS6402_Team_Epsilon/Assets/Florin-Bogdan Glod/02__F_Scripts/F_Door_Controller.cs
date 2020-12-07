using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class F_Door_Controller : MonoBehaviour
{
    public bool b_available = false;//true = door can be interacted ||| false = door cannot be interacted (STUCK ANIMATION WILL PLAY ON INTERACTION)
    [HideInInspector] public bool b_readyToUse = true;//true = door is not performing any animation right now ||| false = door is already performing an animation
    [HideInInspector] public bool b_doorOpen = false;//true = on interaction the door will close  ||| false = on interaction the door will open (if possible)
    public bool b_doorNeedKey = false;//door starts as stuck (b_available = false) but if player finds a key the door can become b_available = true
    [HideInInspector] public bool b_inRange = false;//made true or false by OnTriggerEnter/OnTriggerExit true = player is close enough to interact with the door
    [HideInInspector] public bool b_I_am_v1 = false; //the door have 2 triggers, one on each side of the door. One is named v1 and will trigger a set of animations, the other one is v2, with the other set
    [Space]
    [Space]
    public float pendingTime_open = 1f;//duration of open animations
    public float pendingTime_close = 1f;//duration of close animations
    [Space]
    [Space]
    public string interestTag = "Player";
    public bool b_NPC_can_use = false;
    [Space]
    [Space]
    public GameObject uiPan_Feedback;//panel letting player know "can use door" or ""
    public Text uiPan_txt;//text that is child of uiPan_Feedback
    public KeyCode interactKey = KeyCode.F;//key that when pressed will trigger an animation 
    [Space]
    [Space]
    public string doorStuck_msg = "Door is stuck. I need to find another way";
    public string doorStuck_NEED_KEY_msg = "Door is locked. Looks like I need a key";
    public string doorClosed_msg ="Open the door by pressing ";
    public string doorOpened_msg ="Close the door by pressing ";
    public Animator anim;



    void Start()
    {
        anim = GetComponentInChildren<Animator>(); 
        uiPan_txt.text = " ";
        uiPan_Feedback.SetActive(false);
        b_inRange = false;

    }//Start





    void Update()
    {
        if (b_inRange == false) return;
        if (b_readyToUse == false) return;

        if (b_inRange)
        {
            if (b_readyToUse == false) return;

            uiPan_Feedback.SetActive(true);

            if (b_available == false && b_doorNeedKey == false) uiPan_txt.text = doorStuck_msg;

            if (b_available == false && b_doorNeedKey) uiPan_txt.text = doorStuck_NEED_KEY_msg;


            if (b_available)
            {
                if (b_doorOpen == false) uiPan_txt.text = doorClosed_msg + interactKey;
                if (b_doorOpen) uiPan_txt.text = doorOpened_msg + interactKey;

            }
        }
        


        if (Input.GetKeyDown(interactKey))
        {
            if(b_doorOpen) StartCoroutine(Door_Busy_Closing());

            if (b_doorOpen == false) StartCoroutine(Door_Busy_Opening());

            if (b_available == false) StartCoroutine(Door_Busy_Stuck());
        }
        
    }//Update



    IEnumerator Door_Busy_Opening()
    {
        b_readyToUse = false;

        if (b_I_am_v1) anim.SetInteger("Pain", 5);
        else anim.SetInteger("Pain", 1);



        yield return new WaitForSeconds(pendingTime_open + 0.2f);
        b_doorOpen = true;
        b_readyToUse = true;
    }


    IEnumerator Door_Busy_Closing()
    {
        b_readyToUse = false;

        if (anim.GetInteger("Pain") == 5) anim.SetInteger("Pain" , 4);
        if (anim.GetInteger("Pain") == 1) anim.SetInteger("Pain" , 2);

        yield return new WaitForSeconds(pendingTime_close + 0.2f);
        b_doorOpen = false;
        b_readyToUse = true;
    }


    IEnumerator Door_Busy_Stuck()
    {
        b_readyToUse = false;

        anim.SetInteger("Pain" , 3);

        yield return new WaitForSeconds(0.7f);
        b_doorOpen = false;
        anim.SetInteger("Pain", 0);
        b_readyToUse = true;
    }




    #region FUNCTIONS FOR NPCs


    public void StuckDoor()
    {
        if (b_readyToUse == false) return;
        StartCoroutine(Door_Busy_Stuck());
    }//StuckDoor

    #endregion




}//END
