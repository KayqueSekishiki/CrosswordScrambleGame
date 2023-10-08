using Assets.Scripts;
using Assets.Scripts.Board;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPuzzleSolvingPuzzleLetterSlot : MonoBehaviour
{
    //preciso de reference para meu index e para minha palavra
    [SerializeField]
    LetterSlot _slotReference;

    private UISolvingPuzzleManager solvingPuzzleManager;

    public LetterSlot SlotReference => _slotReference;

    public void SetupComponent(LetterSlot slotReference, UISolvingPuzzleManager uISolvingPuzzleManager)
    {
        _slotReference = slotReference;
        solvingPuzzleManager = uISolvingPuzzleManager;
    }

    public int AttemptToFindTheCorrectLetter(char letter)
    {
        return _slotReference.ValidateLetter(letter);
    }

    public void Btn_AttemptToSolveLetter()
    {
        if (solvingPuzzleManager.currentSelectedChoice.ToString() == "")
        {
            Debug.Log("Nenhuma letra escolhida");
            return;
        }
        switch ((GameReturnCodes)AttemptToFindTheCorrectLetter(solvingPuzzleManager.currentSelectedChoice))
        {
            case GameReturnCodes.Success:
                Debug.Log("Escolha Correta!");
                GetComponentInChildren<TextMeshProUGUI>().text = _slotReference.CorrectLetter.ToString();
                break;
            case GameReturnCodes.Fail: Debug.Log("Escolha Incorreta!"); break;
            case GameReturnCodes.NotAllowed:
                Debug.Log("Letra já resolvida!"); break;
                break;
        }

        if (solvingPuzzleManager.CurrentWordToSolve.IsWordComplete()) solvingPuzzleManager.SetWordAsSolved();

    }



}
