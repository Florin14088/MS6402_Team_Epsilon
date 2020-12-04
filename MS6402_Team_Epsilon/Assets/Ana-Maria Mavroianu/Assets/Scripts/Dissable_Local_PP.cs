using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissable_Local_PP : MonoBehaviour
{
    public GameObject local_PP;
    public GameObject local_PP_02;
    [Space]
    public bool enable_1;
    public bool enable_2;
    // Start is called before the first frame update
    void Start()
    {
        local_PP.SetActive(false);
        local_PP_02.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (enable_1 && local_PP.activeSelf == false)
        {
            enable_2 = false;
            local_PP.SetActive(true);
            local_PP_02.SetActive(false);
        }

        if (enable_2 && local_PP_02.activeSelf == false)
        {
            enable_1 = false;
            local_PP.SetActive(false);
            local_PP_02.SetActive(true);
        }
    }
}
