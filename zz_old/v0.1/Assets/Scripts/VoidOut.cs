using UnityEngine;
using System.Collections;

public class VoidOutZone : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float respawnDelay = 0.5f;
    [Tooltip("Small upward offset to avoid being exactly inside ground (optional)")]
    [SerializeField] private float respawnYOffset = 0.05f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            StartCoroutine(HandleVoidOut(other.gameObject));
        }
    }

    private IEnumerator HandleVoidOut(GameObject player)
    {
        // Disable player movement
        var movementScript = player.GetComponent<MovePlayer>();
        if (movementScript != null)
            movementScript.enabled = false;

        Rigidbody rb = player.GetComponent<Rigidbody>();

        bool rbExisted = rb != null;
        bool originallyKinematic = rbExisted && rb.isKinematic;

        // If Rigidbody exists and it was NOT kinematic, clear velocities
        if (rbExisted && !originallyKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // If Rigidbody exists and we need to ensure physics won't interfere while teleporting,
        // set it kinematic only if it wasn't already
        if (rbExisted && !originallyKinematic)
        {
            rb.isKinematic = true;
        }

        yield return new WaitForSeconds(respawnDelay);

        // Teleport player safely
        if (respawnPoint != null)
        {
            Vector3 targetPos = respawnPoint.position + Vector3.up * respawnYOffset;

            if (rbExisted)
            {
                rb.position = targetPos;
            }
            else
            {
                player.transform.position = targetPos;
            }
        }

        // Restore Rigidbody physics state and stabilize
        if (rbExisted)
        {
            if (!originallyKinematic)
            {
                rb.isKinematic = false;
                rb.Sleep();
                rb.WakeUp();
            }
        }

        // Re-enable movement
        if (movementScript != null)
            movementScript.enabled = true;
    }
}
