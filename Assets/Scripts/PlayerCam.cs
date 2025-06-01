using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 cameraOffsetPos;
    [SerializeField] private Vector3 cameraOffsetRot;
    [SerializeField] private float _stepSpeed;
    private Vector3 carBaseScale;
    private Vector3 targetDirection;
    private Vector3 realoffset;
    void Start()
    {

        transform.position = player.transform.position + cameraOffsetPos;
        carBaseScale = player.transform.localScale;
    }

    void LateUpdate()
    {
        //float angle = Mathf.Atan((player.transform.position.x - transform.position.x) / (player.transform.position.z - transform.position.z));      //Possibly wrong sign, depending on your setup
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, angle, transform.rotation.eulerAngles.z);

        if (carBaseScale == player.transform.localScale)
        {
            realoffset = player.transform.forward * cameraOffsetPos.z + player.transform.right * cameraOffsetPos.x + player.transform.up * cameraOffsetPos.y;
        }
        else
        {
            realoffset = (player.transform.forward * cameraOffsetPos.z + player.transform.right * cameraOffsetPos.x + player.transform.up * cameraOffsetPos.y) * 1.5f;
        }

        // Add checks for if the car is grounded or not, if not reset cameraOffsetRot to 0 0 0 to look at the car directly

        if (!CarController.isOnTheGround)
            // Determine which direction to rotate towards
            targetDirection = player.transform.localPosition + cameraOffsetRot - transform.position;
        else
            {
                targetDirection = player.transform.localPosition - transform.position;
            }
            

        transform.position = player.transform.localPosition - realoffset;

        

        // The step size is equal to speed times frame time.
        float singleStep = _stepSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        if (CarController.isOnTheGround)
            transform.rotation = Quaternion.LookRotation(newDirection);
        else
            // if the car is in the air, look a bit higher and from further away
            transform.rotation = Quaternion.LookRotation(newDirection);

        //float angle = Mathf.Atan((player.transform.position.x - transform.position.x) / (player.transform.position.z - transform.position.z));      //Possibly wrong sign, depending on your setup
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, angle, transform.rotation.eulerAngles.z);

    }
}
