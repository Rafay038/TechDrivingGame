using UnityEngine;

public class HealthIncrease : MonoBehaviour, IPowerUp
{
    [SerializeField] public float healthAmount = 200f;
    [SerializeField] public string powerUpName = "Health Boost";

    public void Apply(CarStatsModifier car, string powerUpName)
    {
        car.ApplyHealthBoost(healthAmount, powerUpName);
    }

    public string GetName() => powerUpName;
}
