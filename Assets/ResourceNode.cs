using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public float resources = 100f; // Max amount
    public float depletionRate = 1f;

    public bool IsDepleted => resources <= 0;

    public void Harvest(float amount)
    {
        if (!IsDepleted)
        {
            resources -= amount;
            if (resources < 0)
            {
                resources = 0;
            }
        }
    }
}
