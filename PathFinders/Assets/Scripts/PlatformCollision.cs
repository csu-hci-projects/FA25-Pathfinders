using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformCollision : MonoBehaviour
{
    [SerializeField] Transform platform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            other.gameObject.transform.parent = platform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            other.gameObject.transform.parent = null;
        }
    }
}