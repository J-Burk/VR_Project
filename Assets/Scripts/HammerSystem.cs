using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using UnityEditor.Build;

/*Class handling the Hammering system  */
public class HammerSystem : MonoBehaviour
{

    System.Random random = new System.Random();

    public GameObject anvil;

    //UI Elements
    public GameObject uiCircles;

    public GameObject fireIcon;
    public GameObject snowIcon;
    public GameObject failIcon;

    //Timer controlling display of the fail icon
    private float failTimer = 0;
    const float failTime = 5;
    private bool failActive = false;

    //Timer controlling rhythm indicators
    private float rhythmTimer = 0;
    private float[] rhythmTime = { 0.5f, 0.3f, 0.15f };
    private int rhythmActive = -1;

    public Anvil anvilClass;

    // Difficulty level; determines circleTime, interval and circlesToSpawn
    public int lvl = 0;

    //Weapon and Ore Type of the weapon currently in the making, based on ingot count and type
    public int weaponType = 0;
    public int oreType = 0;

    // Dimensions and Position of the interactive Area of the hammering game 
    private float fieldCenterX;
    private float fieldCenterY;
    private float fieldCenterZ;
    private Vector3 fieldCenter;
    const float fieldWidth = 0.8f;
    const float fieldHeight = 0.25f;

    // Radius of a timing Circle
    const float circleRadius = 0.05f;

    //Distance between circles
    const float circleDistance = 0.175f;

    // Radius of the hit window
    const float hitRadius = 0.13f;

    // Amount of Time between the appearance and dissappearance of a timing circle
    private readonly int[] circleTime = { 5, 3, 1 };

    // Window before the circle disappears during which a hit will be considered successfull
    const double hitTiming = 0.5f;

    // Time between appearance of circles
    private readonly float[] circleInterval = { 2, 1, 0.75f };

    // Amount of circles spawned per game
    private readonly int[] circlesToSpawn = { 10, 25, 40 };

    // Amount of circles spawned
    private int circlesSpawned = 0;

    // Amount of circles hit
    private int progress = 0;

    // Percentage of Circles that need to be hit
    const double demandedHitRate = 0.2;

    // A TimingCircle has a position and a life timer
    private class timingCircle
    {
        public GameObject circleObject;

        // Position of the Circle (Y will always be fieldCenterY)
        public float positionX;
        public float positionZ;

        // Remaining life time of the Circle
        public float lifeTime;
    }

    // List of all active timing circles
    private List<timingCircle> timingCircles = new List<timingCircle>();

    // Is a game currently running
    private bool gameRunning = false;

    // Time until next circle is spawned
    private float circleSpawnCooldown = 0;

    // Maximum and minimum size of the timing circle between which the current size is interpolated
    const float innerCircleMinSize = 0.06f;
    const float innerCircleMaxSize = 0.111f;

    private bool paused = false;

    void Start()
    {

        GameEvents.instance.hPressed += hitMostRecent;
        GameEvents.instance.hammerHit += registerHit;
        GameEvents.instance.hammerStarted += startGame;

        fieldCenterX = anvil.transform.position.x;
        fieldCenterY = anvil.transform.position.y + 0.67f;
        fieldCenterZ = anvil.transform.position.z;
        fieldCenter = new Vector3(fieldCenterX, fieldCenterY, fieldCenterZ);

    }

    // Update is called once per frame
    void Update()
    {
        //Temperatur Icon handling
        if (!anvilClass.empty())
        {
            displayTemperature();
        }
        else
        {
            snowIcon.SetActive(false);
            fireIcon.SetActive(false);
        }

        //Fail Icon handling
        if (failActive)
        {
            failTimer += Time.deltaTime;
            if (failTimer > failTime)
            {
                failIcon.SetActive(false);
                failTimer = 0;
                failActive = false;
            }
        }

        //Fail Icon handling
        if (rhythmActive > 0)
        {
            rhythmTimer += Time.deltaTime;
            if (rhythmTimer > rhythmTime[lvl])
            {
                GameEvents.instance.PlaySound("Rhythm", this.gameObject.transform.position);
                rhythmTimer = 0;
                rhythmActive--;
            }
        }

        if (gameRunning && !paused)
        {
            //Reduce the timers of all circles and remove them from the list if they expire
            for (int i = 0; i < timingCircles.Count; i++)
            {
                //Interpolate timing circle size
                timingCircles[i].lifeTime -= Time.deltaTime;
                float t = Mathf.Clamp01(timingCircles[i].lifeTime / circleTime[lvl]);
                float interpolatedSize = Mathf.Lerp(innerCircleMinSize, innerCircleMaxSize, t);
                SpriteRenderer innerCircle =
                    timingCircles[i].circleObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
                innerCircle.transform.localScale = new Vector3(interpolatedSize, interpolatedSize, 1);

                if((timingCircles[i].lifeTime <= 4.0f*rhythmTime[lvl] + hitTiming) && rhythmActive == -1)
                {
                    Debug.Log("Rhythm Started");
                    rhythmActive = 3;
                }

                //Circle runs out
                if (timingCircles[i].lifeTime <= 0)
                {
                    Destroy(timingCircles[i].circleObject);
                    timingCircles.RemoveAt(i);
                    rhythmActive = -1;

                    //If the last circle in the list ran out and no more circles are being spawned, the game ends
                    if (i == timingCircles.Count && circlesSpawned == circlesToSpawn[lvl])
                    {
                        endGame();
                    }

                    i--;
                }
            }

            //Spawn a new circle every interval
            circleSpawnCooldown -= Time.deltaTime;
            if (circleSpawnCooldown <= 0 && circlesSpawned < circlesToSpawn[lvl])
            {
                createNewCircle();
                circleSpawnCooldown = circleInterval[lvl];
            }

            //Checks for appropriate temperature
            if (!checkTemperature())
            {
                Debug.Log("Temperature Fail");
                paused = true;
            }
        }
    }

