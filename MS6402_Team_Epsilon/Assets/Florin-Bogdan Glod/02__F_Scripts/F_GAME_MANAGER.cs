using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_GAME_MANAGER : MonoBehaviour
{
    #region Own CLASSES
    [System.Serializable] public class Fear_Man_In_The_Middle
    {
    }
    #endregion



    #region PUBLIC
    public Fear_Man_In_The_Middle cls_MiD = new Fear_Man_In_The_Middle();
    #endregion



    #region PRIVATE

    #endregion



    #region PRE DEFINITED FUNCTIONS
    void Start()
    {
       

    }//Start


    void Update()
    {
        Check_NPCs();


    }//Update
    #endregion


    #region OWN FUNCTIONS

    void Check_NPCs()
    {

    }//Check_NPCs

    #endregion


}//END