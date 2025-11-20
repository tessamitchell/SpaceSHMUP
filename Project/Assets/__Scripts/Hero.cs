using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class Hero : MonoBehaviour
{
    static public Hero S { get; private set; }  // Singleton property    // a
 
    [Header("Inscribed")]
    // These fields control the movement of the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Dynamic")][Range(0, 4)]                                          // b
    private float _shieldLevel = 2;


    [Tooltip("This field holds a reference to the last triggering GameObject")]
    private GameObject lastTriggerGo = null;
    // Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();                                // a
    // Create a WeaponFireDelegate event named fireEvent.
    public event WeaponFireDelegate fireEvent;


    void Awake()
    {
        if (S == null)
        {
            S = this; // Set the Singleton only if it’s null                  // c
        }
       else
       {
        Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
       }
        //fireEvent += TempFire;
        ClearWeapons();
        weapons[0].SetType(eWeaponType.swivel);
    }
 
    void Update()
    {
                 // Pull in information from the Input class
        float hAxis = Input.GetAxis("Horizontal");                            // d
        float vAxis = Input.GetAxis("Vertical");                              // d

         // Change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

         // Rotate the ship to make it feel more dynamic                       // e
        transform.rotation = Quaternion.Euler(vAxis * pitchMult, hAxis * rollMult, 0);


        // Use the fireEvent to fire Weapons when the Spacebar is pressed.
        if (Input.GetAxis("Jump") == 1 && fireEvent != null)
        {               // d
            fireEvent();                                                     // e
        }
    }

    //void TempFire()
    //{                                                        // b
    //    GameObject projGO = Instantiate<GameObject>(projectilePrefab);
    //    projGO.transform.position = transform.position;
    //    Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
    //    ProjectileHero proj = projGO.GetComponent<ProjectileHero>();         // h
    //    proj.type = eWeaponType.blaster;
    //    float tSpeed = Main.GET_WEAPON_DEFINITION(proj.type).velocity;
    //    rigidB.velocity = Vector3.up * tSpeed;
    //}


    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;                    // a
        GameObject go = rootT.gameObject;
        //Debug.Log("Shield trigger hit by: " + go.gameObject.name);

        if (go == lastTriggerGo) return;                                    // c
        lastTriggerGo = go;                                                   // d
        Enemy enemy = go.GetComponent<Enemy>();                               // e
        PowerUp pUp = go.GetComponent<PowerUp>();
        if (enemy != null)
        {  // If the shield was triggered by an enemy
            shieldLevel--;        // Decrease the level of the shield by 1
            Destroy(go);          // … and Destroy the enemy                  // f
        }
        else if (pUp != null)
        { // If the shield hit a PowerUp
            AbsorbPowerUp(pUp);
        }  // … absorb the PowerUp
        else
        {
            Debug.LogWarning("Shield trigger hit by non-Enemy: " + go.name);    // g
        }
    }

    public void AbsorbPowerUp(PowerUp pUp)
    {
        Debug.Log("Absorbed PowerUp: " + pUp.type);                         // b
        switch (pUp.type)
        {
            case eWeaponType.shield:                                              // a 
                shieldLevel++;
                break;
            default:                                                             // b
                if (pUp.type == weapons[0].type)
                { // If it is the same type     // c
                    Weapon weap = GetEmptyWeaponSlot();
                    if (weap != null)
                    {
                    // Set it to pUp.type
                        weap.SetType(pUp.type);
                    }
                }
                else
                { // If this is a different weapon type                   // d
                    ClearWeapons();
                    weapons[0].SetType(pUp.type);
                }
            break;
         
        // Leave this switch block empty for now.

    }
        pUp.AbsorbedBy( this.gameObject );
    }

    public float shieldLevel
    {
        get { return (_shieldLevel); }                                      // b
        private set
        {                                                         // c
            _shieldLevel = Mathf.Min(value, 4);                             // d
            // If the shield is going to be set to less than zero…
            if (value < 0)
            {                                                  // e
                Destroy(this.gameObject);  // Destroy the Hero
                Main.HERO_DIED();
            }
        }
    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == eWeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(eWeaponType.none);
        }
    }

}