    /*
     * Sets all values to default and starts the game (if the game is not already running). Also toggles the paused state
     */
    private void startGame()
    {
        if (paused)
        {
            paused = !paused;
        }
        if (!gameRunning && anvilClass.valid() && checkTemperature())
        {
            int level = calculateLevel();
            if(level != -1 && oreType != -1)
            {
                Debug.Log("Level: " + level);
                Debug.Log("Hammer Game Start");
                timingCircles = new List<timingCircle>();
                lvl = level;
                circlesSpawned = 0;
                progress = 0;
                GameEvents.instance.MakeProgressHammer(progress);
                circleSpawnCooldown = 0;
                gameRunning = true;
                GameEvents.instance.PlaySound("HammerOn", this.gameObject.transform.position);
            }
        }
    }

    /* 
     * Ends a game if it is currently running
     * @return bool - true if the player succeeded, false if they didnt or there was no ongoing game
     */
    private bool endGame()
    {
        if(gameRunning)
        {
            for (int i = 0; i < timingCircles.Count; i++)
            {
                Destroy(timingCircles[i].circleObject);
            }
            gameRunning = false;
            Debug.Log("Hammer Game Endm \n Score: " + progress);

            //Checks if the game has been passed or not
            if (progress >= circlesToSpawn[lvl] * demandedHitRate)
            {
                GameEvents.instance.PlaySound("Success", this.gameObject.transform.position);
                GameEvents.instance.HammerSuccess(weaponType, oreType, ((float)progress/10.0f));
                return true;
            } else
            {
                GameEvents.instance.PlaySound("Failure", this.gameObject.transform.position);
                GameEvents.instance.HammerFail();
                failActive = true;
                failIcon.SetActive(true);
            }

            GameEvents.instance.PlaySound("HammerOff", this.gameObject.transform.position);
        }
        return false;
    }

    /*
     * Creates a new randomized timing Circle and adds it to the list
     */
    private void createNewCircle()
    {
        // Creates a new Random Position inside the field for the circle
        float minValue = 0 - fieldWidth / 2 + circleRadius;
        float maxValue = 0 + fieldWidth / 2 - circleRadius;
        float newX = (float)(random.NextDouble() * (maxValue - minValue) + minValue);

        minValue = 0 - fieldHeight / 2 + circleRadius;
        maxValue = 0 + fieldHeight / 2 - circleRadius;
        float newZ = (float)(random.NextDouble() * (maxValue - minValue) + minValue);

        // Repeats the process until the new Circle doesnt overlap with current ones
        int loops = 0;
        while ( doCirclesOverlap(newX, newZ) && loops < 100 )
        {
            minValue = 0 - fieldWidth / 2 + circleRadius;
            maxValue = 0 + fieldWidth / 2 - circleRadius;
            newX = (float)(random.NextDouble() * (maxValue - minValue) + minValue);

            minValue = 0 - fieldHeight / 2 + circleRadius;
            maxValue = 0 + fieldHeight / 2 - circleRadius;
            newZ = (float)(random.NextDouble() * (maxValue - minValue) + minValue);

            loops++;
        }

        timingCircles.Add(new timingCircle()
        {
            circleObject = Instantiate(uiCircles, fieldCenter + new Vector3(newX, 0, newZ), uiCircles.transform.rotation),
            positionX = newX + fieldCenterX,
            positionZ = newZ + fieldCenterZ,
            lifeTime = circleTime[lvl]

        });
        circlesSpawned++;
    }

    /*
     * Checks if a new circle would overlap with any already existing ones
     * @param newX - X-Coordinate of the new circle
     * @param newZ - Z-Coordinate of the new circle
     * @return bool - if the new circle would overlap with any already existing ones
     */
    private bool doCirclesOverlap(float newX, float newZ) 
    {
        bool overlapping = false;

        for (int i = 0; i < timingCircles.Count && !overlapping; i++)
        {
            float vectorX = (newX + fieldCenterX) - timingCircles[i].positionX;
            float vectorY = (newZ + fieldCenterZ)- timingCircles[i].positionZ;
            double distance = Math.Sqrt(vectorX * vectorX + vectorY * vectorY);
            Debug.Log("Distance: " + distance);

            overlapping = distance <= circleDistance;

        }

        return overlapping;
    }

