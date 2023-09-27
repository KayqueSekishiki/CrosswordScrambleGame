using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Board;
using UnityEngine;

public class UILetterSlotHolder : MonoBehaviour
{
    private LetterSlot _slotReference;

    public LetterSlot SlotReference => _slotReference;

    public void StashLetterSlotReference(LetterSlot slot)
    {
        _slotReference = slot;
    }


    public void OpenWordSolverWindow()
    {

    }
}
