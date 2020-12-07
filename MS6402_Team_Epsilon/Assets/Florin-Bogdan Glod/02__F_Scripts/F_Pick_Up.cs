using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Pick_Up : MonoBehaviour
{
    #region CLASSES

    [System.Serializable] public class General
    {
        [Header("General Stuff")]
        public List<string> interestTag = new List<string>() {"Player"};
        public bool b_oneUse = false;//destroy after use? if this is false, then after each use, the object will be hidden for cooldownUse seconds
        public float cooldownUse = 10;
        public bool b_overrideDestroy = false;
    }

    [System.Serializable] public class FearStuff
    {
        [Header("Stuff regarding Fear Meter")]
        public bool b_useThis = false;
        [Range(-100, 100)] public float amount = 0; //+50 will increase fear be 50  ||| -50 will decrease fear by 50   ||| etc
    }

    [System.Serializable] public class MaxAmmo
    {
        [Header("Stuff regarding maximum Ammo")]
        public bool b_useThis = false;
        [Range(0, 100)] public float amount = 0; 
    }

    [System.Serializable] public class AvailableAmmo
    {
        [Header("Stuff regarding available Ammo")]
        public bool b_useThis = false;
        [Range(0, 100)] public float amount = 0; 
    }

    [System.Serializable] public class GOAL_Main_1
    {
        [Header("Stuff regarding first Main goal")]
        public bool b_collected = false;
        
    }

    [System.Serializable] public class GOAL_Side_1
    {
        [Header("Stuff regarding first side goal")]
        public bool b_collected = false;

    }

    [System.Serializable] public class GOAL_Side_2
    {
        [Header("Stuff regarding second side goal")]
        public bool b_collected = false;

    }

    [System.Serializable] public class GOAL_Side_3
    {
        [Header("Stuff regarding third side goal")]
        public bool b_collected = false;

    }
    #endregion



    #region PUBLIC
    [Space]
    [Space]
    [Header("Pick Up Script")]
    public General _class_General = new General();
    [Space]
    [Space]
    public FearStuff _class_Fear = new FearStuff();
    [Space]
    [Space]
    public MaxAmmo _class_Max_AMMO = new MaxAmmo();
    [Space]
    [Space]
    public MaxAmmo _class_Available_AMMO = new MaxAmmo();
    #endregion



    #region PRIVATE

    #endregion







    IEnumerator HiddenMechanic(float timeWaiting)
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(timeWaiting);

        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<Collider>().enabled = true;

        yield break;

    }//HiddenMechanic



    private void OnTriggerEnter(Collider other)
    {
        #region FEAR stuff here

        if (_class_Fear.b_useThis)
        {
            foreach (string s in _class_General.interestTag)
            {
                if (other.gameObject.tag == s)
                {
                    other.gameObject.transform.root.transform.gameObject.GetComponent<F_FEAR_Mechanic>().fear_meter += _class_Fear.amount;//fear will increase with the amount. If amount is negative, then fear will decrease
                }//bracket froom IF

            }//bracket from FOREACH
            
        }//bracket from IF       

        #endregion


        #region Maximum AMMO stuff here

        if (_class_Max_AMMO.b_useThis)
        {
            foreach (string s in _class_General.interestTag)
            {
                if (other.gameObject.tag == s)
                {
                    other.gameObject.transform.root.transform.gameObject.GetComponent<F_Range_Attack>().maxAmmo += _class_Max_AMMO.amount;
                }//bracket froom IF

            }//bracket from FOREACH

        }//bracket from IF       

        #endregion


        #region Available AMMO stuff here

        if (_class_Available_AMMO.b_useThis)
        {
            foreach (string s in _class_General.interestTag)
            {
                if (other.gameObject.tag == s)
                {
                    other.gameObject.transform.root.transform.gameObject.GetComponentInChildren<SimpleShoot>().availableAmmo += _class_Available_AMMO.amount;
                }//bracket froom IF

            }//bracket from FOREACH
            
        }//bracket from IF       

        #endregion


        #region Here object is destroyed or hidden for a while (see General class)

        if(_class_General.b_overrideDestroy == false)
        {
            foreach (string s in _class_General.interestTag)
            {
                if (other.gameObject.tag == s)
                {
                    if (_class_General.b_oneUse) Destroy(gameObject);
                    else StartCoroutine(HiddenMechanic(_class_General.cooldownUse));
                }
            }
        }


        #endregion


    }//OnTriggerEnter



}//END

