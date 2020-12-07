using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class F_STATIC
{


    #region Load scene using given name
    public static void Demand_New_Level(string sceneName) //public static function with one parameter required
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);//unloading the current scene
        Resources.UnloadUnusedAssets();//unloading unused assets
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);//loading the scene that was passed in the parameter
    }
    #endregion





    #region Open my itch.io page (in scene End Credits is used)   ANA-MARIA MAVROIANU

    //This function is used in End Credits scene, to make sure that player get to the correct web page
    public static void Demand_OpenMyWebPage_URSA()//public static function that can be called from anywhere. No parameter is required
    {
        Application.OpenURL("https://ursa-bitexe.itch.io/");//command to open the given URL in the browser.
    }
    #endregion




    #region Open my itch.io page (in scene End Credits is used)   FLORIN GLOD

    //This function is used in End Credits scene, to make sure that player get to the correct web page
    public static void Demand_OpenMyWebPage_PAIN()//public static function that can be called from anywhere. No parameter is required
    {
        Application.OpenURL("https://floring-pain.itch.io/");//command to open the given URL in the browser.
    }
    #endregion




    #region Give FEAR Damage

    public static void Demand_SendFear(float quantity)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<F_FEAR_Mechanic>().fear_meter += quantity;
    }
    #endregion





}
