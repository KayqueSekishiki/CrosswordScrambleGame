using Assets.Scripts.Board;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Assets.Scripts.Board.Word;

public class UISolvingPuzzleManager : MonoBehaviour
{

    [SerializeField]
    GameObject _puzzleLettersGridParent;
    [SerializeField]
    GameObject _choiceLettersGridParent;
    [SerializeField]
    GameObject _SolvePuzzleCanvas;
    [SerializeField]
    TextMeshProUGUI _wordTipTMP;

    [SerializeField]
    GameObject _puzzleSlotsPrefab;
    [SerializeField]
    GameObject _choiceSlotsPrefab;

    Word currentWordToSolve;


    public char currentSelectedChoice;

    public Word CurrentWordToSolve { get => currentWordToSolve;}
    private void Start()
    {
        currentSelectedChoice = default;
    }

    public void StartSolvingWord(Word wordToSolve)
    {
        currentWordToSolve = wordToSolve;
        _wordTipTMP.SetText(CurrentWordToSolve.GetTip);

        InitiateAttemptToSolveWord();

        _SolvePuzzleCanvas.SetActive(true);
         
    }

   public void SetWordAsSolved()
    {
        StopSolvingWord();  
    }
    public void StopSolvingWord()
    {
        _SolvePuzzleCanvas.SetActive(false);

        foreach (Transform child in _puzzleLettersGridParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void InitiateAttemptToSolveWord()
    {
        PopulatePuzzleLattersGrid(CurrentWordToSolve);
        PopulateChoiceLattersGrid();
    }

    void PopulatePuzzleLattersGrid(Word word)
    {
        //Clear slots
        foreach (Transform child in _puzzleLettersGridParent.transform)
        {
            DestroyImmediate(child.gameObject);
        }

        foreach (LetterSlot letterSlot in word.LetterSlots)
        {
            GameObject puzzleLetter = Instantiate(_puzzleSlotsPrefab, _puzzleLettersGridParent.transform);
            puzzleLetter.GetComponent<UIPuzzleSolvingPuzzleLetterSlot>().SetupComponent(letterSlot, this);

            if (letterSlot.IsLocked)
            {
                puzzleLetter.GetComponentInChildren<TextMeshProUGUI>().text = letterSlot.CorrectLetter.ToString();
            }
            else
            {
                puzzleLetter.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }

        }
    }

    void PopulateChoiceLattersGrid()
    {
        string alphabet = "aãábcdeéêfghijklmnoóôpqrstuvwxyz";

        //no need to spawn if it did already previously
        if (_choiceLettersGridParent.transform.childCount == alphabet.Length) return;


        foreach (char letter in alphabet)
        {
            GameObject choiceLetter = Instantiate(_choiceSlotsPrefab, _choiceLettersGridParent.transform);
            choiceLetter.GetComponent<UIPuzzleSolvingChoiceLetterSlot>().SetupComponent(letter, this);
            choiceLetter.GetComponentInChildren<TextMeshProUGUI>().text = letter.ToString();
        }
    }

}
