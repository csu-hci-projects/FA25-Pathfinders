using UnityEngine;
using System.Collections;

public class VoidOutZone : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float respawnDelay = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            StartCoroutine(HandleVoidOut(other.gameObject));
        }
    }

    private IEnumerator HandleVoidOut(GameObject player)
    {
        // Disable player movement script
        var movementScript = player.GetComponent<MovePlayer>();
        if (movementScript != null)
            movementScript.enabled = false;

        // Stop player velocity
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Short delay (optional)
        yield return new WaitForSeconds(respawnDelay);

        // Move player to respawn point
        if (respawnPoint != null)
        {
            player.transform.position = respawnPoint.position;
        }

        // Re-enable movement
        if (movementScript != null)
            movementScript.enabled = true;
    }
}
