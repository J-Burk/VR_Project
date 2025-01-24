using UnityEngine;

/** Class handling the Anvil's in- and output Game Objects*/
public class Anvil : MonoBehaviour
{
    /* Slots for Ingots on the Anvil */
    public int snapedObjectCounter = 0;
    public GameObject[] states = new GameObject[6];
    /*Arrays including the Game Objects for the different Ore Variations of weapons */
    public GameObject[] daggers;
    public GameObject[] axes;
    public GameObject[] swords;

    
    // Start is called before the first frame update
    void Start()
    {
        // Listening to the Hammer game Success and Fail calls made by HammerSystem.cs
        GameEvents.instance.hammerSuccess += smithingsucces;
        GameEvents.instance.hammerFail += deleteMaterial;
        for (int i = 0; i < states.Length; i++)
        {
            states[i] = null;
        }
    }

    /*
     * Spawns the respective weapon if the Hammer Game has been completed successfully
     * @param weaponType - Type of Weapon (0-2)
     * @param oreType - Which Ore the Weapon is made of (0-3)
     * @param score - Reached Hammer Game Score (0.2 - 1.0)
     */
    private void smithingsucces(int weaponType, int oreType, float score)
    {
        GameObject spawnedWeapon = null;

        //Instantiate a weapon Game Object based on the parameters
        switch (weaponType)
        {
            case 0: 
                spawnedWeapon = Instantiate(daggers[oreType], new Vector3(0.4f, 0.9f, -0.9f), Quaternion.identity);
                break;
            case 1:
                spawnedWeapon = Instantiate(axes[oreType], new Vector3(0.4f, 0.9f, -0.9f), Quaternion.identity);
                break;
            case 2:
                spawnedWeapon = Instantiate(swords[oreType], new Vector3(0.4f, 0.9f, -0.9f), Quaternion.identity);
                break;
        }

        // Feeds weapon information into the Game Object
        WeaponStats stats = spawnedWeapon.GetComponent<WeaponStats>();
        stats.hammerScore = score;
        stats.weaponType = weaponType;
        stats.oreType = oreType;

        // Deletes the Ores on the anvil
        deleteMaterial();
    }

    /*
     * "Cleans up" the anvil by deleting all ores on it and resetting the respective variables 
     */
    private void deleteMaterial()
    {
        for (int i = 0; i < states.Length; i++)
        {
            Destroy(states[i]);
            states[i] = null;
        }
        snapedObjectCounter = 0;
    }

    /*
     * Checks how many ingots are lying on the anvil
     * @return int - how many ingots are lying on the anvil
     */
    public int ingotCount()
    {
        int count = 0;
        for (int i = 0; i < states.Length; i++)
        {
            if(states[i] != null)
            {
                count++;
            }
        }
        return count;
    }

    /*
     * Checks if the anvil is empty
     * @return bool - is the anvil empty
     */
    public bool empty()
    {
        return ingotCount() == 0;
    }

    /*
     * Checks if the ingot count on the anvil is valid for the hammer game
     * @return bool - is the anvil empty
     */
    public bool valid()
    {
        int count = ingotCount();
        return count != 0 && count != 1 && count != 3 && count != 5;
    }

    
}
