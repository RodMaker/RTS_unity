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

    public AudioClip powerAddedClip;
    public AudioClip powerInsufficientClip;

    private AudioSource powerAudioSource;

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

        powerAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void AddPower(int amount) // Constructing a new producer building
    {
        PlayPowerAddedSound();
        totalPower += amount;
        UpdatePowerUI();
    }

    public void ConsumePower(int amount) // Constructing a new consumer building
    {
        powerUsage += amount;
        UpdatePowerUI();

        if (IsInsufficientPower())
        {
            PlayPowerInsufficientSound();
        }
    }

    public void RemovePower(int amount) // Destroying a producer building
    {
        totalPower -= amount;
        UpdatePowerUI();

        if (IsInsufficientPower())
        {
            PlayPowerInsufficientSound();
        }
    }

    public void ReleasePower(int amount) // Destroying a consumer building
    {
        powerUsage -= amount;
        UpdatePowerUI();
    }

    private bool IsInsufficientPower()
    {
        return (totalPower - powerUsage) <= 0;
    }

    private void UpdatePowerUI()
    {
        int availablePower = totalPower - powerUsage;

        if (availablePower > 0)
        {
            sliderFill.gameObject.SetActive(true);
        }
        else
        {
            sliderFill.gameObject.SetActive(false);
        }

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

    public void PlayPowerAddedSound()
    {
        powerAudioSource.PlayOneShot(powerAddedClip);
    }

    public void PlayPowerInsufficientSound()
    {
        powerAudioSource.PlayOneShot(powerInsufficientClip);
    }
}
