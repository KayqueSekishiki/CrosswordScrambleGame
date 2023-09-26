using System;
using UnityEngine;

namespace Assets.Scripts.Board
{
    /// <summary>
    /// Represents a board in a match.
    /// </summary>
    public class Board
    {
        private Word[] _words;  // Array to hold Word objects.

        public Word[] Words => _words;  // Public getter for _words.
        [Serializable]
        public struct BoardWords
        {
            public string Word;  // The word itself.
            public Vector2 InitialGridPosition;  // The initial grid position for the word.
            public bool IsHorizontal;  // Flag indicating if the word is horizontal.


            public BoardWords(string word, Vector2 gridPosition, bool isHorizontal)
            {
                Word = word;
                InitialGridPosition = gridPosition;
                IsHorizontal = isHorizontal;
            }
        }

        /// <summary>
        /// Check if the board is complete, i.e., all words are complete.
        /// </summary>
        /// <returns>True if complete, else false.</returns>
        bool IsBoardComplete()
        {
            foreach (Word word in _words)
            {
                if (!word.IsWordComplete())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Initializes a new instance of the Board class.
        /// Create the Words which will create the letter slots, and register to the Words required events
        /// </summary>
        /// <param name="words">Array of BoardWords struct.</param>
        public Board(BoardWords[] words)
        {
            _words = new Word[words.Length];

            if (_words.Length != 0)
            {// if we have any words, then we actually initialize it.
                for (int i = 0; i < words.Length; i++)
                {
                    _words[i] = new Word(this, i, words[i].Word, words[i].IsHorizontal, words[i].InitialGridPosition);
                }

                foreach (Word fullWord in _words)
                {
                    //register to word events
                    fullWord.OnWordCompleted += OnWordCompleted;
                }
            }
          
        }


        /// <summary>
        /// Destructor to unregister event handlers.
        /// </summary>
        ~Board()
        {

            foreach (Word fullWord in _words)
            {
                //unregister to word events
                fullWord.OnWordCompleted -= OnWordCompleted;
            }
        }

        /// <summary>
        /// Event handler for when a word is completed.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments containing the word data.</param>
        private void OnWordCompleted(object sender, Word.WordEventHandler e)
        {
            Debug.Log("Word " + e.Word + " completed");
            if (IsBoardComplete())
            {
                Debug.Log("Board completed");
            }
        }

        /// <summary>
        /// Completes all words in the board.
        /// </summary>
        /// <returns>Game return code.</returns>
        int CompleteBoard()
        {
            int returnCode = (int)GameReturnCodes.Success;

            foreach (Word word in _words)
            {
                returnCode = word.CompleteWord();
                if (returnCode != (int)GameReturnCodes.Success)
                {
                    return returnCode;
                }
            }

            return returnCode;
        }

        /// <summary>
        /// Restarts all words in the board.
        /// </summary>
        /// <returns>Game return code.</returns>
        int RestartBoard()
        {
            int returnCode = (int)GameReturnCodes.Success;

            foreach (Word word in _words)
            {
                returnCode = word.RestartWord();
                if (returnCode != (int)GameReturnCodes.Success)
                {
                    return returnCode;
                }
            }

            return returnCode;
        }
    }
}
