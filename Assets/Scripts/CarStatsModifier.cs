using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarStatsModifier : MonoBehaviour
{
    [Header("Target Car Controller Reference")]
    public CarController car;

    private Dictionary<string, object> boostMultipliers = new Dictionary<string, object>();
    private Dictionary<string, List<object>> activeBoostValues = new Dictionary<string, List<object>>();
    public static bool isPowerUpApplied = false;
    public static string powerUp = "";
    private Coroutine speedCoroutine, handlingCoroutine, sizeCoroutine;

    private Vector3 baseScale;
    private float baseMotorForce, baseMaxSteerAngle;
    private double baseCarHealth;

    private void Awake()
    {
        if (car == null)
            car = GetComponent<CarController>();

        baseScale = car.transform.localScale;
        baseMotorForce = car.MotorForce;
        baseMaxSteerAngle = car.MaxSteerAngle;
        baseCarHealth = CarController.carHealth;
        isPowerUpApplied = false;
        powerUp = "";
    }

    public void ApplySpeedBoost(float boostAmount, float duration, string powerUpName)
    {

        speedCoroutine = StartCoroutine(
            ApplyTemporaryBoost(
                () => car.MotorForce,
                value => car.MotorForce = value,
                boostAmount,
                duration,
                (a, b) => a + b,
                powerUpName,
                false
            )
        );
    }

    public void ApplyHandlingBoost(float boostAmount, float duration, string powerUpName)
    {

        handlingCoroutine = StartCoroutine(
            ApplyTemporaryBoost(
                () => car.MaxSteerAngle,
                value => car.MaxSteerAngle = value,
                boostAmount,
                duration,
                (a, b) => a + b,
                powerUpName,
                false
            )
        );
    }

    public void ApplyHealthBoost(float boostAmount, string powerUpName)
    {
        CarController.carHealth = Math.Min(CarController.carHealth + boostAmount, baseCarHealth);
    }

    public void ChangeSize(float scaleFactor, float duration, string powerUpName)
    {

        sizeCoroutine = StartCoroutine(
            ApplyTemporaryBoost(
                () => car.transform.localScale,
                value => car.transform.localScale = value,
                baseScale * scaleFactor,
                duration,
                (_, newVal) => newVal,
                powerUpName,
                true
            )
        );
    }

    private T Negate<T>(T value) where T : struct
    {
        if (typeof(T) == typeof(float))
        {
            float v = (float)(object)value;
            return (T)(object)(-v);
        }
        else if (typeof(T) == typeof(int))
        {
            int v = (int)(object)value;
            return (T)(object)(-v);
        }
        else if (typeof(T) == typeof(double))
        {
            double v = (double)(object)value;
            return (T)(object)(-v);
        }
        else
        {
            throw new NotSupportedException($"Negate not supported for type {typeof(T)}");
        }
    }

    private IEnumerator ApplyTemporaryBoost<T>(
        Func<T> getter,
        Action<T> setter,
        T boostValue,
        float duration,
        Func<T, T, T> adder,
        string powerUpName,
        bool isOverwrite = false
    ) where T : struct
    {
        if (!activeBoostValues.ContainsKey(powerUpName))
        {
            boostMultipliers[powerUpName] = getter(); 
            activeBoostValues[powerUpName] = new List<object>();
        }

        activeBoostValues[powerUpName].Add(boostValue);

        T baseValue = (T)boostMultipliers[powerUpName];
        T totalBoost;

        if (isOverwrite)
        {
            totalBoost = boostValue;
        }
        else
        {
            totalBoost = baseValue;
            foreach (var val in activeBoostValues[powerUpName])
                totalBoost = adder(totalBoost, (T)val);
        }

        setter(totalBoost);

        isPowerUpApplied = true;
        powerUp = powerUpName;

        yield return new WaitForSeconds(duration);

        activeBoostValues[powerUpName].Remove(boostValue);

        if (activeBoostValues[powerUpName].Count == 0)
        {
            setter(baseValue);

            boostMultipliers.Remove(powerUpName);
            activeBoostValues.Remove(powerUpName);

            isPowerUpApplied = false;
            powerUp = "";
        }
        else
        {
            if (isOverwrite)
            {
                var lastBoost = (T)activeBoostValues[powerUpName][activeBoostValues[powerUpName].Count - 1];
                setter(lastBoost);
            }
            else
            {
                totalBoost = baseValue;
                foreach (var val in activeBoostValues[powerUpName])
                {
                    totalBoost = adder(totalBoost, (T)val);
                }
                setter(totalBoost);
            }
        }

    }
}
