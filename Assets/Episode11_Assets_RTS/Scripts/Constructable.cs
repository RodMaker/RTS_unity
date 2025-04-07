using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class Constructable : MonoBehaviour, IDamageable
{
    private float constHealth;
    public float constMaxHealth;

    public HealthTracker healthTracker;

    public bool isEnemy = false;

    public BuildingType buildingType;

    public Vector3 buildPosition;

    public bool inPreviewMode;

    private void Start()
    {
        constHealth = constMaxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(constHealth, constMaxHealth);

        if (constHealth <= 0)
        {
            // Other destruction logic
            ResourceManager.Instance.UpdateBuildingChanged(buildingType, false, buildPosition);

            SoundManager.Instance.PlayBuildingDestructionSound();

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (inPreviewMode == false)
        {
            if (constHealth > 0 && buildPosition != Vector3.zero)
            {
                ResourceManager.Instance.SellBuilding(buildingType);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        constHealth -= damage;
        UpdateHealthUI();
    }

    public void ConstructableWasPlaced(Vector3 position)
    {
        buildPosition = position;

        inPreviewMode = false;

        Debug.Log("placed");

        // Make healthbar visible
        healthTracker.gameObject.SetActive(true);

        ActivateObstacle();

        if (isEnemy)
        {
            gameObject.tag = "Enemy";
        }

        if (GetComponent<PowerUser>() != null)
        {
            GetComponent<PowerUser>().PowerOn();
        }

        if (buildingType == BuildingType.SupplyCenter)
        {
            GameObject prefab = Resources.Load<GameObject>("Harvester");
            if (prefab != null)
            {
                Transform supplyDropTransform = transform.Find("SupplyDrop").transform;

                GameObject instance = Instantiate(prefab, supplyDropTransform.position, Quaternion.identity);
                instance.transform.SetParent(null);

                Harvester harvester = instance.GetComponent<Harvester>();
                harvester.supplyCenter = supplyDropTransform;
            }
        }
    }

    private void ActivateObstacle()
    {
        NavMeshObstacle[] obstacles = GetComponentsInChildren<NavMeshObstacle>();
        foreach (NavMeshObstacle obstacle in obstacles)
        {
            obstacle.enabled = true;
        }
    }
}
