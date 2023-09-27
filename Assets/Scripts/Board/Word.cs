using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Board
{
    /// <summary>
    /// Represents a word on the game board.
    /// </summary>
    public class Word
    {
        /// <summary>
        /// Event handler structure for when a word is completed.
        /// </summary>
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
        /// <summary>
        /// Public accessor for the LetterSlot array.
        /// </summary>
        public LetterSlot[] LetterSlots => _letterSlots;

        public string GetWord => _word;

        public bool IsWordHorizontal => _isWordHorizontal;

        public Vector2 WordInitialGridPosition => _wordInitialGridPosition;

        /// <summary>
        /// Event triggered when a word is completed.
        /// </summary>
        public EventHandler<WordEventHandler> OnWordCompleted;
        public EventHandler<WordEventHandler> OnWordUpdated;

        Board _parentBoard;
        int _index;
        string _word;
        private readonly bool _isWordHorizontal;
        private readonly Vector2 _wordInitialGridPosition;
        
        /// <summary>
        /// Constructor to initialize a Word instance.
        /// </summary>
        /// <param name="boardParent">Parent board object.</param>
        /// <param name="index">Index of the word.</param>
        /// <param name="word">Actual word string.</param>
        /// <param name="isWordHorizontal">Orientation of the word.</param>
        /// <param name="wordInitialGridPosition">Initial grid position of the word.</param>
        public Word(Board boardParent, int index, string word, bool isWordHorizontal, Vector2 wordInitialGridPosition)
        {
            _parentBoard = boardParent;
            _index = index;
            _word = word;
            _isWordHorizontal = isWordHorizontal;
            _wordInitialGridPosition = wordInitialGridPosition;

            PopulateWord(GetWord, isWordHorizontal, wordInitialGridPosition);

            foreach (LetterSlot letterSlot in _letterSlots)
            {
                letterSlot.OnLetterValidated += OnLetterValidated;
                letterSlot.OnSlotOccupied += OnSlotOccupied;
                letterSlot.OnSlotFree += OnSlotFree;
                letterSlot.OnSlotLocked += OnSlotLocked;

            }
        }

        /// <summary>
        /// Destructor to unregister from slot events.
        /// </summary>
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
            OnWordUpdated.Invoke(this, new WordEventHandler(_index, GetWord));
        }

        private void OnSlotFree(object sender, LetterSlot.LetterEventHandler e)
        {
            //PLACE HOLDER
             
        }

        private void OnSlotOccupied(object sender, LetterSlot.LetterEventHandler e)
        {
            //PLACE HOLDER
            
        }


        private void OnLetterValidated(object sender, LetterSlot.LetterEventHandler e)
        {
            OnWordUpdated.Invoke(this, new WordEventHandler(_index, GetWord));
            if (IsWordComplete())
            {
                OnWordCompleted.Invoke(this, new WordEventHandler(_index, GetWord));
            }

        }
        #endregion

        /// <summary>
        /// Populates the word into LetterSlots.
        /// </summary>
        /// <param name="newWord">The word to populate.</param>
        /// <param name="isHorizontal">Orientation of the word.</param>
        /// <param name="initialGridPosition">Initial position in the grid.</param>
        /// <returns>Success or failure code.</returns>
        private int PopulateWord(string newWord, bool isHorizontal, Vector2 initialGridPosition)
        {
            _letterSlots = new LetterSlot[newWord.Length];
            if (_letterSlots.Length == 0)
            {
                Debug.LogError("letter slots are empty");
                return (int)GameReturnCodes.Fail;
            }


            for (int i = 0; i < newWord.Length; i++)
            {
                Vector2 letterGridPosition = new Vector2(initialGridPosition.x, initialGridPosition.y);

                if (isHorizontal) letterGridPosition.Set(letterGridPosition.x + i, letterGridPosition.y);
                else letterGridPosition.Set(letterGridPosition.x, letterGridPosition.y + i);

                _letterSlots[i] = new LetterSlot(this, letterGridPosition, i, newWord[i]);
            }

            return (int)GameReturnCodes.Success;
        }

        /// <summary>
        /// Checks if the word is complete.
        /// </summary>
        /// <returns>True if complete, false otherwise.</returns>
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

        /// <summary>
        /// Forces the completion of the word.
        /// </summary>
        /// <returns>Success or failure code.</returns>
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

            OnWordCompleted.Invoke(this, new WordEventHandler(_index, GetWord));
            return (int)GameReturnCodes.Success;
        }

        /// <summary>
        /// Forces the restart of the word.
        /// </summary>
        /// <returns>Success or failure code.</returns>
        public int RestartWord()
        {
            PopulateWord(GetWord,IsWordHorizontal,WordInitialGridPosition);
            return (int)GameReturnCodes.Success;
        }

    }
}
