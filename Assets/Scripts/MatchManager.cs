using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Assets.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

using Assets.Scripts.Board;
public class Match : MonoBehaviour
{
    [Serializable]
    public struct BoardWords
    {
        public string Word;
        public Vector2 InitialGridPosition;
        public bool IsHorizontal;
    }

    [SerializeField] private int _gridSize = 5;

    private Dictionary<Vector2, LetterSlot> _letterSlotsDictionary = new Dictionary<Vector2, LetterSlot>();

    [SerializeField] private BoardWords[] _gameWordsHolderList;
    private Board _board;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //Initiate the board
    private int InitiateBoard()
    {
        if (_gameWordsHolderList.Length != 0)
        {
            if (_board != null)
            {
                Debug.Log("Board already created");
                return (int)GameReturnCodes.InvalidParameter;
            }
            _letterSlotsDictionary.Clear();
            string[] allWords;

            //grab all words from the game words holder list
            allWords = new string[_gameWordsHolderList.Length];
            for (int i = 0; i < _gameWordsHolderList.Length; i++)
            {
                allWords[i] = _gameWordsHolderList[i].Word;
            }

            //create a new board
            _board = new Board(allWords);

            Debug.Log("Board created");
            return (int)GameReturnCodes.Success;
        }
        else
        {
            Debug.LogError("No words were found in the game words holder list");
            return (int)GameReturnCodes.InvalidParameter;
        }
    }

    //Draw gizmos each letter slot as cubes
    private void OnDrawGizmos()
    {
        InitiateBoard();


        //draws the grid
        Gizmos.color = Color.white;
        for (int i = 0; i < _gridSize; i++)
        {
            for (int j = 0; j < _gridSize; j++)
            {
                Gizmos.DrawWireCube(new Vector3(i * 100, j * 100, 0), Vector3.one * 100);

            }
        }

        //draws the words
        for (int x = 0; x < _gameWordsHolderList.Length; x++)
        {
            BoardWords boardWord = _gameWordsHolderList[x];
            Word actualWordEquivalent = _board.Words[x];

            if (boardWord.IsHorizontal)
            {

                for (int i = 0; i < boardWord.Word.Length; i++)
                {

                    Vector2 letterGridPosition = new Vector2(boardWord.InitialGridPosition.x + i,
                        boardWord.InitialGridPosition.y);

                    //check if its over the grid
                    if (boardWord.InitialGridPosition.x + i >= _gridSize)
                    {
                        Debug.LogError("Word " + boardWord.Word + " is out of the grid");
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.cyan;
                    }




                    Gizmos_ValidateOverlapingLetterSlots(letterGridPosition, actualWordEquivalent, i, boardWord);

                    Gizmos.DrawWireCube(
                        new Vector3((boardWord.InitialGridPosition.x + i) * 100, boardWord.InitialGridPosition.y * 100,
                            0), Vector3.one * 100);
                }
            }
            else
            {
                for (int i = 0; i < boardWord.Word.Length; i++)
                {

                    Vector2 letterGridPosition = new Vector2(boardWord.InitialGridPosition.x,
                        boardWord.InitialGridPosition.y + i);

                    //check if its over the grid
                    if (boardWord.InitialGridPosition.y + i >= _gridSize)
                    {
                        Debug.LogError("Word " + boardWord.Word + " is out of the grid");
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.cyan;
                    }




                    Gizmos_ValidateOverlapingLetterSlots(letterGridPosition, actualWordEquivalent, i, boardWord);


                    Gizmos.DrawWireCube(
                        new Vector3(boardWord.InitialGridPosition.x * 100, (boardWord.InitialGridPosition.y + i) * 100,
                            0), Vector3.one * 100);
                }
            }
        }
    }

    private void Gizmos_ValidateOverlapingLetterSlots(Vector2 letterGridPosition, Word word, int letterIndexInsideWord,
        BoardWords boardWord)
    {
        if (Gizmos_IsLetterSlotOverlaping(letterGridPosition, out LetterSlot alreadyUsedChosenSlot))
        {
            //we already have a letter here. 

            //Check if its the same letter
            if (alreadyUsedChosenSlot.CorrectLetter == word.LetterSlots[letterIndexInsideWord].CorrectLetter)
            {
                Debug.LogWarning("Word " + boardWord.Word + " is overlapping with another word with the CORRECT letter");
                Gizmos.color = Color.green;
            }
            else
            {
                //does not match
                Debug.LogError("Word " + boardWord.Word + " is overlapping with another word with the WRONG letter}");
                Gizmos.color = Color.red;
            }
        }
        else
        {
            if (word.LetterSlots[letterIndexInsideWord] != null) _letterSlotsDictionary.TryAdd(letterGridPosition, word.LetterSlots[letterIndexInsideWord]);
        }

    }

    private bool Gizmos_IsLetterSlotOverlaping(Vector2 letterGridPosition, out LetterSlot chosenSlot)
    {
        //check if word is overlapping
        _letterSlotsDictionary.TryGetValue(letterGridPosition, out chosenSlot);

        if (chosenSlot != null)
        {
            return true;
        }
        return false;
    }
}
