using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    [Header("Location Refrences")]
    [SerializeField] private Animator anim;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Weapon Stats")]
    [Tooltip("The most ammo that the player can have at any given moment")] public float maxAmmo = 36;
    [Tooltip("The ammo available for the player to use in this moment")] public float availableAmmo = 24;
    [Tooltip("The ammo that can be stored in a clip (how much ammo a reload can give)")] public float clipAmmo = 6;
    [Tooltip("The ammo that is loaded in the weapon (weapon clip)")] public float currentAmmo = 6;
    [Tooltip("How fast can the weapon shot")] public float cooldownFire = 0.3f;
    [HideInInspector] public float next_CooldownFire = 0.3f;
    [HideInInspector] public bool b_needReloading = false;//this true = weapon cannot fire
    [HideInInspector] public bool b_alreadyreloading = false;//true when already reloading weapon, prevent having 2 reloads in the same time for example
    [HideInInspector] public bool b_processFinished = false; //used in coroutine that is reloading the weapon to prevent weapon being reloaded more than once per reloading
    [Tooltip("How much it takes to reload?")] public float durationReloading = 2f;
    [Tooltip("Which key to press for reload?")] public KeyCode reloadKey = KeyCode.R;
     public bool b_externalPermission = false;//manipulated by F_Player_Helpers.cs script

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer_casing = 2f;
    [Tooltip("Specify time to destory the bullet object")] [SerializeField] private float destroyTimer_bullet = 5f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;

    [Header("Sounds")]
    [Tooltip("Sound when weapon fire a bullet")] public GameObject fireSound;
    [Tooltip("Sound when weapon is asked to fire a bullet, but there is no availableAmmo")] public GameObject dryfireSound;
    [Tooltip("Sound used when reloading")] public GameObject reloadSound;




    void Start()
    {
        if (barrelLocation == null) barrelLocation = transform;


        if (anim == null) anim = GetComponentInChildren<Animator>();

    }//Start



    void Update()
    {

        Reloading();


        if (b_needReloading) return;


        if (Input.GetKeyDown(reloadKey) && currentAmmo < clipAmmo && availableAmmo > 0) b_needReloading = true;


        if (Input.GetKey(KeyCode.Mouse1) == false) return;//first aim, then fire

        if (Input.GetKey(KeyCode.Mouse0) && b_externalPermission)//fire
        {
            if(Time.time > next_CooldownFire)
            {
                next_CooldownFire = Time.time + cooldownFire;

                #region Current ammo (clip ammo) is 0. Reload if possible or play dry fire sound

                if (currentAmmo <= 0)
                {
                    if (availableAmmo > 0)
                    {
                        b_needReloading = true;
                    }
                    else
                    {
                        Instantiate(dryfireSound, gameObject.transform.position, gameObject.transform.rotation);//play dry fire sound
                    }

                }
                #endregion


                #region Current ammo (clip ammo) is bigger than 0. Deduct one ammo and play fire animation
                if (currentAmmo > 0)
                {
                    currentAmmo--;//............................................................................take one bullet                    

                    anim.SetTrigger("Fire"); //I ONLY NEED TO CALL THE ANIMATION. THERE ARE FLAGS AND STUFF ON ANIMATION TO CALL THE REST OF THE FUNCTIONS AT THE EXACT FRAME

                }
                #endregion
            }
        }



    }//Update



    //CALLED BY FIRE ANIMATION
    void Shoot()
    {
        if (muzzleFlashPrefab)
        {
            GameObject tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);
            GameObject tempFireSound = Instantiate(fireSound, gameObject.transform.position, gameObject.transform.rotation);//.....play fire sound
            
            Destroy(tempFlash, destroyTimer_casing);//Destroy the muzzle flash effect
            Destroy(tempFireSound, destroyTimer_casing);//Destroy sound effect
        }

        //cancels if there's no bullet prefeb
        if (!bulletPrefab) return;

        // Create a bullet and add force on it in direction of the barrel
        GameObject bulletTemp = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
        bulletTemp.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);

        Destroy(bulletTemp, destroyTimer_bullet);


    }//Shoot



    //CALLED BY FIRE ANIMATION
    void CasingRelease()
    {
        if (!casingExitLocation || !casingPrefab) return;

        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation);

        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);

        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer_casing);


    }//CasingRelease



    private void Reloading()
    {
        if (b_needReloading && b_alreadyreloading == false)
        {
            b_alreadyreloading = true;
            //the player will have a reloading animation that will be triggered by the same "R" key
            StartCoroutine(ReloadPatience(durationReloading));
        }


    }//Reloading


    IEnumerator ReloadPatience(float durationPending)
    {
        yield return new WaitForSeconds(durationPending);

        if(availableAmmo >= clipAmmo && b_processFinished == false)
        {
            b_processFinished = true;
            availableAmmo -= clipAmmo;//deduct one clip size from available ammo
            currentAmmo = clipAmmo;//load one clip size into gun
        }

        if(availableAmmo < clipAmmo && b_processFinished == false)
        {
            b_processFinished = true;
            availableAmmo = 0;//available ammo is now zero
            currentAmmo = availableAmmo;//load all available ammo in the weapon
        }


        b_alreadyreloading = false;
        b_needReloading = false;
        b_processFinished = false;
        StopCoroutine(ReloadPatience(durationReloading));
    }



}//END