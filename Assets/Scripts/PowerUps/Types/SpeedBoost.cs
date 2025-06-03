using UnityEngine;

public class SpeedBoost : MonoBehaviour, IPowerUp
{
    [SerializeField] public float amount = 1000f;
    [SerializeField] public float duration = 3f;
    [SerializeField] public string powerUpName = "Speed Boost";

    public void Apply(CarStatsModifier car, string powerUpName)
    {
        car.ApplySpeedBoost(amount, duration, powerUpName);
    }

    public string GetName() => powerUpName;
}
