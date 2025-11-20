using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;


[RequireComponent(typeof(BoundsCheck))]
public class ProjectileHero : MonoBehaviour
{
    public float projectileSpeed = 40;
    private BoundsCheck bndCheck;
    private Renderer rend;

    [Header("Dynamic")]
    public Rigidbody rigid;
    [SerializeField]                                                         // a
    private eWeaponType _type;                                               // b
     
    // This public property masks the private field _type
    public eWeaponType type
    {                                              // c
        get { return (_type); }
        set { SetType(value); }                  
    }

    void Awake()
    {
         bndCheck = GetComponent<BoundsCheck>();
         rend = GetComponent<Renderer>();
         rigid = GetComponent<Rigidbody>();
         vel = Vector3.zero;
        
    }

    void Update()
    {
        if (bndCheck.LocIs(BoundsCheck.eScreenLocs.offUp))
        {          // a
            Destroy(gameObject);
        }
        if (_type == eWeaponType.phaser)
        {
            MovePhaser();
        }
        else if (_type == eWeaponType.missile)
        {
            MoveMissile();
        }

    }

    //private void FixedUpdate()
    //{
    //    Vector3 pos=transform.position;
    //    transform.position=pos+(Vector3.up * projectileSpeed * Time.deltaTime);
    //}

    public void SetType(eWeaponType eType)
    {                               // e
        _type = eType;
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(_type);
        rend.material.color = def.projectileColor;
    }


    public Vector3 vel
    {
        get { return rigid.velocity; }
        set { rigid.velocity = value; }
    }


    [Tooltip("# of seconds for a full sine wave")]                            // b 
    public float waveFrequency = 1;
    [Tooltip("Sine wave width in meters")]
    public float waveWidth = 2;
    [Tooltip("Amount the ship will roll left and right with the sine wave")]
    public float waveRotY = 45;

    public float x0; // The initial x value of pos
    private float birthTime;
    
    private float speed = 10f;
    public float phaserVel { get { return speed; } set { speed = value; } }
    

    public void MovePhaser()
    {                                             // d
                                                  // Because pos is a property, you can’t directly set pos.x
                                                  //   so get the pos as an editable Vector3
        Vector3 tempPos = pos;
        // theta adjusts based on time
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        // rotate a bit about y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        // base.Move() still handles the movement down in y
        Vector3 tempPos2 = pos;
        tempPos.y += speed * Time.deltaTime;
        pos = tempPos;                                                         // e

        //print( bndCheck.isOnScreen ); // Leave this line commented out     // f
    }


    public Vector3 pos
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }

    public GameObject missileTarget;

    public void MoveMissile()
    {
        if (missileTarget != null) { transform.position = Vector3.Lerp(transform.position, missileTarget.transform.position, .01f); }
        else { Destroy(gameObject); }
        
    }

}
