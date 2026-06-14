using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public DoorOpener doorOpener;
    public GameObject pressEText;

    bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            if (GameProgress.Instance.AllPuzzlesSolved())
            {
                pressEText.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            pressEText.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInside &&
            Input.GetKeyDown(KeyCode.E) &&
            GameProgress.Instance.AllPuzzlesSolved())
        {
            pressEText.SetActive(false);

            doorOpener.OpenDoors();
        }
    }
}