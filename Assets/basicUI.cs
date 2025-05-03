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
        if (CarController.carSpeed != 0)
            displayTutorial = false;

        if (!displayTutorial)
            displayText.text = "Speed: " + CarController.carSpeed + "km/h";
    }
}
