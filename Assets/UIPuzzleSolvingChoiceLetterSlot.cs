using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPuzzleSolvingChoiceLetterSlot : MonoBehaviour
{
    public char myLetter;
    UISolvingPuzzleManager _UISolvingPuzzleManager;
 
    public void SetupComponent(char letter, UISolvingPuzzleManager manager)
    {
        _UISolvingPuzzleManager = manager;
        myLetter = letter;
    }
    //refactor this later
    private void SetMyLetterAsCurrentSelectedLetter()
    {
        _UISolvingPuzzleManager.currentSelectedChoice = myLetter;
    }


    public void Btn_SelectThisLetterAsChoice()
    {
        SetMyLetterAsCurrentSelectedLetter();
    }
}
