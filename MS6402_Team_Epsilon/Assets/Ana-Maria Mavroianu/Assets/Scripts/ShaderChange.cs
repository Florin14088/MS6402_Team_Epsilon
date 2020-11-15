using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderChange : MonoBehaviour
{
    public List<Material> matList = new List<Material>();

    private Shader shader_dissolv;
    private Shader shader_lit;




    // Start is called before the first frame update
    void Start()
    {
        shader_dissolv = Shader.Find("Shader Graphs/Dissolve");    // asign shaders value
        shader_lit = Shader.Find("Lightweight Render Pipeline/Lit");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            foreach(Material m in matList)
            {
                if (m.shader == shader_lit) m.shader = shader_dissolv;
                else m.shader = shader_lit;
            }
        }
    }



}
