using UnityEngine;
using UnityEngine.AI;

public class Harvester : MonoBehaviour
{
    public Transform assignedNode; // Assigned resource node
    public Transform supplyCenter; // Supply center for depositing resources
    public float harvestAmountPerSecond = 1f; // Harvest rate

    public float maxCapacity = 10f; // Max carrying capacity

    public float currentCapacity = 0f; // Current resource load
    private NavMeshAgent agent;
    private Animator animator;
    private bool isDepositing = false; // Tracks if currently in Depositing state

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Update Animator parameters
        animator.SetBool("hasAssignedNode", assignedNode != null);
        animator.SetBool("isFull", currentCapacity >= maxCapacity);

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
        agent.SetDestination(target.position);
    }

    public void Harvest()
    {
        if (assignedNode == null) return;

        if (assignedNode != null && !assignedNode.GetComponent<ResourceNode>().IsDepleted)
        {
            // Simulate harvesting
            assignedNode.GetComponent<ResourceNode>().Harvest(harvestAmountPerSecond * Time.deltaTime);
            currentCapacity += harvestAmountPerSecond * Time.deltaTime;

            // Clamp capacity
            if (currentCapacity > maxCapacity)
            {
                currentCapacity = maxCapacity;
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
        float tempCapacity = currentCapacity;

        while (currentCapacity > 0f)
        {
            currentCapacity -= 1 * Time.deltaTime;

            // Ensure currentCapacity doesn't go below 0
            if (currentCapacity < 0f)
            {
                currentCapacity = 0f;
            }

            yield return null; // Wait for the next frame
        }

        ResourceManager.Instance.IncreaseResource(ResourceManager.ResourcesType.Credits, ConvertResourceIntoCredits(tempCapacity));

        isDepositing = false;


        // Transition back to GoingToHarvest
        if (assignedNode != null)
        {
            animator.SetTrigger("doneDepositing");
        }

    }

    private int ConvertResourceIntoCredits(float tempCapacity)
    {
        return ((int)tempCapacity * ResourceManager.Instance.creditsPerKiloSpice);
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
