using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*Class handling the Orders given to the player */
public class OrderSystem : MonoBehaviour
{
    System.Random random = new System.Random();

    /*UI Elements */
    public GameObject satisfactionFill;
    public GameObject satisfactionImage;
    public TextMeshPro satisfactionText;
    public Canvas boardCanvas;
    public GameObject orderPrefab;

    public Sprite dagger;
    public Sprite axe;
    public Sprite sword;

    //TODO
    public Sprite[] weaponSprites;

    public Sprite smile;
    public Sprite neutral;
    public Sprite frown;
    public TextMeshPro dayCountText;
    public TextMeshProUGUI gameOverText;

    /**
    public Color[] typeColors = {
      new Color(1.0f, 0.78f, 0.09f, 1.0f),
      new Color(0.43f, 0.48f, 0.37f, 1.0f),
      new Color(0.60f, 0.23f, 0.07f, 1.0f),
      new Color(0.28f, 0.31f, 0.33f, 1.0f)
    };

    public Color color1 = new Color(1.0f, 0.78f, 0.09f, 1.0f);
    public Color color2 = new Color(0.43f, 0.48f, 0.37f, 1.0f);
    public Color color3 = new Color(0.60f, 0.23f, 0.07f, 1.0f);
    public Color color4 = new Color(0.28f, 0.31f, 0.33f, 1.0f);
    */

    private bool gameRunning = false;
    private bool gamePaused = false;

    /*Current customer satisfaction (HP of the player), increased by handing in Orders in time */
    private float satisfaction = 0.5f;

    private int difficultyLevel = 1;

    //TODO ?
    /*Current day (Score of the run) */
    private int dayCount = 0;

    /*Amount of different Materials in the Game */
    const int materialCount = 4;

    /* Amount of different Weapon Types in the Game */
    const int weaponTypeCount = 3;

    /* Difficulty at which all weapon properties are equally likely to occur */
    const int middleWeightLevel = 3;

    /*Base change of an extra Order being given out after handing in */
    private float bonusOrderBaseProbability = 0.05f;

    /*If a Bonus Order is to be created */
    private bool createBonusOrder = false;

    /*Current time to fullfil an oder*/
    private float fullfilmentTimeS = 150;

    /*Base time to fullfil an oder*/
    private float baseFullfilmentTimeS = 150;

    /* Base change to Satisfaction upon order success or fail*/
    private float baseSatisfactionChange = 0.1f;

    /*Class reporesenting an Order */
    private class order
    {
        public GameObject orderImage;

        public int material;

        public int weaponType;

        public float timeLeft;
    }

    // List of all active Orders
    private List<order> ordersList = new List<order>();

    /*
     * Starts the Order System
     */
    public void startGame()
    {
        Debug.Log("Started Orders");
        gameRunning = true;
    }

