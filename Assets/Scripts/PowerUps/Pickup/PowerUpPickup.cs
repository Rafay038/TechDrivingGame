using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerUpPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CarStatsModifier car = other.GetComponentInParent<CarStatsModifier>();
        if (car != null)
        {
            foreach (var powerUp in GetComponents<IPowerUp>())
            {
                powerUp.Apply(car, powerUp.GetName());
            }

            Destroy(gameObject);
        }
    }
}
