using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerCam : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;

    void FixedUpdate()
    {
        transform.position = cameraPosition.position;
    }
}
