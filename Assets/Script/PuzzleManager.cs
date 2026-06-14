using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public Slot[] slots;
    public GameObject puzzleSolvedPopup;
    public GameObject puzzleUI;

    public int puzzleID; // Set 1 for Puzzle 1, 2 for Puzzle 2

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

        Invoke(nameof(ClosePuzzle), 2f);
    }

    void ClosePuzzle()
    {
        puzzleUI.SetActive(false);

        if (puzzleID == 1)
        {
            GameProgress.Instance.puzzle1Solved = true;
        }
        else if (puzzleID == 2)
        {
            GameProgress.Instance.puzzle2Solved = true;
        }
    }
}