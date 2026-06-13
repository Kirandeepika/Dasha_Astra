using UnityEngine;

public class PipeDrag : MonoBehaviour
{
    private bool dragging = false;
    private float distance;

    private bool isPlaced = false;
    private Slot currentSlot;

    private PuzzleManager puzzleManager;
    private bool isSelected = false;
    private Camera mainCamera;

    [Header("Sound Effects")]
    public AudioClip pickupSound;
    public AudioClip placeSound;
    public AudioClip removeSound;
    public AudioClip errorSound; // plays when pipe dropped on invalid slot

    private AudioSource audioSource;

    void Start()
    {
        puzzleManager = FindObjectOfType<PuzzleManager>();
        mainCamera = Camera.main;

        // Add AudioSource automatically
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void SetCamera(Camera cam)
    {
        mainCamera = cam;
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    void OnMouseDown()
    {
        if (mainCamera == null) return;

        if (!isPlaced)
        {
            distance = mainCamera.WorldToScreenPoint(transform.position).z;
            dragging = true;
            isSelected = true;

            PlaySound(pickupSound); // Play pickup sound
        }
        else
        {
            isSelected = true;
        }
    }

    void OnMouseUp()
    {
        if (!isPlaced)
        {
            dragging = false;
            bool placed = TryPlacePipe(transform.position);

            if (!placed)
                PlaySound(errorSound); // Play error if not placed on valid slot
        }
    }

    void Update()
    {
        if (mainCamera == null) return;

        // Dragging
        if (dragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = distance;
            transform.position = mainCamera.ScreenToWorldPoint(mousePos);
        }

        // Remove from slot with Space key — only for selected pipe
        if (isSelected && isPlaced && Input.GetKeyDown(KeyCode.Space))
        {
            RemoveFromSlot();
        }

        // Deselect when clicking elsewhere
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform != transform)
                {
                    isSelected = false;
                }
            }
            else
            {
                isSelected = false;
            }
        }
    }

    bool TryPlacePipe(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, 0.5f);

        foreach (Collider hit in hits)
        {
            Slot slot = hit.GetComponent<Slot>();

            if (slot != null && !slot.occupied)
            {
                transform.position = slot.transform.position;

                slot.occupied = true;
                currentSlot = slot;
                isPlaced = true;

                PlaySound(placeSound); // Play place sound

                Debug.Log("Pipe Placed");

                if (puzzleManager != null)
                {
                    puzzleManager.CheckPuzzleSolved();
                }

                return true; // Successfully placed
            }
        }

        return false; // Not placed
    }

    void RemoveFromSlot()
    {
        if (currentSlot != null)
        {
            currentSlot.occupied = false;
        }

        currentSlot = null;
        isPlaced = false;

        distance = mainCamera.WorldToScreenPoint(transform.position).z;
        dragging = true;

        PlaySound(removeSound); // Play remove sound

        Debug.Log("Pipe Removed");
    }
}