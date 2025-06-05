using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Harvester : MonoBehaviour
{
    public Transform assignedNode; // Assigned resource node
    public Transform supplyCenter; // Supply center for depositing resources
    public float harvestAmountPerSecond = 1f; // Harvest rate

    public float maxCapacity = 10f; // Max carrying capacity

    //public float currentCapacity = 0f; // Current resource load
    private NavMeshAgent agent;
    private Animator animator;
    private bool isDepositing = false; // Tracks if currently in Depositing state

    public Slider capacitySlider;

    private float _currentCapacity;

    public ResourceType currentResourceType;

    public float CurrentCapacity
    {
        get => _currentCapacity;
        set
        {
            _currentCapacity = Mathf.Clamp(value, 0, maxCapacity);
            UpdateSlider();
        }
    }

    private void UpdateSlider()
    {
        if (capacitySlider != null)
        {
            float normalized = CurrentCapacity / maxCapacity;
            float scaled = normalized * 10f;
            capacitySlider.value = Mathf.Round(scaled);
        }
    }

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        currentResourceType = ResourceType.None;
    }

    void Update()
    {
        // Update Animator parameters
        animator.SetBool("hasAssignedNode", assignedNode != null);
        animator.SetBool("isFull", CurrentCapacity >= maxCapacity);
        animator.SetBool("isNotEmpty", CurrentCapacity > 0);

        // Check for node depletion
        if (assignedNode != null && assignedNode.GetComponent<ResourceNode>().IsDepleted)
        {
            assignedNode = null; // Clear the node
            animator.SetBool("hasAssignedNode", false); // Trigger Idle state
        }
    }

    public void MoveTo(Transform target)
    {
        if (target == null) return;
        UnitMovement unitMovement = GetComponent<UnitMovement>();
        if (unitMovement != null)
        {
            unitMovement.MoveTo(target.position, false); // Automatic via state machine = false
        }
    }

    public void CancelHarvesting()
    {
        assignedNode = null;
        animator.SetBool("hasAssignedNode", false);
        animator.SetBool("atNode", false);
    }

    public void Harvest()
    {
        if (assignedNode == null) return;

        if (assignedNode != null && !assignedNode.GetComponent<ResourceNode>().IsDepleted)
        {
            ResourceNode node = assignedNode.GetComponent<ResourceNode>();
            currentResourceType = node.resourceType;

            // Simulate harvesting
            node.Harvest(harvestAmountPerSecond * Time.deltaTime);
            CurrentCapacity += harvestAmountPerSecond * Time.deltaTime;

            // Clamp capacity
            if (CurrentCapacity > maxCapacity)
            {
                CurrentCapacity = maxCapacity;
            }
        }
    }

    public void DepositResources()
    {
        if (isDepositing) return;

        isDepositing = true;

        StartCoroutine(WaitInDepositingState());
    }

    private System.Collections.IEnumerator WaitInDepositingState()
    {
        float tempCapacity = CurrentCapacity;

        while (CurrentCapacity > 0f)
        {
            CurrentCapacity -= 1 * Time.deltaTime;

            // Ensure currentCapacity doesn't go below 0
            if (CurrentCapacity < 0f)
            {
                CurrentCapacity = 0f;
            }

            yield return null; // Wait for the next frame
        }

        //ResourceManager.Instance.IncreaseResource(ResourceType.Credits, ConvertResourceIntoCredits(tempCapacity));

        HandleResourceDeposit(tempCapacity, currentResourceType);

        isDepositing = false;


        // Transition back to GoingToHarvest
        if (assignedNode != null)
        {
            animator.SetTrigger("doneDepositing");
        }

        currentResourceType = ResourceType.None;
    }

    private void HandleResourceDeposit(float tempCapacity, ResourceType currentResourceType)
    {
        switch(currentResourceType)
        {
            case ResourceType.Oil:
                ResourceManager.Instance.IncreaseResource(ResourceType.Oil, (int)tempCapacity);
                return;
            case ResourceType.Gold:
                ResourceManager.Instance.IncreaseResource(ResourceType.Credits, ((int)tempCapacity * ResourceManager.CREDITS_PER_KILO_GOLD));
                return;
        }
    }

    private void OnTriggerEnter(Collider other) // Use onTriggerStay instead to check if the agent stopped moving.
    {
        if (assignedNode != null && other.transform == assignedNode)
        {
            // Reached the resource node
            animator.SetBool("atNode", true);
        }
        else if (supplyCenter != null && other.transform == supplyCenter)
        {
            // Reached the supply center
            animator.SetBool("atSupply", true);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (assignedNode != null && other.transform == assignedNode)
        {
            // Left the resource node
            animator.SetBool("atNode", false);
        }
        else if (supplyCenter != null && other.transform == supplyCenter)
        {
            // Left the supply center
            animator.SetBool("atSupply", false);
        }
    }
}