    /*
     * Checks if a collision hit a circle and if it did so at the proper time
     * @param hitCircle - Supposed Hit Circle which is being hit
     * @return bool - if a collision hit a circle and if it did so at the proper time
     */
    public void registerHit(GameObject hitCircle)
    {
        Debug.Log("RegisterAnyHitCalled");
        if (timingCircles.Count != 0)
        {
            Debug.Log("Circle Life Time = " + timingCircles[0].lifeTime);
            if (timingCircles[0].lifeTime <= hitTiming && hitCircle == timingCircles[0].circleObject)
            {
                GameEvents.instance.PlaySound("HammerSuccess", this.gameObject.transform.position);
                Debug.Log("Hit at the right timing");
                progress++;
                Debug.Log("Progress Made: " + (float)progress / (float)circlesToSpawn[lvl]);
                GameEvents.instance.MakeProgressHammer((float)progress / (float)circlesToSpawn[lvl]);
            } else
            {
                GameEvents.instance.PlaySound("HammerFail", this.gameObject.transform.position);
            }
            Destroy(timingCircles[0].circleObject);
            timingCircles.RemoveAt(0);
            rhythmActive = -1;

            //If a circle was hit and no more will spawn, the game ends
            if (timingCircles.Count == 0 && circlesSpawned == circlesToSpawn[lvl])
            {
                endGame();
            }
        }
    }

    /*
     * Function that enables the entire Hammer Game to be played with one Button ("H") on Desktop
     */
    public void hitMostRecent()
    {
        if (paused)
        {
            paused = false;
        }
        if(!gameRunning)
        {
            startGame();
        } else
        {
            if (timingCircles.Count != 0)
            {

                registerHit(timingCircles[0].circleObject);
            }
        }   
    }

    /*
     * Checks if the temperature of all Ingots on the anvil is acceptable
     * @return bool - is the temperature of all Ingots on the anvil acceptable
     */

    //TODO states zu array?
    public bool checkTemperature()
    {
        bool allGood = true;
        for (int i = 0; i < anvilClass.states.Length && anvilClass.states[i] != null && allGood; i++) {
            allGood = anvilClass.states[i].GetComponent<Material>().temperatureOk();
        }
        

        return allGood;
    }

    /*
     * Displays/Removes temperature Icon from the anvil
     */
    public void displayTemperature()
    {
        int tempStatus = 2;
        Material script;
        for (int i = 0; i < anvilClass.states.Length && tempStatus == 2; i++) {
            if(anvilClass.states[i] != null)
            {
                tempStatus = anvilClass.states[i].GetComponent<Material>().hotOrCold();
            }
        }
        
        if (tempStatus == 0)
        {
            fireIcon.SetActive(true);
            snowIcon.SetActive(false);
        }
        if (tempStatus == 1)
        {
            fireIcon.SetActive(false);
            snowIcon.SetActive(true);
        }
        if (tempStatus == 2)
        {
            fireIcon.SetActive(false);
            snowIcon.SetActive(false);
        }
    }

    /*
     * Calculates difficulty level, weapon type and weapon ore type based on Ore Type and amount of Ore
     */
    public int calculateLevel()
    {
        int numberOfOre = 0;
        float materialValue = 0;

        oreType = -1;
        
        int goldValue = 1;
        int steelValue = 2;
        int fireValue = 3;
        int ironValue = 4;

        for (int i = 0; i < anvilClass.states.Length; i++)
        {
            if(anvilClass.states[i] != null)
            {
                numberOfOre++;
                switch (anvilClass.states[i].name.Substring(0, 1))
                {
                    case "F":
                        materialValue += fireValue;
                        break;
                    case "G":
                        materialValue += goldValue;
                        break;
                    case "I":
                        materialValue += ironValue;
                        break;
                    case "S":
                        materialValue += steelValue;
                        break;
                }
            }
        }

        //Determines Weapon Type, Weapon Ore Type and Lvl
        oreType = (int)(materialValue / numberOfOre) - 1;
        materialValue /= numberOfOre;
        int maxScore = 10;
        int minScore = 2;

        Debug.Log("MaterialValue: " + materialValue);
        Debug.Log("NumberOfOre: " + numberOfOre);


        float score = materialValue + numberOfOre;

        float inputInRange = score - 2;
        float inputRatio = inputInRange / 8.0f; 
        score = inputRatio * 2.0f;
        Debug.Log("Score: " + score);
        int lvl = (int) Math.Round(score);
        if(numberOfOre%2 == 1 )
        {
            weaponType = -1;
        } else
        {
            weaponType = (numberOfOre - 1) / 2;
        }
        

        return lvl;
    }




}
