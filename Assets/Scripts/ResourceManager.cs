using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; set; }

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

    public int credits = 300;
    public int oil = 0;

    public event Action OnResourceChanged;
    public event Action OnBuildingsChanged;

    public TextMeshProUGUI creditsUI;
    public TextMeshProUGUI oilAmountUI;

    public List<BuildingType> allExistingBuildings;

    public PlacementSystem placementSystem;

    internal const int CREDITS_PER_KILO_GOLD = 5;

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateBuildingChanged(BuildingType buildingType, bool isNew, Vector3 position)
    {
        if (isNew)
        {
            // Building
            allExistingBuildings.Add(buildingType);

            SoundManager.Instance.PlayBuildingConstructionSound();
        }
        else
        {
            // Can be both destruction and selling
            placementSystem.RemovePlacementData(position);
            allExistingBuildings.Remove(buildingType);
        }

        OnBuildingsChanged?.Invoke();
    }

    public void IncreaseResource(ResourceType resource, int amountToIncrease)
    {
        switch (resource)
        {
            case ResourceType.Credits:
                credits += amountToIncrease;
                break;
            case ResourceType.Oil:
                oil += amountToIncrease; 
                break;
            default: 
                break;
        }

        OnResourceChanged?.Invoke();
    }

    public void DecreaseResource(ResourceType resource, int amountToDecrease)
    {
        switch (resource)
        {
            case ResourceType.Credits:
                credits -= amountToDecrease;
                break;
            case ResourceType.Oil:
                oil -= amountToDecrease;
                break;
            default:
                break;
        }

        OnResourceChanged?.Invoke();
    }

    public void SellBuilding(BuildingType buildingType)
    {
        Debug.Log("Selling building");

        SoundManager.Instance.PlayBuildingSellingSound();

        var sellingPrice = 0;
        
        foreach (ObjectData obj in DatabaseManager.Instance.databaseSO.objectsData)
        {
            if (obj.thisBuildingType == buildingType)
            {
                foreach (BuildRequirement req in obj.resourceRequirements)
                {
                    if (req.resource == ResourceType.Credits)
                    {
                        sellingPrice = req.amount;
                    }
                }
            }
        }

        int amountToReturn = (int)(sellingPrice * 0.5f); // 50% of the cost

        IncreaseResource(ResourceType.Credits, amountToReturn);
    }

    private void UpdateUI()
    {
        creditsUI.text = $"{credits}";
        oilAmountUI.text = $"{oil}";
    }

    public int GetCredits()
    {
        return credits;
    }

    internal int GetResourceAmount(ResourceType resource)
    {
        switch (resource)
        {
            case ResourceType.Credits:
                return credits;
            case ResourceType.Oil:
                return oil;
            default:
                break;
        }

        return 0;
    }

    internal void DecreaseResourcesBasedOnRequirement(ObjectData objectData)
    {
        foreach (BuildRequirement req in objectData.resourceRequirements)
        {
            DecreaseResource(req.resource, req.amount);
        }
    }

    private void OnEnable()
    {
        OnResourceChanged += UpdateUI;
    }

    private void OnDisable()
    {
        OnResourceChanged -= UpdateUI;
    }
}

public enum ResourceType
{
    Credits,
    Oil,
    Gold,
    None
}
