using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BuySystem : MonoBehaviour
{
    public GameObject buildingsPanel;
    public GameObject unitsPanel;

    public Button buildingsButton;
    public Button unitsButton;

    public PlacementSystem placementSystem;

    public ObjectsDatabaseSO database;

    #region | --- Unit Training --- |

    public bool IsDatabaseItemABuilding(int itemID)
    {
        // return true if it is a building
        return database.GetObjectByID(itemID).thisBuildingType != BuildingType.None;
    }

    public void StartUnitTraining(int databaseItemID, float trainingTime) // Bridge method
    {
        StartCoroutine(StartUnitTrainingCoroutine(databaseItemID, trainingTime));
    }

    internal IEnumerator StartUnitTrainingCoroutine(int databaseItemID, float trainingTime)
    {
        yield return new WaitForSeconds(trainingTime);

        ObjectData unitToTrain = database.GetObjectByID(databaseItemID);
        if (unitToTrain.thisUnitType == UnitType.None)
        {
            Debug.LogError("Forgot to assign a unit type");
        }

        InstantiateUnit(unitToTrain.Name);
    }

    private void InstantiateUnit(string unitName)
    {
        Constructable[] allConstructables = FindObjectsOfType<Constructable>();
        Constructable commandCenter = null;

        foreach (var c in allConstructables)
        {
            if (c.buildingType == BuildingType.CommandCenter)
            {
                commandCenter = c;
                break;
            }
        }

        if (commandCenter == null)
        {
            // This should not happen
            Debug.LogWarning("No Command Center found");
        }

        GameObject unitPrefab = Resources.Load<GameObject>(unitName);

        if (unitPrefab == null)
        {
            Debug.LogWarning("Could not find unit prefab, make sure the name is the same as in the Database");
            return;
        }

        Vector3 entrancePosition = commandCenter.transform.position + new Vector3(-1f, 0f, 0.5f);
        Vector3 randomOffset = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        Vector3 finalSpawnPosition = entrancePosition + randomOffset;

        GameObject newUnit = Instantiate(unitPrefab, finalSpawnPosition, Quaternion.identity);

        SoundManager.Instance.PlayUnitReadySound();
    }

    public void StartBuildingOrTraining(BuySlot slot, float trainingTime) // Bridge method
    {
        StartCoroutine(StartBuildingOrTrainingCoroutine(slot, trainingTime));
    }

    private IEnumerator StartBuildingOrTrainingCoroutine(BuySlot slot, float trainingTime)
    {
        slot.isBuildingOrTraining = true;

        yield return new WaitForSeconds(trainingTime);

        slot.isReadyToPlace = true;
        slot.readyBG.SetActive(true);

        SoundManager.Instance.ConstructionCompleteSound();
    }

    #endregion

    #region | --- UI Related --- |

    private void Start()
    {
        unitsButton.onClick.AddListener(UnitsCategorySelected);
        buildingsButton.onClick.AddListener(BuildingsCategorySelected);

        unitsPanel.SetActive(false);
        buildingsPanel.SetActive(true);
    }

    private void BuildingsCategorySelected()
    {
        unitsPanel.SetActive(false);
        buildingsPanel.SetActive(true);
    }

    private void UnitsCategorySelected()
    {
        unitsPanel.SetActive(true);
        buildingsPanel.SetActive(false);
    }

    #endregion
}
