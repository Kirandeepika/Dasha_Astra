using UnityEngine;
using TMPro;

public class PuzzleTrigger : MonoBehaviour
{
    public GameObject puzzleBoard;
    public Camera playerCamera;
    public Camera puzzleCamera;

    public MonoBehaviour playerMovement;
    public ThirdPersonCamera thirdPersonCamera;

    public TextMeshProUGUI pressEText; // Assign in Inspector

    private bool playerInside = false;
    private bool puzzleOpen = false;

    void Start()
    {
        puzzleBoard.SetActive(false);
        puzzleCamera.enabled = false;
        playerCamera.enabled = true;

        if (pressEText != null)
            pressEText.gameObject.SetActive(false);

        AssignCameraToPipes();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            // Show Press E text
            if (pressEText != null)
                pressEText.gameObject.SetActive(true);

            Debug.Log("Player entered. Press E to open puzzle.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            // Hide Press E text
            if (pressEText != null)
                pressEText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInside && !puzzleOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenPuzzle();
        }

        if (puzzleOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePuzzle();
        }
    }

    void AssignCameraToPipes()
    {
        if (puzzleBoard == null || puzzleCamera == null) return;

        // true = include inactive children
        PipeDrag[] pipes = puzzleBoard.GetComponentsInChildren<PipeDrag>(true);

        Debug.Log($"Found {pipes.Length} pipes.");

        foreach (PipeDrag pipe in pipes)
        {
            pipe.SetCamera(puzzleCamera);
        }
    }

    void OpenPuzzle()
    {
        puzzleOpen = true;
        puzzleBoard.SetActive(true);

        playerMovement.enabled = false;

        playerCamera.enabled = false;
        puzzleCamera.enabled = true;

        // Hide Press E text when puzzle opens
        if (pressEText != null)
            pressEText.gameObject.SetActive(false);

        if (thirdPersonCamera != null)
            thirdPersonCamera.UnlockCursor();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        AssignCameraToPipes();
    }

    void ClosePuzzle()
    {
        puzzleOpen = false;
        puzzleBoard.SetActive(false);

        playerMovement.enabled = true;

        playerCamera.enabled = true;
        puzzleCamera.enabled = false;

        if (thirdPersonCamera != null)
            thirdPersonCamera.LockCursor();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Show Press E text again if player still inside trigger
        if (playerInside && pressEText != null)
            pressEText.gameObject.SetActive(true);

        GameProgress.Instance.puzzle1Solved = true;
    }
}