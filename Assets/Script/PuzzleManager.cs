using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public Slot[] slots;
    public GameObject puzzleSolvedPopup;
    public GameObject puzzleUI; // Drag your puzzle canvas here

    private bool solved = false;

    public void CheckPuzzleSolved()
    {
        if (solved) return;

        foreach (Slot slot in slots)
        {
            if (!slot.occupied)
            {
                return;
            }
        }

        solved = true;

        puzzleSolvedPopup.SetActive(true);

        Debug.Log("Puzzle Solved!");

        Invoke(nameof(ClosePuzzle), 2f); // Wait 2 seconds
    }

    void ClosePuzzle()
    {
        puzzleUI.SetActive(false);
    }
}