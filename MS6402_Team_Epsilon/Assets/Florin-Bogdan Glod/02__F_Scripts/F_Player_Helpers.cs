using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.Animations.Rigging; 
/// <summary>
/// This script works with the script that is controlling the player and with the script that is on the pistol. 
/// To fire, player first needs to aim. To do that, player needs to hold right click pressed, and an animation will play, with player raising weapon.
/// After the weapon is in position, the player can fire by pressing left click
/// </summary>
public class F_Player_Helpers : MonoBehaviour
{
    public GameObject pistolHandgun;
    public Animator anim;
    public float delayApprovalShooting;
    [HideInInspector] public float containerTime;
    [HideInInspector] public float cooldown = 0.1f;
    [HideInInspector] public float nextCooldown = 0;
    [HideInInspector] public F_Weapon_Switch __scriptWpnSwitch;
    [Space]
    [Space]
    public GameObject additionalRig;
    public bool b_aiming = false;
    public float cooldownRigMove = 0;
    public float nextCooldownRigMove = 0;
    public float valueModificated = 0.07f;





    void Start()
    {
        __scriptWpnSwitch = GetComponentInChildren<F_Weapon_Switch>();

    }//Start



    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1) && __scriptWpnSwitch.item_2.activeSelf == true) //this is aiming
        {
            anim.SetInteger("Pain", 1);
            b_aiming = true;

            if (Time.time > nextCooldown && containerTime < delayApprovalShooting)
            {
                nextCooldown = Time.time + cooldown;
                containerTime += cooldown;
            }

            if(containerTime >= delayApprovalShooting)
            {
                pistolHandgun.GetComponentInChildren<SimpleShoot>().b_externalPermission = true;
            }

        }


        if (Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetInteger("Pain", 0);
            b_aiming = false;
            containerTime = 0;
            pistolHandgun.GetComponentInChildren<SimpleShoot>().b_externalPermission = false;
        }


        if(b_aiming && additionalRig.GetComponent<MultiAimConstraint>().weight != 1)
        {
            if(Time.time > nextCooldownRigMove)
            {
                nextCooldownRigMove = Time.time + cooldownRigMove;
                additionalRig.GetComponent<MultiAimConstraint>().weight += valueModificated;
            }
            
        }


        if(b_aiming == false && additionalRig.GetComponent<MultiAimConstraint>().weight != 0)
        {
            if (Time.time > nextCooldownRigMove)
            {
                nextCooldownRigMove = Time.time + cooldownRigMove;
                additionalRig.GetComponent<MultiAimConstraint>().weight -= valueModificated;
            }
        }


    }//Update




}//END
