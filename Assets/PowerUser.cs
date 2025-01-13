using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUser : MonoBehaviour
{
    public int producingPower;
    public int consumingPower;

    public bool isProducer;

    public void PowerOn()
    {
        if (isProducer)
        {
            PowerManager.Instance.AddPower(producingPower);
        }
        else // Is consumer
        {
            PowerManager.Instance.ConsumePower(consumingPower);
        }
    }

    private void OnDestroy()
    {
        if (GetComponent<Constructable>().inPreviewMode == false) // Only run the code if not in preview
        {
            if (isProducer)
            {
                PowerManager.Instance.RemovePower(producingPower);
            }
            else // Is consumer
            {
                PowerManager.Instance.ReleasePower(consumingPower);
            }
        }
    }
}
