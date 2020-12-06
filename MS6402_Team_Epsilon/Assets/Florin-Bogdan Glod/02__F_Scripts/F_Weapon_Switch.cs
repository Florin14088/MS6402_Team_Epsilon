using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class F_Weapon_Switch : MonoBehaviour
{
    public GameObject item_1;
    public GameObject item_2;
    [Space]
    public KeyCode key_itm1 = KeyCode.Alpha1;
    public KeyCode key_itm2 = KeyCode.Alpha2;
    [Space]
    public GameObject panel_item_1;
    public GameObject panel_item_2;





    void Start()
    {
        item_2.SetActive(false);
        panel_item_2.SetActive(false);
        panel_item_1.SetActive(true);

    }//Start




    void Update()
    {
        if (Input.GetKeyDown(key_itm1))
        {
            //print("Hands");
            item_1.SetActive(true);
            item_2.SetActive(false);

            panel_item_1.SetActive(true);
            panel_item_2.SetActive(false);

        }

        if (Input.GetKeyDown(key_itm2))
        {
            //print("Pistol");
            item_1.SetActive(false);
            item_2.SetActive(true);

            panel_item_1.SetActive(false);
            panel_item_2.SetActive(true);

        }



    }//Update




}//END
