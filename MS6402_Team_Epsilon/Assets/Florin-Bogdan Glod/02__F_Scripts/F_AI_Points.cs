using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class F_AI_Points : MonoBehaviour
{
    public bool b_player_Been_Here = false;
     public bool b_alreadyCoroutine = false;
    public float timeDissapear = 10;//after the NPC is visible, the NPC will remain visible for this amount of time, then it will dissolve
    public float timeReset = 5;//when the NPC starts to get dissolved, the script will wait for this amount of time before returning the NPC to initial position with normal materials
    [Space]
    public GameObject child_NPC;
    public Renderer sk_meshRen_child;


    public List<Material> matList = new List<Material>();
    public Shader shader_dissolv;
    public Shader shader_lit;




    void Start()
    {
        child_NPC.SetActive(false);
        shader_dissolv = Shader.Find("Shader Graphs/Dissolve 1");    // asign shaders value
        shader_lit = Shader.Find("Universal Render Pipeline/Lit");
        //foreach (Material m in matList)
        //{
        //    m.shader = shader_lit;
        //}
    }//Start




    void Update()
    {
        

    }//Update


    void OnBecameInvisible()
    {
        if (b_player_Been_Here && b_alreadyCoroutine == false)
        {
            b_alreadyCoroutine = true;

            child_NPC.SetActive(true);
            StartCoroutine(PrepareDissapear());
        }
        
    }



    IEnumerator PrepareDissapear()
    {

        yield return new WaitForSeconds(timeDissapear);

        //foreach (Material m in matList)
        //{
        //    m.shader = shader_dissolv;
        //}


        //foreach (Material m in gameObject.GetComponent<Renderer>().materials)
        //{
        //    m.shader = shader_dissolv;
        //}

        Debug.Log($"Name of materials is {sk_meshRen_child.materials[0]}");

        sk_meshRen_child.materials[0].shader = shader_dissolv;
        sk_meshRen_child.materials[1].shader = shader_dissolv;

        StartCoroutine(Waiting_Reset());
        b_player_Been_Here = false;

    }//PrepareDissapear


    IEnumerator Waiting_Reset()
    {

        yield return new WaitForSeconds(timeReset);
        
        Destroy(gameObject);
    }


    private void OnDestroy()
    {
        foreach (Material m in matList)
        {
            m.shader = shader_lit;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && b_alreadyCoroutine == false)
        {
            b_player_Been_Here = true;
            
        }
    }


}//END



