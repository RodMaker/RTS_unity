using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; set; }

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


    private GameObject markerInstance;  // The instantiated marker object
    bool isMarkerActive = false;  // Boolean to control marker activation
    float markerHeight = 0.0f;  // Height at which the marker should be placed (e.g., ground level)

    [Header("Marker Prefabs")]
    public GameObject walkableCursor;
    public GameObject selectableCursor;
    public GameObject attackableCursor;
    public GameObject unAvailableCursor;
    public GameObject sellingModeCursor;


    public CursorType currentCursor;


    public enum CursorType
    {
        None,
        Walkable,
        UnAvailable,
        Selectable,
        Attackable,
        SellCursor
    }

    void Update()
    {
        // Check if the marker should be active
        if (isMarkerActive)
        {
            // Show the marker and hide the cursor
            markerInstance.SetActive(true);
            Cursor.visible = false;

            // Position the marker at the mouse position
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            Plane groundPlane = new Plane(Vector3.up, new Vector3(0, markerHeight, 0));
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 worldPosition = ray.GetPoint(rayDistance);
                markerInstance.transform.position = worldPosition;
            }
        }
        else
        {
            // Hide the marker and show the cursor
            markerInstance?.SetActive(false);

            Cursor.visible = true;
        }
    }

    public void SetMarkerType(CursorType type)
    {
        if (type != currentCursor)
        {
            isMarkerActive = true;

            currentCursor = type;

            switch (type)
            {
                case CursorType.Walkable:
                    markerInstance?.SetActive(false);
                    markerInstance = walkableCursor;
                    return;
                case CursorType.Selectable:
                    markerInstance?.SetActive(false);
                    markerInstance = selectableCursor;
                    return;
                case CursorType.Attackable:
                    markerInstance?.SetActive(false);
                    markerInstance = attackableCursor;
                    return;
                case CursorType.UnAvailable:
                    markerInstance?.SetActive(false);
                    markerInstance = unAvailableCursor;
                    return;
                case CursorType.SellCursor:
                    markerInstance?.SetActive(false);
                    markerInstance = sellingModeCursor;
                    return;
                case CursorType.None:
                    markerInstance?.SetActive(false);
                    isMarkerActive = false;
                    return;
            }
        }
    }
}
