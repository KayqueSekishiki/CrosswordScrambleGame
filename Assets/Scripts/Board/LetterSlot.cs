using System;

namespace Assets.Scripts.Board
{ 

    public class LetterSlot
    {
        char _correctLetter;
        char _attemptedLetter;
        int _index;
        bool _isOccupied;
        bool _isLocked;
        Word _parentWord;

        public char CorrectLetter => _correctLetter;
        public bool IsOccupied => _isOccupied;
        public bool IsLocked => _isLocked;
        public struct LetterEventHandler
        {
            public int Index { get; }
            public char Letter { get; }

            public LetterEventHandler(int index, char letter)
            {
                this.Index = index;
                this.Letter = letter;
            }
        }

        public EventHandler<LetterEventHandler> OnSlotOccupied;
        public EventHandler<LetterEventHandler> OnSlotFree;
        public EventHandler<LetterEventHandler> OnSlotLocked;
        public EventHandler<LetterEventHandler> OnLetterValidated;


        public LetterSlot(Word parentWord, int index, char correctLetter)
        {
            _parentWord = parentWord;
            _index = index;
            _correctLetter = correctLetter;
            _isOccupied = false;
            _isLocked = false;
        }

        //Validates the correctLetter and locks the slot if it is correct
        public int ValidateLetter(char letter)
        {
            //checks if the slot is free and not locked
            if (!_isOccupied && !_isLocked)
            {
                //occupies the slot
                OcupySlot();
                OnSlotOccupied?.Invoke(this, new LetterEventHandler(_index, CorrectLetter));

                //checks if the letter is correct
                if (letter == _correctLetter)
                {
                    //locks the slot
                    LockSlot();
                    OnSlotLocked?.Invoke(this, new LetterEventHandler(_index, CorrectLetter));
                    return (int)GameReturnCodes.Success;
                }
                else
                {
                    return (int)GameReturnCodes.Fail;
                }

              
            }
            else return (int)GameReturnCodes.NotAllowed;
        }
        //Sets this slot as occupied
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
                OnSlotOccupied?.Invoke(this, new LetterEventHandler(_index, CorrectLetter));
                return (int)GameReturnCodes.Success;
            }
        }
        //Frees the slot, removing the attemped correctLetter
        int FreeSlot()
        {
            //checks if the slot is occupied
            if (_isOccupied)
            {
                //frees the slot
                _isOccupied = false;
                OnSlotFree?.Invoke(this, new LetterEventHandler(_index, CorrectLetter));
                return (int)GameReturnCodes.Success;
            }
            else
            {
                return (int)GameReturnCodes.NotAllowed;
            }
        }

        //Sets the slot as locked, not allowing it to be freed
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
                OnSlotLocked?.Invoke(this, new LetterEventHandler(_index, CorrectLetter));
                return (int)GameReturnCodes.Success;
            }
        }


    }
}
