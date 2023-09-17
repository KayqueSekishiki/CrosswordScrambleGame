using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Board
{
    public class Word
    {
        public struct WordEventHandler
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
        public LetterSlot[] LetterSlots => _letterSlots;
        
        public EventHandler<WordEventHandler> OnWordCompleted;

        Board _parentBoard;
        int _index;
        string _word;

        public Word(Board boardParent, int index, string word)
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

        ~Word()
        {
            foreach (LetterSlot letterSlot in _letterSlots)
            {
                letterSlot.OnLetterValidated -= OnLetterValidated;
                letterSlot.OnSlotOccupied -= OnSlotOccupied;
                letterSlot.OnSlotFree -= OnSlotFree;
                letterSlot.OnSlotLocked -= OnSlotLocked;
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
            if (IsWordComplete())
            {
                OnWordCompleted.Invoke(this, new WordEventHandler(_index, _word));
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
        public bool IsWordComplete()
        {
            //checks if all the slots are locked (validated)
            for (int i = 0; i < _letterSlots.Length; i++)
            {
                if (!_letterSlots[i].IsLocked)
                {
                    return false;
                }
            }
            return true;
        }

        //Force complete word
        public int CompleteWord()
        {
            //checks if all the slots are locked (validated)
            for (int i = 0; i < _letterSlots.Length; i++)
            {
                if (!_letterSlots[i].IsLocked)
                {
                    _letterSlots[i].ValidateLetter(_letterSlots[i].CorrectLetter);
                }
            }

            OnWordCompleted.Invoke(this, new WordEventHandler(_index, _word));
            return (int)GameReturnCodes.Success;
        }

        //Force restart word
        public int RestartWord()
        {
            PopulateWord(_word);
            return (int)GameReturnCodes.Success;
        }

    }
}
