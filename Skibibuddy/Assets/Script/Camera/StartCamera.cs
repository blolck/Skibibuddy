using System.Collections;
using UnityEngine;

public class StartCamera : MonoBehaviour
{
    [Header("Targets")]
    public Transform player;                    
    public Transform firstPersonCamTarget;     
    public Camera firstPersonCamera;           

    [Header("Timings")]
    public float aimDuration = 2f;            
    public float holdDelay = 0.8f;            
    public float transitionDuration = 0.25f;    

    [Header("Aim Settings")]
    public Vector3 aimOffset = new Vector3(0, 1.5f, -6f); 

    [Header("Behavior")]
    public bool activateFirstPersonCameraOnFinish = true; 
    
    [Header("Game Flow")]
    public Avalanche avalanche; 
    public PlayerController playerController; 

    private Camera thisCam;

    void Awake()
    {
        thisCam = GetComponent<Camera>();
        
        // Auto-find Avalanche if not assigned
        if (avalanche == null)
            avalanche = FindObjectOfType<Avalanche>();

        // Disable Avalanche immediately so it waits for the camera sequence
        if (avalanche != null)
            avalanche.enabled = false;

        // Auto-find PlayerController if not assigned
        if (playerController == null && player != null)
            playerController = player.GetComponent<PlayerController>();
        else if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        // Disable PlayerController immediately
        if (playerController != null)
        {
            playerController.enabled = false;
   
            Rigidbody rb = playerController.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.freezeRotation = true;
            }
        }
    }

    void Start()
    {
        StartCoroutine(CameraIntroSequence());
    }

    IEnumerator CameraIntroSequence()
    {
        if (player == null || firstPersonCamTarget == null)
        {
            Debug.LogWarning("StartCamera: player ");
            FinishSwitchImmediate();
            yield break;
        }

        // 1) Aim player
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 aimPos = player.position + aimOffset;
        Quaternion aimRot = Quaternion.LookRotation(player.position - aimPos, Vector3.up);

        float t = 0f;
        while (t < aimDuration)
        {
            t += Time.deltaTime;
            float frac = Mathf.SmoothStep(0f, 1f, t / aimDuration);
            transform.position = Vector3.Lerp(startPos, aimPos, frac);
            transform.rotation = Quaternion.Slerp(startRot, aimRot, frac);
            yield return null;
        }

        transform.position = aimPos;
        transform.rotation = aimRot;

        yield return new WaitForSecondsRealtime(holdDelay);

        // 
        startPos = transform.position;
        startRot = transform.rotation;
        Vector3 targetPos = firstPersonCamTarget.position;
        Quaternion targetRot = firstPersonCamTarget.rotation;

        t = 0f;
        while (t < transitionDuration)
        {
            t += Time.unscaledDeltaTime; // unscaled  Time.timeScale 
            float frac = Mathf.SmoothStep(0f, 1f, t / transitionDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, frac);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, frac);
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = targetRot;

        // 4)FirstPerson Camera
        if (activateFirstPersonCameraOnFinish)
        {
            if (firstPersonCamera != null)
            {
                // Fix 2: Ensure FirstPersonCamera renders Skybox
                // Sometimes switching cameras can cause Skybox issues if ClearFlags are wrong
                if (firstPersonCamera.clearFlags == CameraClearFlags.Depth || firstPersonCamera.clearFlags == CameraClearFlags.Nothing)
                {
                    firstPersonCamera.clearFlags = CameraClearFlags.Skybox;
                }

                firstPersonCamera.enabled = true;
                if (thisCam != null) thisCam.enabled = false;
            }
            else
            {
        
                transform.SetParent(firstPersonCamTarget);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }

     
        if (avalanche != null)
        {
            avalanche.enabled = true;
        }

        // 6) Enable PlayerController
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // 7) Reset Player Camera Rotation to face Z-forward
        if (firstPersonCamera != null)
        {
            PlayerCam pcCam = firstPersonCamera.GetComponent<PlayerCam>();
            if (pcCam != null)
            {
                pcCam.ResetCameraRotation();
            }
        }
    }

    void FinishSwitchImmediate()
    {
        if (firstPersonCamTarget != null)
        {
            transform.position = firstPersonCamTarget.position;
            transform.rotation = firstPersonCamTarget.rotation;
            if (activateFirstPersonCameraOnFinish && firstPersonCamera != null)
            {
                firstPersonCamera.enabled = true;
                if (thisCam != null) thisCam.enabled = false;
            }
        }
    }
}
