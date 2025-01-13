using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    public static PowerManager Instance { get; private set; }

    public int totalPower; // Total power produced
    public int powerUsage; // Total power consumed

    [SerializeField] private Image sliderFill;
    [SerializeField] private Slider powerSlider;
    [SerializeField] private TextMeshProUGUI powerText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void AddPower(int amount)
    {
        totalPower += amount;
        UpdatePowerUI();
    }

    public void ConsumePower(int amount)
    {
        powerUsage += amount;
        UpdatePowerUI();
    }

    public void RemovePower(int amount)
    {
        totalPower -= amount;
        UpdatePowerUI();
    }

    public void ReleasePower(int amount)
    {
        powerUsage -= amount;
        UpdatePowerUI();
    }

    private void UpdatePowerUI()
    {
        if (powerSlider != null)
        {
            powerSlider.maxValue = totalPower;
            powerSlider.value = totalPower - powerUsage; // The total available power
        }

        if (powerText != null)
        {
            powerText.text = $"{totalPower - powerUsage}/{totalPower}";
        }
    }

    public int CalculateAvailablePower()
    {
        return totalPower - powerUsage;
    }
}
