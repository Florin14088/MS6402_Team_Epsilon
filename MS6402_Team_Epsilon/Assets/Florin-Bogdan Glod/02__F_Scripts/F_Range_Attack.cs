using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Range_Attack : MonoBehaviour
{
    public float maxAmmo = 36;//the most ammo that the player can have at any given moment
    public float availableAmmo = 24;//the ammo available for the player to use in this moment
    public float currentAmmo = 6;//the ammo that is loaded in the weapon (weapon clip)
    [Space]
    [Space]
    public GameObject bulletPrefab;//prefab of projectile that will be instantiated
    public Transform location_Spawn_bullet;
    [Space]
    [Space]
    public ParticleSystem muzzleFlash;//particle system that is placed on gun where the bullet is instantiated. This is the flame that appears when shooting
    public ParticleSystem emptyShellDrop;//particle system that is placed on gun. Everytime when a pistol fire a bullet, an empty shell is dropped from the right side
    [Space]
    [Space]
    public GameObject fireSound;//sound when weapon fire a bullet
    public GameObject dryfireSound;//sound when weapon is asked to fire a bullet, but there is no availableAmmo
    public GameObject reloadSound;//sound used when reloading

    public Animator anim { get; private set; }




    void Start()
    {

    }//Start




    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            #region Current ammo (clip ammo) is 0. Reload if possible or play dry fire sound
            if (currentAmmo <= 0)
            {
                if (availableAmmo > 0) Reloading();
                else
                {
                    Instantiate(dryfireSound, gameObject.transform.position, gameObject.transform.rotation);//play dry fire sound
                }

            }
            #endregion


            #region Current ammo (clip ammo) is bigger than 0. Deduct one ammo, instantiate bullet, flame, cartrige and sound
            if (currentAmmo > 0)
            {
                currentAmmo--;//............................................................................take one bullet
                Instantiate(bulletPrefab, location_Spawn_bullet.position, location_Spawn_bullet.rotation);//spawn projectile at custom location
                muzzleFlash.Play();//.......................................................................play the particle system that represents the flame
                emptyShellDrop.Play();////..................................................................play the particle system that represents the dropped empty shell
                Instantiate(fireSound, gameObject.transform.position, gameObject.transform.rotation);//.....play fire sound

            }
            #endregion

        }

    }//Update



    private void Reloading()
    {

    }//Reloading


}//END
