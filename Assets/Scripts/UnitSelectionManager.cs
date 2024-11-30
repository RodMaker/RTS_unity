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

    public GameObject groundMarker;

    private Camera cam;

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
    }

    private bool AtLeastOneOffensiveUnit(List<GameObject> unitsSelected)
    {
        foreach (GameObject unit in unitsSelected)
        {
            if (unit.GetComponent<AttackController>())
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
        unit.transform.Find("Indicator").gameObject.SetActive(isVisible);
    }
}
