using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuySlot : MonoBehaviour
{
    public Sprite availableSprite;
    public Sprite unAvailableSprite;

    private bool isAvailable;

    public BuySystem buySystem;

    public int databaseItemID;

    private void Start()
    {
        // Subscribe to event / listen to event
        ResourceManager.Instance.OnResourceChanged += HandleResourcesChanged;
        HandleResourcesChanged();

        ResourceManager.Instance.OnBuildingsChanged += HandleBuildingsChanged;
        HandleBuildingsChanged();
    }

    private void OnDestroy()
    {
        // Unsubscribe
        ResourceManager.Instance.OnResourceChanged -= HandleResourcesChanged;
        ResourceManager.Instance.OnBuildingsChanged -= HandleBuildingsChanged;
    }

    public void ClickedOnSlot()
    {
        if (isAvailable)
        {
            buySystem.placementSystem.StartPlacement(databaseItemID);
        }
    }

    private void UpdateAvailabilityUI()
    {
        if (isAvailable)
        {
            GetComponent<Image>().sprite = availableSprite;
            GetComponent<Button>().interactable = true;
        }
        else
        {
            GetComponent<Image>().sprite = unAvailableSprite;
            GetComponent<Button>().interactable = false;
        }
    }

    private void HandleResourcesChanged()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.objectsData[databaseItemID];

        bool requirementMet = true;

        foreach (BuildRequirement req in objectData.resourceRequirements)
        {
            if (ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
            {
                requirementMet = false;
                break;
            }
        }

        isAvailable = requirementMet;

        UpdateAvailabilityUI();
    }

    private void HandleBuildingsChanged()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.objectsData[databaseItemID];

        foreach (BuildingType dependency in objectData.buildDependency)
        {
            // If the building has not dependencies
            if (dependency == BuildingType.None)
            {
                gameObject.SetActive(true);
                return;
            }

            // Check if dependency exists
            if (ResourceManager.Instance.allExistingBuildings.Contains(dependency) == false)
            {
                gameObject.SetActive(false);
                return;
            }
        }

        // If all requirements are met
        gameObject.SetActive(true);
    }
}
