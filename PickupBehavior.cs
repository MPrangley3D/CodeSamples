using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    //[NOTES]This logic exists to eventually allow for AI driven NPCs, and selective responses based on the actor type (Player, Enemy, Projectile, Etc) 
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            //[NOTES] The logic for points, or powerups would be placed here
            //[NOTES] Once a UI is implemenmted, this script will communicate with a manager class to pass info about player score and HUD elements
            //[NOTES] There should also be a global inventory manager to handle the ability for the player to upgrade items and add new capabilities (Extra jump, Wall Grab, Shields, etc)
            //[NOTES] I'm currently intentionally delaying the destruction of the gems, to allow for some physics fun
            Destroy(this.gameObject,0.2f);
        }
    }
}
