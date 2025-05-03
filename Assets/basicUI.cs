using TMPro;
using UnityEngine;


public class basicUI : MonoBehaviour

{

    [SerializeField]
    private TMP_Text displayText;

    // Update is called once per frame
    void Update()
    {
        displayText.text = "Speed: " + CarController.carSpeed + "km/h";
    }
}
