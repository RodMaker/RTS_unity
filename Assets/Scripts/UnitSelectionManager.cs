using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; set; }

    public List<GameObject> allUnitsList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();

    public LayerMask clickable;
    public LayerMask ground;
    public LayerMask attackable;

    public bool attackCursorVisible;

    public LayerMask constructable;

    public GameObject groundMarker;

    private Camera cam;

    public bool playedDuringThisDrag = false;

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

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // If we are hitting a clickable object
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    MultiSelect(hit.collider.gameObject);
                }
                else
                {
                    SelectByClicking(hit.collider.gameObject);
                }
            }
            else // If we are NOT hitting a clickable object
            {
                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    DeselectAll();
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && unitsSelected.Count > 0)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // If we are hitting the ground
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                groundMarker.transform.position = hit.point;

                groundMarker.SetActive(false);
                groundMarker.SetActive(true);
            }
        }

        // Attack target
        if (unitsSelected.Count > 0 && AtLeastOneOffensiveUnit(unitsSelected))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            // If we are hitting a clickable object
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, attackable))
            {
                Debug.Log("Enemy hovered with mouse");

                attackCursorVisible = true;

                if (Input.GetMouseButtonDown(1))
                {
                    Transform target = hit.transform;

                    foreach (GameObject unit in unitsSelected)
                    {
                        if (unit.GetComponent<AttackController>())
                        {
                            unit.GetComponent<AttackController>().targetToAttack = target;
                        }
                    }
                }
            }
            else
            {
                attackCursorVisible = false;
            }
        }

        // Hovering and clicking on Resource node while Harvester unit is selected

        if (unitsSelected.Count > 0 && OnlyHarvestersSelected()) // Only if harvesters are selected
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide))
            {
                ResourceNode resourceNode = hit.transform.GetComponent<ResourceNode>();
                if (resourceNode != null)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        Transform node = hit.transform;

                        foreach (GameObject unit in unitsSelected)
                        {
                            Harvester harvester = unit.GetComponent<Harvester>();
                            if (harvester != null)
                            {
                                if (harvester.currentResourceType == ResourceType.None ||
                                    harvester.currentResourceType == resourceNode.resourceType)
                                {
                                    // Either empty or the same
                                    harvester.assignedNode = node;
                                }
                                else
                                {
                                    UnitMovement unitMovement = harvester.GetComponent<UnitMovement>();
                                    unitMovement.allowManualInput = false;
                                    harvester.MoveTo(harvester.supplyCenter);
                                    StartCoroutine(ReenableManualInputNextFrame(unitMovement));
                                }
                            }
                        }
                    }
                }
            }
        }

        CursorSelector();
    }

    private IEnumerator ReenableManualInputNextFrame(UnitMovement unitMovement)
    {
        yield return null; // wait for one frame
        unitMovement.allowManualInput = true;
    }

    private bool OnlyHarvestersSelected()
    {
        if (unitsSelected.Count == 0) 
        {
            return false;
        }

        foreach (GameObject unit in unitsSelected)
        {
            if (unit == null || unit.GetComponent<Harvester>() == null)
            {
                return false; // If a non-harvester unit is selected return false
            }
        }
        return true;
    }

    private void CursorSelector()
    {
        if (TrySetWalkableCursor()) return;
        if (TrySetSelectableCursor()) return;
        if (TrySetAttackableCursor()) return;
        if (TrySetUnAvailableCursor()) return;
        if (TrySetSellCursor()) return;
        if (TrySetGatheringCursor()) return;

        CursorManager.Instance.SetMarkerType(CursorManager.CursorType.None);
    }

    private bool RayHits(LayerMask mask, out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit, Mathf.Infinity, mask);
    }

    private bool TrySetWalkableCursor()
    {
        if (unitsSelected.Count > 0 && RayHits(ground, out _))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Walkable); 
            return true;
        }
        return false;
    }

    private bool TrySetSelectableCursor()
    {
        if (RayHits(clickable, out _))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Selectable);
            return true;
        }
        return false;
    }

    private bool TrySetAttackableCursor()
    {
        if (unitsSelected.Count > 0 && AtLeastOneOffensiveUnit(unitsSelected) && RayHits(attackable, out _))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Attackable);
            return true;
        }
        return false;
    }

    private bool TrySetUnAvailableCursor()
    {
        if (unitsSelected.Count > 0 && RayHits(constructable, out _))
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.UnAvailable);
            return true;
        }
        return false;
    }

    private bool TrySetSellCursor()
    {
        if (ResourceManager.Instance.placementSystem.inSellMode)
        {
            CursorManager.Instance.SetMarkerType(CursorManager.CursorType.SellCursor);
            return true;
        }
        return false;
    }

    private bool TrySetGatheringCursor()
    {
        if (unitsSelected.Count > 0 && OnlyHarvestersSelected())
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Collide))
            {
                if (hit.transform.GetComponent<ResourceNode>() != null)
                {
                    CursorManager.Instance.SetMarkerType(CursorManager.CursorType.Gathering);
                    return true;
                }
            }
        }
        return false;
    }

    private bool AtLeastOneOffensiveUnit(List<GameObject> unitsSelected)
    {
        foreach (GameObject unit in unitsSelected)
        {
            if (unit != null && unit.GetComponent<AttackController>())
            {
                return true;
            }
        }
        return false;
    }

    private void MultiSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            unitsSelected.Add(unit);
            SelectUnit(unit, true);
        }
        else
        {
            SelectUnit(unit, false);
            unitsSelected.Remove(unit);
        }
    }

    public void DeselectAll()
    {
        foreach (var unit in unitsSelected)
        {
            SelectUnit(unit, false);
        }

        groundMarker.SetActive(false);

        unitsSelected.Clear();
    }

    public void DragSelect(GameObject unit)
    {
        if (unitsSelected.Contains(unit) == false)
        {
            unitsSelected.Add(unit);
            SelectUnit(unit, true);
        }
    }

    private void SelectByClicking(GameObject unit)
    {
        DeselectAll();

        unitsSelected.Add(unit);

        SelectUnit(unit, true);
    }

    private void SelectUnit(GameObject unit, bool isSelected)
    {
        TriggerSelectionIndicator(unit, isSelected);
        EnableUnitMovement(unit, isSelected);
    }

    private void EnableUnitMovement(GameObject unit, bool shouldMove)
    {
        unit.GetComponent<UnitMovement>().enabled = shouldMove;
    }

    private void TriggerSelectionIndicator(GameObject unit, bool isVisible)
    {
        GameObject indicator = unit.transform.Find("Indicator").gameObject;

        if (!indicator.activeInHierarchy && !playedDuringThisDrag)
        {
            SoundManager.Instance.PlayUnitSelectionSound();
            playedDuringThisDrag = true;
        }

        indicator.SetActive(isVisible);
    }
}
