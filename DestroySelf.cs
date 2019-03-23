using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[NOTES]This was broken into it's own class to allow me to expand on the logic later
//[NOTES]For example, possibly maintain a cache of failed room shapes, which could be excluded in future attempts.
public class DestroySelf : MonoBehaviour
{
    public void DestroyObject()
    {
        Debug.Log("Destroy this: "+this.name);
        Destroy(gameObject);
    }
}
