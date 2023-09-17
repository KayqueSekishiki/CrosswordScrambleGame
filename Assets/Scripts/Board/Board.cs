using UnityEngine;

namespace Assets.Scripts.Board
{
    public class Board
    {
        Word[] _words;

        public Word[] Words => _words;

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

        public Board(string[] words)
        {
            _words = new Word[words.Length];
            if (_words.Length != 0)
            {
                for (int i = 0; i < words.Length; i++)
                {
                    _words[i] = new Word(this, i, words[i]);
                }
            }

            foreach (Word fullWord in _words)
            {
                //register to word events
                fullWord.OnWordCompleted += OnWordCompleted;
            }
        }

        ~Board()
        {

            foreach (Word fullWord in _words)
            {
                //unregister to word events
                fullWord.OnWordCompleted -= OnWordCompleted;
            }
        }


        private void OnWordCompleted(object sender, Word.WordEventHandler e)
        {
            Debug.Log("Word " + e.Word + " completed");
            if (IsBoardComplete())
            {
                Debug.Log("Board completed");
            }
        }

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
