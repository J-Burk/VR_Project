using UnityEngine;

/*Class handling the Sharpening system  */
public class SharpeningSystem : MonoBehaviour
{

    System.Random random = new System.Random();

    //UI Elements
    public GameObject indicator;
    public GameObject[] routes;

    public GameObject failIcon;

    //Timer controlling display of the fail icon
    public float failTimer = 0;
    const float failTime = 5;
    public bool failActive = false;

    // Everything sharpening related happens on a flat plane
    const int zValue = 0;

    // Difficulty level; determines speed and acceptanceRadius
    private int lvl = 0;

    //Current shaprening pattern
    private Vector2[] pattern;

    //Current shaprening pattern
    private Vector2[][] patterns;

    // Array of Coordinate Points between which the sharpening pattern will move (From Point 0 to 1 to 2 to 1 to 0 to 1...)
    private readonly Vector2[] daggerPattern =
    {
        new Vector2(0.45f, 0),
        new Vector2(-0.45f, 0)
    };

    private readonly Vector2[] axePattern =
    {
        new Vector2(0.15f, 2.8f),
        new Vector2(-0.15f, 2.8f),
        new Vector2(0.15f, -2.8f),
        new Vector2(-0.15f, -2.8f),
        
    };

    private readonly Vector2[] swordPattern =
    {
        new Vector2(0.138f, -2.46f),
        new Vector2(0.231f, -0.51f),
        new Vector2(0.152f, 0.7f),
        new Vector2(0.039f, -1.01f),
        new Vector2(-0.053f, 0.42f),
        new Vector2(0.047f, 2.58f),
        new Vector2(-0.039f, 3.84f),
        new Vector2(-0.14f, 2.1f)
    };

    // Weapon Type of the weapon currently being sharpened
    private int weaponType;

    // Last Point visited (Array index)
    private int currentOrigin = 0;

    // Next point to visit (Array index)
    private int currentGoal = 1;

    // Determines weather Origin and Goal are changed positively or negatively (value is always 1 or -1)
    private int patternDirection = 1;

    // Path between origin and goal
    private Vector2 currentVector;

    // Speed at which this should happen (per second)
    private readonly float[] speed = {0.25f, 0.5f, 1.5f, 1.75f};

    // Current point on the pattern, the weapon should be at
    private Vector2 currentPoint = new Vector2(0, 0);

    // Position the player is currently holding the weapon at
    public Vector2 playerPos = new Vector2(0, 0);

    //If the weapon is currently colliding with the indicator
    private bool colliding = false;

    // Amount of Time the player spends in- and outside the acceptanceRadius, used to calculate the overall successRate
    private float[] successRate = { 0, 0 };

    // Successrate needed to pass the game
    const double successThreshhold = 0.2;

    // Length of a game
    private readonly int[] gameLength = { 15, 20, 25, 30 };

    // How long the current game has been going for
    private float currentTime = 0;

    // Is a game currently running
    private bool gameRunning = false;

    //Reference to the weapon being sharpened
    public GameObject weapon;

    void Start()
    {
        GameEvents.instance.jPressed += desktopControl;
        GameEvents.instance.sharpToggle += reactToCollision;
        for(int i = 0; i < routes.Length; i++)
        {
            routes[i].SetActive(false);
        }
        patterns = new Vector2[3][] { daggerPattern, axePattern, swordPattern };
    }

    void Update()
    {
        // Fail Icon Handling
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

        if (gameRunning)
        {
            float interval = Time.deltaTime;

            //Checks if the player is in- or outside the accepted range
            if(colliding)
            {
                successRate[0] += interval;
            } else
            {
                successRate[1] += interval;
            }
            GameEvents.instance.MakeProgressSharpen((float)successRate[0] / (float)gameLength[lvl]);

            //Checks if the game is over
            currentTime += interval;

            if(currentTime >= gameLength[lvl])
            {
                endGame();


            } else
            {
                //Moves ahead the currentPoint according to the set speed
                currentPoint.x += currentVector.x * speed[lvl] * interval;
                currentPoint.y += currentVector.y * speed[lvl] * interval;

                //Checks if the currentPoint has reached the goal (indicated by vector direction)
                if(! sameDirection(currentVector, new Vector2(pattern[currentGoal].x - currentPoint.x, pattern[currentGoal].y - currentPoint.y)))
                {
                    
                    //Changes origin and goal accordingly and flips patternDirection if necessary
                    
                    currentOrigin = currentGoal;
                    currentGoal += patternDirection;
                    
                    if ( (patternDirection == 1 && currentGoal == pattern.Length) || (patternDirection == -1 && currentGoal == -1))
                    {
                        patternDirection *= -1;
                        currentGoal = currentOrigin + patternDirection;
                    } else
                    {
                        //On the hardest pattern theres a random chance to switch move directions
                        if(weaponType == 2 && (Random.Range(0f, 1f) <= 0.2f))
                        {
                            patternDirection *= -1;
                        }
                    }
                    
                    currentVector = pattern[currentGoal] - pattern[currentOrigin];
                }
                

                indicator.transform.localPosition = new Vector3(currentPoint.x, currentPoint.y, zValue);

            }
        }
    }

