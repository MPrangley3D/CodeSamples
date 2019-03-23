using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    //[NOTES]The level generation properties are all defined here and sorted so they they appear nicely in the unity editor
    //[NOTES]Note the use of serialized private fields, to allow for access and editing in the editor window for test purposes, while maintaining encapsulation of the variables.
    [Header("[PLAYER PICKER]")]
    [SerializeField]
    private GameObject player;

    [Header("[LEVEL GENERATOR SETTINGS]")]
    [SerializeField]
    private Transform[] startingPositions;
    [SerializeField]
    private GameObject[] rooms;
    [SerializeField]
    private GameObject[] rightEdgeRooms;
    [SerializeField]
    private GameObject[] leftEdgeRooms;
    private int spawnerMoveDirection;
    private float spawnerStepSize = 10f;
    private float spawnTimer;
    [SerializeField]
    private float spawnDelay;

    [Header("[WORLD SIZE]")]
    [SerializeField]
    [Range(-5f, 20f)]
    private float minX;
    [SerializeField]
    [Range(20f, 45f)]
    private float maxX;
    [SerializeField]
    [Range(0f, 505f)]
    private float maxY;
    private float rayDistance = 1f;
    private bool stopGen = false;
    private GameObject lastRoomSpawned;
    private Vector2 checkPreviousDir;
    private string location = "middle";
    private bool retrySpawn = false;
    private bool firstSpawn = true;

    private LayerMask whatIsGround;
    private LayerMask isSpawned;

    private void Start()
    {
        //Set the Raycast Layers
        whatIsGround = LayerMask.GetMask("Ground");
        isSpawned = LayerMask.GetMask("RoomSpawn");

        //Pick an origin (Set via array in inspector) and spawn first room & player there
        int randStartingPos = Random.Range(0, startingPositions.Length);
        this.transform.position = startingPositions[randStartingPos].position;
        SpawnRoom();
        SpawnPlayer();
    }

    private void Update()
    {
        //[NOTES]This was "function-ified" to make it simple to comment on/off during development
        RayDebugger();

        //[NOTES]Stop Gen initializes as false, and controls whether or not the critical path is built
        if (!stopGen)
        {
            if (retrySpawn)
            {
                RetrySpawn();
            }
            
            else if (!retrySpawn)
            {
                Spawner();
            }
        }

        //[NOTES]Once critical path is built, the voids are filled arbitrarily
        else if (stopGen)
        {
            Filler();
        }
        
    }

    //[NOTES]Draws a green ray backwards and a red ray where the level generator is trying to march
    private void RayDebugger()
    {
        Debug.DrawRay(this.transform.position, checkPreviousDir * rayDistance, Color.green);
        if (spawnerMoveDirection == 1 || spawnerMoveDirection == 2 || spawnerMoveDirection == 3)
        {
            Debug.DrawRay(this.transform.position, Vector2.right * rayDistance, Color.red);
        }
        else if (spawnerMoveDirection == 4 || spawnerMoveDirection == 5 || spawnerMoveDirection == 6)
        {
            Debug.DrawRay(this.transform.position, Vector2.left * rayDistance, Color.red);
        }
        else if (spawnerMoveDirection == 7 || spawnerMoveDirection == 8)
        {
            Debug.DrawRay(this.transform.position, Vector2.up * rayDistance, Color.red);
        }
        
    }

    //[NOTES]Destroys and rebuilds a new room
    private void RetrySpawn()
    {
        lastRoomSpawned.GetComponentInChildren<DestroySelf>().DestroyObject();
        Debug.Log("Re-Trying Spawn Path");
        SpawnRoom();
    }

    //[NOTES]spawnTimer exists just to slow down generation for vizualization purposes
    private void Spawner()
    {
        if (spawnTimer <= 0)
        {
            Debug.Log("===Spawn New Room===");
            bool invalidPick = PickDirection();
            if (!invalidPick)
            {
                Debug.Log("=[]Valid Direction Found[]=");
                Move();
                Debug.Log("Spawning New Room");
                SpawnRoom();
            }
            spawnTimer = spawnDelay;
        }
        else
        {
            spawnTimer -= Time.deltaTime; 
        }
    }

    //[NOTES]The final implementation would look something like this:
    /*
    private void Spawner()
    {
        Debug.Log("===Spawn New Room===");
        bool invalidPick = PickDirection();
        if (!invalidPick)
        {
            Debug.Log("=[]Valid Direction Found[]=");
            Move();
            Debug.Log("Spawning New Room");
            SpawnRoom();
        }
    }
    */

    //[NOTES]The code for randomizing the direction began to grow quite large (see below) so I broke that into it's own function.
    private bool PickDirection()
    {
        //picks a direction, factoring for the current column location
        RandomizeNewDirection();

        //Verifies that the path is clear 
        bool check = LookInDirection();

        if (check)
        {
            Debug.Log("[-] Tried & failed to Move: " + spawnerMoveDirection+"Check" + check);
        }
        else
        {
            Debug.Log("[+] Direction is Clear!: " + spawnerMoveDirection+"Check"+check);
        }

        return (check);
    }

    //[NOTES]Picking a new direction is constrained based on current location, to prevent travel outside intended boundaries
    //[NOTES]I wanted some bias for right and left being more common than up, so 1-3 and 4-6 are left and right, while up is only 7 and 8.
    private void RandomizeNewDirection()
    {
        string currentLocation = CheckLocation();

        //Cases where we must go up!
        if (currentLocation == "left" && checkPreviousDir == Vector2.right)
        {
            spawnerMoveDirection = Random.Range(7, 9);
        }
        else if (currentLocation == "right" && checkPreviousDir == Vector2.left)
        {
            spawnerMoveDirection = Random.Range(7, 9);
        }
        //cases where we go left or up
        else if (currentLocation == "right")
        {
            spawnerMoveDirection = Random.Range(4, 9);
        }
        else if (currentLocation == "middle" && checkPreviousDir == Vector2.right)
        {
            spawnerMoveDirection = Random.Range(4, 9);
        }
        //cases where we go right or up
        else if (currentLocation == "left")
        {
            int[] skipLeft = new int[5] { 1, 2, 3, 7, 8 };
            spawnerMoveDirection = skipLeft[Random.Range(1, 5)];
        }

        else if (currentLocation == "middle" && checkPreviousDir == Vector2.left)
        {
            int[] skipLeft = new int[5] { 1, 2, 3, 7, 8 };
            spawnerMoveDirection = skipLeft[Random.Range(1, 5)];
        }
        //we can go left, right, or up
        else if (currentLocation == "middle" && checkPreviousDir == Vector2.down)
        {
            spawnerMoveDirection = Random.Range(1, 9);
        }
        else
        {
            spawnerMoveDirection = Random.Range(1, 9);
        }
    }

    //[NOTES]This just fires a raycast in the selected direction to see if there's anything blocking the path.  
    private bool LookInDirection()
    {
        bool check;
        //Look Right
        if (spawnerMoveDirection == 1 || spawnerMoveDirection == 2 || spawnerMoveDirection == 3)
        {
            check = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y) + Vector2.right * 4.5f, Vector2.right, rayDistance, whatIsGround);
        }
        //Look Left
        else if (spawnerMoveDirection == 4 || spawnerMoveDirection == 5 || spawnerMoveDirection == 6)
        {
            check = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y) + Vector2.left * 4.5f, Vector2.left, rayDistance, whatIsGround);
        }
        //Look up (Dogs can't)
        else if (spawnerMoveDirection == 7 || spawnerMoveDirection == 8)
        {
            check = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y) + Vector2.up * 4.5f, Vector2.up, rayDistance, whatIsGround);
        }
        else
        {
            check = false;
        }
        return (check);
    }

    //[NOTES]This moves the spawner node
    private void Move()
    {
        if (spawnerMoveDirection == 1 || spawnerMoveDirection == 2 || spawnerMoveDirection == 3)
        {
            StepRight();
        }

        else if (spawnerMoveDirection == 4 || spawnerMoveDirection == 5 || spawnerMoveDirection == 6)
        {
            StepLeft();
        }

        else if (spawnerMoveDirection == 7 || spawnerMoveDirection == 8)
        {
            StepUp();
        }
    }

    //[NOTES]I feel like these stepping functions repeat themselves a little much, but I like the readability of this format
    //[NOTES]MY first inclination was to make this one function, pass it strings, but I didn't like how that solution was looking, so I went with this for now.
    private void StepRight()
    {
        if (this.transform.position.x < maxX)
        {
            Vector2 newPos = new Vector2(transform.position.x + spawnerStepSize, transform.position.y);
            this.transform.position = newPos;
            checkPreviousDir = Vector2.left;
        }
        else
        {
            StepUp();
        }
    }

    private void StepLeft()
    {
        if (this.transform.position.x > minX)
        {
            Vector2 newPos = new Vector2(transform.position.x - spawnerStepSize, transform.position.y);
            this.transform.position = newPos;
            checkPreviousDir = Vector2.right;
        }
        else
        {
            StepUp();
        }
    }

    private void StepUp()
    {
        if (this.transform.position.y < maxY)
        {
            Vector2 newPos = new Vector2(transform.position.x, transform.position.y + spawnerStepSize);
            this.transform.position = newPos;
            checkPreviousDir = Vector2.down;
        }
        else
        {
            stopGen = true;
        }
    }

    //[NOTES]This instantiates the new room at the location, then uses a coroutine to validate that a path back to the parent room exists
    private void SpawnRoom()
    {
        string spawnLocation = CheckLocation();

        if (spawnLocation == "left")
        {
            lastRoomSpawned = Instantiate(leftEdgeRooms[Random.Range(0, leftEdgeRooms.Length)], transform.position, Quaternion.identity);
        }
        else if (spawnLocation == "right")
        {
            lastRoomSpawned = Instantiate(rightEdgeRooms[Random.Range(0, rightEdgeRooms.Length)], transform.position, Quaternion.identity);
        }
        else if (spawnLocation == "middle")
        {
            lastRoomSpawned = Instantiate(rooms[Random.Range(0, rooms.Length)], transform.position, Quaternion.identity);
        }

        Debug.Log("New Room Named:  " + lastRoomSpawned);
        
        StartCoroutine("ValidateNewRoom");
    }

    //[NOTES]This fires a raycast backwards, and then returns a boolean that will inform the spawn vs retrySpawn logic that triggers in the Update method.
    IEnumerator ValidateNewRoom()
    {
        yield return new WaitForSeconds(0f);
        
        bool checkPrev = Physics2D.Raycast(new Vector2(this.transform.position.x,this.transform.position.y)+checkPreviousDir*4.5f, checkPreviousDir, rayDistance, whatIsGround);
        Debug.DrawRay(this.transform.position, checkPreviousDir * rayDistance, Color.blue);
        Debug.Log("Validating new room - True should destroy " + checkPrev);
        if (checkPrev)
        {
            Debug.Log("Call Destroy this: " + lastRoomSpawned.name);
        }

        //[NOTES]This handles the edge case where a "previous room" does not exist
        if (firstSpawn)
        {
            retrySpawn = false;
            firstSpawn = false;
        }
        //[NOTES]firstSpawn is never switched back to True, so this executes for all subsequent
        else
        {
            retrySpawn = checkPrev;
        }
        
    }

    private void SpawnPlayer()
    {
        Debug.Log("Player Spawned");
        player.transform.position = this.transform.position;
    }

    //Returns the current position of the room spawner
    private string CheckLocation()
    {
        if (this.transform.position.x >= maxX)
        {
            location = "right";
        }
        else if (this.transform.position.x <= minX)
        {
            location = "left";
        }
        else
        {
            location = "middle";
        }
        return location;
    }

    //Fills the voids
    //[NOTES]This is the final step of the generation, to fill all non-critical path voids
    private void Filler()
    {
        float yIter = 5;
        float xIter = minX;
        while (xIter <= maxX)
        {
            while (yIter <= maxY)
            {
                BuildColumn(xIter, yIter);
                yIter += 10;
            }
            yIter = 5;
            xIter += 10;
        }
    }

    //[NOTES]You can see it loops through the structure column by column, from bottom left to top right, building the rooms in voids
    void BuildColumn(float buildX, float buildY) 
    {
        Vector2 newPos = new Vector2(buildX, buildY);
        this.transform.position = newPos;
        bool check = CheckExisting();
        if (!check)
        {
            Instantiate(rooms[Random.Range(0, rooms.Length)], transform.position, Quaternion.identity);
        }
    }

    bool CheckExisting()
    {
        bool check = Physics2D.Raycast(this.transform.position, Vector2.right, 1.0f, isSpawned);
        return check;
    }
}
