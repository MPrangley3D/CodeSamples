using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SpawnObject : MonoBehaviour
{
    //[NOTES]The level generation properties are all defined here and sorted so they they appear nicely in the unity editor
    //[NOTES]Note the use of serialized private fields, to allow for access and editing in the editor window for test purposes, while maintaining encapsulation of the variables.
    [SerializeField]
    private GameObject[] objects;
    [Range(1f, 100f)]
    [SerializeField]
    private float spawnRate;

    private void Start()
    {
        RandomizeSpawn();
    }

    //[NOTES]If the spawn node succeeds, a random object will be spawned
    //[NOTES]This is used for random terrain as well as random items & pickups.
    //[NOTES]Specify the object list in the inspector at the spawn node
    private void RandomizeSpawn()
    {
        int roll = Random.Range(1, 101);

        if (roll < spawnRate)
        {
            int rand = Random.Range(0, objects.Length);
            GameObject newPickup = Instantiate(objects[rand], transform.position, Quaternion.identity);
            newPickup.transform.parent = this.gameObject.transform;
        }
    }
}