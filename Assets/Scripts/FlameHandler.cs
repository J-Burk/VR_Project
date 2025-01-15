using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameHandler : MonoBehaviour
{
    // Temperatur Values
    public float temperatur;
    private float minTemperatur;
    private float maxTemperatur;

    // Flameparticle Values
    private ParticleSystem flameParticle;
    private float newGravity;
    private float actTemp;

    void Start()
    {
        flameParticle = transform.GetChild(0).GetComponent<ParticleSystem>();

        // Temperatur Settings
        temperatur = 100;
        minTemperatur = 100;
        maxTemperatur = 150;
        newGravity = 0.01f;
    }

    void Update()
    {
        // Reduce Temp after using Bellow
        if (temperatur > minTemperatur)
        {
            temperatur -= Time.deltaTime;
        }

        // Set back after ereas above Maxtemp
        if (temperatur > maxTemperatur)
        {
            temperatur -= (temperatur - maxTemperatur);
        }

        // Change Flamesize
        actTemp = (newGravity * (temperatur - minTemperatur)) / 4;
        ChangeGravityModifier(actTemp);
    }

    void ChangeGravityModifier(float actTemp)
    {
        // Set of the GravityModifier
        var mainModule = flameParticle.main;
        mainModule.gravityModifier = -actTemp;
    }
}