    /*
     * Pauses the Order System (for testing purposes)
     */
    public void pause()
    {
        gamePaused = !gamePaused;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameEvents.instance.oPressed += startGame;
        GameEvents.instance.pPressed += pause;
        if (dayCountText != null)
        {
            dayCountText.text = dayCount.ToString();
        }
        if (gameOverText != null)
        {
            gameOverText.enabled = false;
        }
        weaponSprites = new Sprite[3] { dagger, axe, sword};
        //startGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning && !gamePaused)
        {
            setAmbientLight();
            //Reduce the timers of all orders and remove them from the list if they expire
            for (int i = 0; i < ordersList.Count; i++)
            {
                ordersList[i].timeLeft -= Time.deltaTime;

                updateTimerUI(ordersList[i].orderImage.transform.GetChild(1).transform.GetChild(1).GetComponent<Image>(), ordersList[i].timeLeft);

                if (ordersList[i].timeLeft <= 0)
                {
                    Destroy(ordersList[i].orderImage);
                    ordersList.RemoveAt(i);
                    Debug.Log("Order Deleted" + i);
                    satisfaction -= baseSatisfactionChange * (1 + (difficultyLevel / 10));
                    updateSatisfactionUI();
                    i--;
                }
            }

            if (satisfaction <= 0)
            {
                gameOver();
                //If there are no Orders left, a new round/day starts
            }
            else if (ordersList.Count == 0)
            {
                difficultyLevel++;
                dayCount++;
                dayCountText.text = dayCount.ToString();
                Debug.Log("Base: " + fullfilmentTimeS + " difficulty: " + (difficultyLevel));
                Debug.Log(Mathf.Max(1, Mathf.RoundToInt((float)difficultyLevel / 2.0f)));

                //On even days, the order count goes up, and thus the fullfilment time needs to aswell
                int orderCount = Mathf.Min(3, difficultyLevel / 2);
                if (dayCount % 2 == 0 && dayCount != 0 && (difficultyLevel / 2) <= 3)
                {
                    fullfilmentTimeS += baseFullfilmentTimeS;
                }
                Debug.Log("Neue Time: " + fullfilmentTimeS);
                for (int i = 0; i < orderCount; i++)
                {
                    createNewOrder();
                    float randomFloat = Random.Range(0f, 1f);
                    createBonusOrder = (randomFloat <= bonusOrderBaseProbability * difficultyLevel);
                }
            }


        }
    }

    /*
     * creates a new Order and adds it to the list of Orders
     */
    private void createNewOrder()
    {
        // Creates a random material and weapontype based on Lvl
        int materialValue = weightedRandomization(materialCount-1);
        int weaponTypeValue = weightedRandomization(weaponTypeCount-1);

        float timeLeft = fullfilmentTimeS;

        // Creates UI for the Order and moves it to the right position
        GameObject orderImg = Instantiate(orderPrefab, boardCanvas.transform.position, orderPrefab.transform.rotation);
        orderImg.transform.SetParent(boardCanvas.transform);
        orderImg.transform.localPosition += new Vector3(ordersList.Count * 0.55f, 0, 0);
        orderImg.transform.localPosition += new Vector3(1.5f, 1.5f, 0);

        //TODO gamescene anpassen
        //Set corrensponding weapon sprite
        Sprite weaponSprite = weaponSprites[weaponTypeValue];
        orderImg.transform.GetChild(2).GetComponent<Image>().sprite = weaponSprite;

        //Change colour of the sprite based on material TODO color array
        switch (materialValue)
        {
            case 0:
                orderImg.transform.GetChild(2).GetComponent<Image>().color = new Color(1.0f, 0.78f, 0.09f, 1.0f);
                break;
            case 1:
                orderImg.transform.GetChild(2).GetComponent<Image>().color = new Color(0.43f, 0.48f, 0.37f, 1.0f);
                break;
            case 2:
                orderImg.transform.GetChild(2).GetComponent<Image>().color = new Color(0.60f, 0.23f, 0.07f, 1.0f);
                break;
            case 3:
                orderImg.transform.GetChild(2).GetComponent<Image>().color = new Color(0.28f, 0.31f, 0.33f, 1.0f);
                break;
        }


        ordersList.Add(new order()
        {
            orderImage = orderImg,
            material = materialValue,
            weaponType = weaponTypeValue,
            timeLeft = timeLeft
        });

        Debug.Log("New Order Created. Material: " + materialValue + " Weapon: " + weaponTypeValue);
    }

    /*
     * Creates a random number based on a maximum and the difficulty Lvl
     * @param maxExclusiveValue - upper border for randomization
     * @return int - a random number based on a maximum and the difficulty Lvl
     */
    private int weightedRandomization(int maxExclusiveValue)
    {
        //TODO maximales level
        //A random number is generated, then shifted based on the Lv
        int weightedValue = Random.Range(0, maxExclusiveValue);

        weightedValue += (difficultyLevel - middleWeightLevel);

        if (weightedValue < 0)
        {
            weightedValue = 0;
        }
        else if (weightedValue >= (maxExclusiveValue))
        {
            weightedValue = maxExclusiveValue--;
        }

        return weightedValue;
    }

