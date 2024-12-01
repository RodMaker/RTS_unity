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
        HandleResourcesChanged();
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

    private void OnEnable()
    {
        // Subscribe to event / listen to event
        ResourceManager.Instance.OnResourceChanged += HandleResourcesChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe to event / stop listening to event
        ResourceManager.Instance.OnResourceChanged -= HandleResourcesChanged;
    }

    private void HandleResourcesChanged()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.objectsData[databaseItemID];

        bool requirementMet = true;

        foreach (BuildRequirement req in objectData.requirements)
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
}
