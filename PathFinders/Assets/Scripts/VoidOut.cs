using UnityEngine;
using System.Collections;

public class VoidOutZone : MonoBehaviour
{
    [Header("Respawn Points")]
    [SerializeField] private Transform respawnPoint1;
    [SerializeField] private Transform respawnPoint2;

    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 0.5f;
    [SerializeField] private float respawnYOffset = 0.05f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            StartCoroutine(HandleVoidOut(other.gameObject));
        }
    }

    private IEnumerator HandleVoidOut(GameObject player)
    {
        // Disable movement script
        var movement = player.GetComponent<MovePlayer>();
        if (movement != null)
            movement.enabled = false;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        bool rbExisted = rb != null;

        // Freeze physics if RB exists
        if (rbExisted)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        yield return new WaitForSeconds(respawnDelay);

        // Pick correct spawn point
        Transform targetRespawn = null;

        if (player.CompareTag("Player1"))
            targetRespawn = respawnPoint1;

        else if (player.CompareTag("Player2"))
            targetRespawn = respawnPoint2;

        // Teleport safely
        if (targetRespawn != null)
        {
            Vector3 pos = targetRespawn.position + Vector3.up * respawnYOffset;

            if (rbExisted)
                rb.position = pos;
            else
                player.transform.position = pos;

            player.transform.rotation = targetRespawn.rotation;
        }

        // Restore physics
        if (rbExisted)
        {
            rb.isKinematic = false;
            rb.Sleep();
            rb.WakeUp();
        }

        // Re-enable movement
        if (movement != null)
            movement.enabled = true;
    }
}