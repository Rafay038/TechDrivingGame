using UnityEngine;

public class HandlingBoost : MonoBehaviour, IPowerUp
{
    [SerializeField] public float amount = 10f;
    [SerializeField] public float duration = 3f;
    [SerializeField] public string powerUpName = "Handling Boost";

    public void Apply(CarStatsModifier car, string powerUpName)
    {
        car.ApplyHandlingBoost(amount, duration, powerUpName);
    }

    public string GetName() => powerUpName;
}
