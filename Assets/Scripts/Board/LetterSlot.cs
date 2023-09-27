using System;
using UnityEngine;

namespace Assets.Scripts.Board
{
    /// <summary>
    /// Represents a letter slot in a word on the game board.
    /// </summary>
    public class LetterSlot
    {
        char _correctLetter;
        char _attemptedLetter;
        int _index;
        bool _isOccupied;
        bool _isLocked;
        Word _parentWord;
        Vector2 _gridPosition;

        /// <summary>
        /// Public accessor for the correct letter.
        /// </summary>
        public char CorrectLetter => _correctLetter;
        /// <summary>
        /// Public accessor to check if the slot is occupied.
        /// </summary>
        public bool IsOccupied => _isOccupied;
        /// <summary>
        /// Public accessor to check if the slot is locked.
        /// </summary>
        public bool IsLocked => _isLocked;
        /// <summary>
        /// Public accessor for the slot's grid position.
        /// </summary>
        public Vector2 GridPosition => _gridPosition;

        public Word ParentWord => _parentWord;

        /// <summary>
        /// Event handler structure for letter slot events.
        /// </summary>
        public struct LetterEventHandler
        {
            public int Index { get; }
            public char Letter { get; }
            public Vector2 GridPosition { get; }

            /// <summary>
            /// Initializes the event handler with index, letter, and grid position.
            /// </summary>
            /// <param name="index">Index of the letter slot.</param>
            /// <param name="letter">Correct letter.</param>
            /// <param name="gridPosition">Position of the slot in the grid.</param>
            public LetterEventHandler(int index, char letter, Vector2 gridPosition)
            {
                this.Index = index;
                this.Letter = letter;
                this.GridPosition = gridPosition;
            }
        }

        public EventHandler<LetterEventHandler> OnSlotOccupied;
        public EventHandler<LetterEventHandler> OnSlotFree;
        public EventHandler<LetterEventHandler> OnSlotLocked;
        public EventHandler<LetterEventHandler> OnLetterValidated;

        /// <summary>
        /// Initializes a new instance of the LetterSlot class.
        /// </summary>
        /// <param name="parentWord">Reference to the parent Word object.</param>
        /// <param name="gridPosition">Position in the grid.</param>
        /// <param name="index">Index of the slot.</param>
        /// <param name="correctLetter">Correct letter for the slot.</param>
        public LetterSlot(Word parentWord, Vector2 gridPosition, int index, char correctLetter)
        {
            _parentWord = parentWord;
            _gridPosition = gridPosition;
            _index = index;
            _correctLetter = correctLetter;
            _isOccupied = false;
            _isLocked = false;
        }

        /// <summary>
        /// Validates the input letter and locks the slot if correct.
        /// </summary>
        /// <param name="letter">Input letter to validate.</param>
        /// <returns>Game return code indicating success, failure, or not allowed.</returns>
        public int ValidateLetter(char letter)
        {
            //checks if the slot is free and not locked
            if (!_isOccupied && !_isLocked)
            {
                //occupies the slot
                OcupySlot();
                OnSlotOccupied?.Invoke(this, new LetterEventHandler(_index, CorrectLetter, _gridPosition));

                //checks if the letter is correct
                if (letter == _correctLetter)
                {
                    //locks the slot
                    LockSlot();
                    OnSlotLocked?.Invoke(this, new LetterEventHandler(_index, CorrectLetter, _gridPosition));
                    return (int)GameReturnCodes.Success;
                }
                else
                {
                    return (int)GameReturnCodes.Fail;
                }


            }
            else return (int)GameReturnCodes.NotAllowed;
        }

        /// <summary>
        /// Checks whether or not this slot is inside the grid
        /// </summary>
        /// <remarks>
        /// It verifies this by checking if either the x or y grid position from this slot
        /// has a value greater then or equal to the <paramref name="gridSize"/>
        /// </remarks>
        /// <param name="gridSize">grid's size</param>
        /// <returns></returns>
        public bool IsThisSlotOutsideOfTheGrid(int gridSize)
        {
            return GridPosition.x >= gridSize || GridPosition.y >= gridSize;
        }

        /// <summary>
        /// Occupies the letter slot.
        /// </summary>
        /// <returns>Game return code indicating success or not allowed.</returns>
        int OcupySlot()
        {
            //checks if the slot is being occupied already
            if (_isOccupied)
            {
                return (int)GameReturnCodes.NotAllowed;
            }
            else
            {
                //occupies the slot
                _isOccupied = true;
                OnSlotOccupied?.Invoke(this, new LetterEventHandler(_index, CorrectLetter, _gridPosition));
                return (int)GameReturnCodes.Success;
            }
        }

        /// <summary>
        /// Frees the occupied slot.
        /// </summary>
        /// <returns>Game return code indicating success or not allowed.</returns>
        int FreeSlot()
        {
            //checks if the slot is occupied
            if (_isOccupied)
            {
                //frees the slot
                _isOccupied = false;
                OnSlotFree?.Invoke(this, new LetterEventHandler(_index, CorrectLetter, _gridPosition));
                return (int)GameReturnCodes.Success;
            }
            else
            {
                return (int)GameReturnCodes.NotAllowed;
            }
        }

        /// <summary>
        /// Locks the letter slot.
        /// </summary>
        /// <returns>Game return code indicating success or not allowed.</returns>
        int LockSlot()
        {
            //checks if the slot is already locked
            if (_isLocked)
            {
                return (int)GameReturnCodes.NotAllowed;
            }
            else
            {
                //locks the slot
                _isLocked = true;
                OnSlotLocked?.Invoke(this, new LetterEventHandler(_index, CorrectLetter, _gridPosition));
                return (int)GameReturnCodes.Success;
            }
        }

      
    }
}
