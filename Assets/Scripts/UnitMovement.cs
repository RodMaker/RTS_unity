using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    Camera cam;
    NavMeshAgent agent;
    public LayerMask ground;
    public bool isCommandedToMove;

    DirectionIndicator directionIndicator;

    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();

        directionIndicator = GetComponent<DirectionIndicator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && IsMovingPossible())
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                MoveTo(hit.point, true); // Player command = true

                // Additional movement actions
                SoundManager.Instance.PlayUnitCommandSound();

                directionIndicator.DrawLine(hit);
            }
        }

        // Agent reached destination
        /*
        if (agent.hasPath == false || agent.remainingDistance <= agent.stoppingDistance)
        {
            isCommandedToMove = false;
        }
        */
    }

    public void MoveTo(Vector3 position, bool isPlayerCommand)
    {
        isCommandedToMove = true;
        StartCoroutine(NoCommand());
        agent.SetDestination(position);

        if (isPlayerCommand)
        {
            Harvester harvester = GetComponent<Harvester>();
            if (harvester != null)
            {
                if (harvester.assignedNode == null || Vector3.Distance(harvester.assignedNode.position, position) > 1f)
                {
                    harvester.CancelHarvesting();
                }
            }
        }
    }

    private bool IsMovingPossible()
    {
        return CursorManager.Instance.currentCursor != CursorManager.CursorType.UnAvailable;
    }

    IEnumerator NoCommand()
    {
        yield return new WaitForSeconds(1);
        isCommandedToMove = false;
    }
}
