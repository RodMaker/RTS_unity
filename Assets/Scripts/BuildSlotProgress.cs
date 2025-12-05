using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildSlotProgress : MonoBehaviour
{
    [SerializeField] private Image progressImage;
    private float buildTime;
    private float startTime;
    private bool isBuilding;

    public void StartBuilding(float time)
    {
        buildTime = time;
        startTime = Time.time;
        isBuilding = true;
        progressImage.fillAmount = 1f; // Full at start
        progressImage.enabled = true;

        gameObject.GetComponentInParent<Button>().interactable = false;
    }

    private void Update()
    {
        if (!isBuilding)
        {
            return;
        }

        float elapsed = Time.time - startTime;
        float progress = elapsed / buildTime;
        progressImage.fillAmount = Mathf.Clamp01(1f - progress); // Count down from 1 to 0

        if (elapsed >= buildTime)
        {
            isBuilding = false;
            progressImage.fillAmount = 0f;
            progressImage.enabled = false;
            OnBuildComplete();
        }
    }

    private void OnBuildComplete()
    {
        gameObject.GetComponentInParent<Button>().interactable = true;
    }
}
