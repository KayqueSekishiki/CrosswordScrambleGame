using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Board;
using UnityEngine;

public class UILetterSlotHolder : MonoBehaviour
{
    private LetterSlot _slotReference;

    virtual public LetterSlot SlotReference => _slotReference;

    virtual public void StashLetterSlotReference(LetterSlot slot) 
    {
        _slotReference = slot;
    }


    public void OpenWordSolverWindow()
    {
        FindObjectOfType<UISolvingPuzzleManager>().StartSolvingWord(_slotReference.ParentWord);
    }
}
