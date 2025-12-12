using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CameraZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomedZ = -1f;      // Z distance when zoomed in
    public float normalZ = -5f;      // Z distance when zoomed out
    public float zoomSpeed = 4f;     // Interpolation speed

    private CinemachineCamera[] cmCameras;
    private Coroutine zoomRoutine;

    private void Start()
    {
        // Find all CinemachineCamera components in the scene (modern API)
        cmCameras = Object.FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);

        Debug.Log($"[CameraZoom] Found {cmCameras.Length} CinemachineCamera objects.");
        foreach (var cam in cmCameras)
        {
            string followTarget = cam.Follow != null ? cam.Follow.name : "(none)";
            Debug.Log($"  Camera: {cam.name}   Follow: {followTarget}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other.tag)) return;

        var cam = FindCameraForPlayer(other.transform);
        if (cam == null)
        {
            Debug.LogWarning($"[CameraZoom] No camera found for player '{other.name}'");
            return;
        }

        var followComp = cam.GetComponent<CinemachineThirdPersonFollow>();
        if (followComp == null)
        {
            Debug.LogWarning($"Camera '{cam.name}' has no ThirdPersonFollow component!");
            return;
        }

        StartZoom(followComp, zoomedZ);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other.tag)) return;

        var cam = FindCameraForPlayer(other.transform);
        if (cam == null) return;

        var followComp = cam.GetComponent<CinemachineThirdPersonFollow>();
        if (followComp == null) return;

        StartZoom(followComp, normalZ);
    }

    private bool IsPlayer(string tag)
    {
        return tag == "Player1" || tag == "Player2";
    }

    private CinemachineCamera FindCameraForPlayer(Transform playerCollider)
    {
        if (cmCameras == null || cmCameras.Length == 0) return null;

        foreach (var cam in cmCameras)
        {
            if (cam == null || cam.Follow == null) continue;

            // ✅ Check if collider is the follow target or a child of it
            if (playerCollider == cam.Follow || playerCollider.IsChildOf(cam.Follow))
                return cam;

            // Optional fallback: match by tag
            if (playerCollider.CompareTag(cam.Follow.tag))
                return cam;
        }

        return null;
    }

    private void StartZoom(CinemachineThirdPersonFollow follow, float targetZ)
    {
        if (zoomRoutine != null)
            StopCoroutine(zoomRoutine);

        zoomRoutine = StartCoroutine(SmoothZoom(follow, targetZ));
    }

    private IEnumerator SmoothZoom(CinemachineThirdPersonFollow follow, float targetZ)
    {
        float startZ = follow.ShoulderOffset.z;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * zoomSpeed;

            Vector3 offset = follow.ShoulderOffset;
            offset.z = Mathf.Lerp(startZ, targetZ, t);
            follow.ShoulderOffset = offset;

            yield return null;
        }

        // Ensure final value is exactly targetZ
        Vector3 final = follow.ShoulderOffset;
        final.z = targetZ;
        follow.ShoulderOffset = final;
    }
}