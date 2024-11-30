using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform localTrans;

    public Camera facingCamera;

    private void Start()
    {
        localTrans = GetComponent<Transform>();
    }

    private void Update()
    {
        if (facingCamera)
        {
            localTrans.LookAt(2 * localTrans.position - facingCamera.transform.position);
        }
    }
}
