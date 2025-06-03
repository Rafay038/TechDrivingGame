public interface IPowerUp
{
    void Apply(CarStatsModifier car, string powerUpName);
    string GetName();
}