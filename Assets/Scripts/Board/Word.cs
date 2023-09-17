using System;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Board
{
    public class Word
    {
        struct WordEventHandler
        {
            public int Index { get; }
            public string Word { get; }

            public WordEventHandler(int index, string word)
            {
                this.Index = index;
                this.Word = word;
            }

        }

        LetterSlot[] _letterSlots;
        EventHandler<WordEventHandler> _onWordCompleted;

        Board _parentBoard;
        int _index;
        string _word;

        Word(Board boardParent, int index, string word)
        {
            _parentBoard = boardParent;
            _index = index;
            _word = word;

            PopulateWord(_word);

            foreach (LetterSlot letterSlot in _letterSlots)
            {
                letterSlot.OnLetterValidated += OnLetterValidated;
                letterSlot.OnSlotOccupied += OnSlotOccupied;
                letterSlot.OnSlotFree += OnSlotFree;
                letterSlot.OnSlotLocked += OnSlotLocked;

            }
        }
        #region LetterSlotsEvents
        private void OnSlotLocked(object sender, LetterSlot.LetterEventHandler e)
        {
            //PLACE HOLDER
            throw new NotImplementedException();
        }

        private void OnSlotFree(object sender, LetterSlot.LetterEventHandler e)
        {
            //PLACE HOLDER
            throw new NotImplementedException();
        }

        private void OnSlotOccupied(object sender, LetterSlot.LetterEventHandler e)
        {
            //PLACE HOLDER
            throw new NotImplementedException();
        }

        private void OnLetterValidated(object sender, LetterSlot.LetterEventHandler e)
        {
            if (IsWordComplete() == 0)
            {
                _onWordCompleted.Invoke(this, new WordEventHandler(_index, _word));
            }
        }
        #endregion

        //Populates the LetterSlots with letters from the word
        private int PopulateWord(string newWord)
        {
            _letterSlots = new LetterSlot[newWord.Length];
            if (_letterSlots.Length == 0)
            {
                Debug.LogError("letter slots are empty");
                return (int)GameReturnCodes.Fail;
            }


            for (int i = 0; i < newWord.Length; i++)
            {
                _letterSlots[i] = new LetterSlot(this, i, newWord[i]);
            }

            return (int)GameReturnCodes.Success;
        }

        //Check if the word is completed
        public int IsWordComplete()
        {
            //checks if all the slots are locked (validated)
            for (int i = 0; i < _letterSlots.Length; i++)
            {
                if (!_letterSlots[i].IsLocked)
                {
                    return (int)GameReturnCodes.Fail;
                }
            }
            return (int)GameReturnCodes.Success;
        }

        //Force complete word
        public int ForceCompleteWord()
        {
            //checks if all the slots are locked (validated)
            for (int i = 0; i < _letterSlots.Length; i++)
            {
                if (!_letterSlots[i].IsLocked)
                {
                    _letterSlots[i].ValidateLetter(_letterSlots[i].CorrectLetter);
                }
            }

            _onWordCompleted.Invoke(this, new WordEventHandler(_index, _word));
            return (int)GameReturnCodes.Success;
        }

        //Force restart word
        public int ForceRestartWord()
        {
            PopulateWord(_word);
            return (int)GameReturnCodes.Success;
        }

    }
}
