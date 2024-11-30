using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    Camera cam;
    NavMeshAgent agent;
    public LayerMask ground;
    public bool isCommandedToMove;

    Animator animator;

    private void Start()
    {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
            {
                isCommandedToMove = true;
                agent.SetDestination(hit.point);
                animator.SetBool("isMoving", true);
            }
        }

        // Agent reached destination
        if (agent.hasPath == false || agent.remainingDistance <= agent.stoppingDistance)
        {
            isCommandedToMove = false;
            animator.SetBool("isMoving", false);
        }
        else
        {
            animator.SetBool("isMoving", true);
        }
    }
}
