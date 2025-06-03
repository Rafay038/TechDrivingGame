using UnityEngine;

public class SizeIncrease : MonoBehaviour, IPowerUp
{
    [SerializeField] public float scaleMultiplier = 1.5f;
    [SerializeField] public float duration = 5f;
    [SerializeField] public string powerUpName = "Size Increase";

    public void Apply(CarStatsModifier car, string powerUpName)
    {
        car.ChangeSize(scaleMultiplier, duration, powerUpName);
    }

    public string GetName() => powerUpName;
}
