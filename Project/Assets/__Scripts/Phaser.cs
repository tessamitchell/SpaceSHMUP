using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phaser : ProjectileHero
{


    [Tooltip("# of seconds for a full sine wave")]                            // b 
    public float waveFrequency = 2;
    [Tooltip("Sine wave width in meters")]
    public float waveWidth = 4;
    [Tooltip("Amount the ship will roll left and right with the sine wave")]
    public float waveRotY = 45;

    private float x0; // The initial x value of pos
    private float birthTime;

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
        MovePhaser();                                                          // e

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
}
