using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button buildButton;
    public PlacementSystem placement;

    private void Start()
    {
        buildButton.onClick.AddListener(() => Construct(0)); // Hard coded id of 0
    }

    private void Construct(int id)
    {
        Debug.Log("clicked");
        placement.StartPlacement(id);
    }
}
