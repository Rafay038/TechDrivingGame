using System;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    internal enum powerupType
    {
        healthRestore,
        nitrusRestore,
        // add more as needed
    }

    [SerializeField] private powerupType powerup;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Spinning effect!
        this.transform.Rotate(Vector3.up * Time.deltaTime * 50);
        this.transform.Rotate(Vector3.left * Time.deltaTime * 50);
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Something entered");
        if (other.CompareTag("Player"))
        {
            //Debug.Log("And that something is a player");
            switch (powerup)
            {
                case powerupType.healthRestore:
                    if (CarController.carHealth + 100 > CarController.carHealthMax)
                        CarController.carHealth = CarController.carHealthMax;
                    else
                        CarController.carHealth += 100;
                    break;
                case powerupType.nitrusRestore:
                    if (CarController.nitrusValue + 10 > 2 * CarController.nitrusValueMax)
                        CarController.nitrusValue = CarController.nitrusValueMax;
                    else
                        CarController.nitrusValue += 10;
                    break;
            }
            Destroy(gameObject);
        }

    }
}