    /*
     * Checks if a handed in weapon is acceptable and changes the game state correspondingly
     * @param material - Material the weapon is made of
     * @param weaponType - Type of weapon
     * @param hammerScore - Score achieved in the hammer game
     * @param sharpeningScore - Score achieved in the sharpening game
     * @return if a handed in weapon is acceptable
     */
    public bool tryToAccept(int material, int weaponType, float hammerScore, float sharpeningScore)
    {
        //Goes through the list of Orders to find one which corresponds to the hand-in
        bool found = false;
        for (int i = 0; i < ordersList.Count && !found; i++)
        { 
            if (ordersList[i].material == material && ordersList[i].weaponType == weaponType)
            {
                found = true;
                //Changes Satisfaction based on the average of the Scores 
                float overallScore = ((float) hammerScore + (float) sharpeningScore) / 2;
                Debug.Log("Satisfaction Change: " + (float) baseSatisfactionChange * overallScore);
                satisfaction += ( (float) baseSatisfactionChange * overallScore);
                Debug.Log("Current Satisfaction: " + satisfaction);

                Destroy(ordersList[i].orderImage);
                ordersList.RemoveAt(i);

                if(satisfaction > 1)
                {
                    satisfaction = 1;
                }
                updateSatisfactionUI();

                if (createBonusOrder)
                {
                    createNewOrder();
                    createBonusOrder = false;
                }
            }
        }
        if(found)
        {
            GameEvents.instance.PlaySound("Success", this.gameObject.transform.position);
        } else
        {
            GameEvents.instance.PlaySound("Failure", this.gameObject.transform.position);
        }
        return found;
    }

    /*
     * Changes the displayed Satisfaction UI after a value change
     */
    private void updateSatisfactionUI() 
    {
        satisfactionFill.GetComponent<Image>().fillAmount = satisfaction;
        satisfactionText.text = ((int) (satisfaction*100)).ToString();
        if (satisfaction <= 0.33f)
        {
            satisfactionFill.GetComponent<Image>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            satisfactionImage.GetComponent<Image>().sprite = frown;
        } else if (satisfaction <= 0.66f)
        {
            satisfactionFill.GetComponent<Image>().color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
            satisfactionImage.GetComponent<Image>().sprite = neutral;
        } else
        {
            satisfactionFill.GetComponent<Image>().color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
            satisfactionImage.GetComponent<Image>().sprite = smile;
        }
        
    }

    /*
     * Changes the displayed Timer UI of an Order
     * @param oderIMG - Order to be updated
     * @param time - leftover time to complete
     */
    private void updateTimerUI(Image orderIMG, float time)
    {
        float timeRatio = time / fullfilmentTimeS;
        orderIMG.fillAmount = timeRatio;
        if (timeRatio <= 0.33f)
        {
            orderIMG.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
        else if (timeRatio <= 0.66f)
        {
            orderIMG.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
        }
        else
        {
            orderIMG.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        }

    }


    /*
     * Starts the following Coroutine to display a game over text and restart the game after a certain interval
     */
    private void gameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    /*
     * Displays a game over text and restarts the game after a certain interval
     */
    IEnumerator GameOverCoroutine()
    {
        gameOverText.enabled = true;
        gameOverText.text = "Game Over...\nYou made it until Day\n" + dayCount;

        yield return new WaitForSeconds(5.0f);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    /**
     * Set the Ambientlight by Time of the OrderSystem
     */
    public void setAmbientLight()
    {
        float currenttime = 0;
        float intensity = 0;

        if (ordersList.Count != 0)
        {
            currenttime = (1 - ordersList[0].timeLeft / fullfilmentTimeS);
            if (currenttime > 0.5)
            {
                intensity = 1 - ((currenttime - 0.5f) * 2);
            }
            else
            {
                intensity = currenttime * 2;
            }
            RenderSettings.ambientIntensity = intensity;
        }
    }


}
