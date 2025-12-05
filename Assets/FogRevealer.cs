using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogRevealer : MonoBehaviour
{
    public float personalRevealArea = 5;

    // Start is called before the first frame update
    void Start()
    {
        if (FogOfWarManager.Instance != null)
        {
            FogOfWarManager.Instance.RegisterVisionSource(this);
        }
    }

    private void OnEnable()
    {
        if (FogOfWarManager.Instance != null)
        {
            FogOfWarManager.Instance.RegisterVisionSource(this);
        }
    }

    private void OnDisable()
    {
        if (FogOfWarManager.Instance != null)
        {
            FogOfWarManager.Instance.UnregisterVisionSource(this);
        }
    }
}
