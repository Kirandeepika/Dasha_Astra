using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance;

    public bool puzzle1Solved;
    public bool puzzle2Solved;

    private void Awake()
    {
        Instance = this;
    }

    public bool AllPuzzlesSolved()
    {
        return puzzle1Solved && puzzle2Solved;
    }
}