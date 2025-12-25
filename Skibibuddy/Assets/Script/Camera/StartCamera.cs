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
    public GameObject gameRoot; // Reference to the "Game" GameObject containing Player, HUD, etc.
    public Avalanche avalanche; 
    public PlayerController playerController; 
    public GameObject crosshairUI; // Reference to the Crosshair Canvas/Image
    public GameObject mainMenuCanvas; // Reference to the Main Menu Canvas
    public GameObject hudCanvas; // Reference to the HUD GameObject

    private Camera thisCam;

    void Awake()
    {
        thisCam = GetComponent<Camera>();
        
        // Ensure Game Root is inactive initially
        if (gameRoot != null)
            gameRoot.SetActive(false);

        // Ensure Main Menu is active initially
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(true);
    }

    void Start()
    {
        // Unlock cursor for menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnStartGame()
    {
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);

        // Enable Game Root
        if (gameRoot != null)
            gameRoot.SetActive(true);

        // Initialize references and disable them for the intro sequence
        InitializeGameComponents();

        // Lock cursor for game
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(CameraIntroSequence());
    }

    void InitializeGameComponents()
    {
        // Auto-find Avalanche if not assigned
        if (avalanche == null)
            avalanche = FindObjectOfType<Avalanche>();

        // Disable Avalanche immediately so it waits for the camera sequence
        if (avalanche != null)
            avalanche.enabled = false;

        // Auto-find PlayerController if not assigned
        if (playerController == null && player != null)
            playerController = player.GetComponent<PlayerController>();

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

        // Disable Crosshair initially
        if (crosshairUI != null)
            crosshairUI.SetActive(false);

        // Disable HUD initially
        if (hudCanvas != null)
            hudCanvas.SetActive(false);
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

        // 7) Enable Crosshair
        if (crosshairUI != null)
        {
            crosshairUI.SetActive(true);
        }

        // 8) Enable HUD
        if (hudCanvas != null)
        {
            hudCanvas.SetActive(true);
        }

        // 9) Reset Player Camera Rotation to face Z-forward
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

        if (crosshairUI != null)
            crosshairUI.SetActive(true);

        if (hudCanvas != null)
            hudCanvas.SetActive(true);
    }
}