    /*
     * Handles collision of the indicator with a weapon and starts the game/toggles the sharpen state
     * @param weapon - weapon the indicator is currently colliding with
     * @param type - Type of the Weapon
     * @param enter - wether its an enter or exit collision
     */
    public void reactToCollision(GameObject weapon, int type, bool enter)
    {
        if (enter)
        {
            if (!gameRunning)
            {
                //Ascends to the Highest Level of the weapon prefab, containing the information
                int counter = 0;
                while (weapon.transform.parent != null && weapon.tag != "Weapon" && counter < 10)
                {
                    weapon = weapon.transform.parent.gameObject;
                    counter++;
                }
                this.weapon = weapon;
                WeaponStats stats = weapon.transform.GetComponent<WeaponStats>();
                startGame(stats.oreType, stats.weaponType);
            }
            else
            {
                colliding = true;
            }
        } else if (gameRunning)
        {
            colliding = false;
            
        }
        
    }

    /*
     * Function allowing toggling the sharpen state with "J" on Desktop
     */
    public void desktopControl()
    {
        colliding = !colliding;
    }

    /*
     * Sets all values to default and starts the game (if the game is not already running)
     * @param difficulty - difficulty level of the game to be played (0-3)
     * @param route - route the game should use (0-2)
     * @return bool - was the game started successfully
     */
    public bool startGame(int difficulty, int route)
    {
        if(!gameRunning)
        {

            for(int i = 0; i < routes.Length; i++)
            {
                routes[i].SetActive(i == route);
            }

            pattern = patterns[route];

            lvl = difficulty;
            currentTime = 0;
            successRate[0] = 0;
            successRate[1] = 0;
            currentOrigin = 0;
            currentGoal = 1;
            currentPoint = pattern[currentOrigin];
            currentVector = pattern[currentGoal] - pattern[currentOrigin];
            patternDirection = 1;
            gameRunning = true;
            weaponType = route;
            return true;
        }
        return false;
    }

    /* 
     * Ends a game if it is currently running
     * @return bool - true if the player succeeded, false if they didnt or there was no ongoing game
     */
    public bool endGame()
    {
        if (gameRunning)
        {
            gameRunning = false;

            //Checks for success or failure
            if ((successRate[0]/(successRate[0]+(successRate[1])) >= successThreshhold))
            {
                //Ascends to the Highest Level of the weapon prefab, containing the information
                int counter = 0;
                while (weapon.transform.parent != null && weapon.tag != "Weapon" && counter < 10)
                {
                    weapon = weapon.transform.parent.gameObject;
                    counter++;
                }
                WeaponStats stats = weapon.GetComponent<WeaponStats>();
                stats.sharpeningScore =  (successRate[0] / (successRate[0] + (successRate[1])));
                GameEvents.instance.PlaySound("Success", this.gameObject.transform.position);
                stats.polished();
                return true;
            }
            else
            {
                GameEvents.instance.PlaySound("Failure", this.gameObject.transform.position);
                failActive = true;
                failIcon.SetActive(true);
            }
        }
        return false;
    }

    /**
     * Checks if 2 Vectors point in the same Direction
     * @param a - Vector 1
     * @param b - Vector 2
     * @return bool - if 2 Vectors point in the same Direction
     */
    private bool sameDirection(Vector2 a, Vector2 b)
    {

        Vector2 normA = a.normalized;
        Vector2 normB = b.normalized;

        return (Vector2.Dot(normA, normB) > 0);
    }


}
