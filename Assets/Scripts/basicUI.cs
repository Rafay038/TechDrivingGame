using TMPro;
using UnityEngine;


public class basicUI : MonoBehaviour

{

    [SerializeField]
    private TMP_Text displayText;

    private bool displayTutorial = true;

    private void Start()
    {
        displayText.text = "Controls:\n\n\tPress W or ArrowUp to accelerate\n\n\tPress S or ArrowDown to reverse\n\n\tPress Spacebar to brake\n\n\tPress R to reset";
    }

    // Update is called once per frame
    void Update()
    {
        // Stop displaying the tutorial text once the car starts moving
        if (Input.GetAxis("Vertical") != 0)
            displayTutorial = false;

        if (!displayTutorial && CarController.carHealth > 0)
            displayText.text = "Speed: " + CarController.carSpeed + " km/h" + "\nHealth: " + CarController.carHealth;
        if (CarController.carHealth <= 0)
            displayText.text = "Oh dear, you are dead!\nPress R to restart";
    }
}
