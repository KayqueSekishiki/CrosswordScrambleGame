using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Board;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts
{
    public class Match : MonoBehaviour
    {

        [Header("Setup")]
        [SerializeField] private int _gridSize = 5;
        [SerializeField] private bool _runDebugGridMode;
        GameObject[,] gridObjectsMatrix;

        [Header("Prefab References")]
        [SerializeField] private Canvas _boardCanvas;
        [SerializeField] private GameObject _letterSlotPrefab;
        [SerializeField] private GameObject _letterSlotPrefabVariant;


        [SerializeField] private Board.Board.BoardWords[] _gameWordsHolderList;
        private Board.Board _board;


        void Start()
        {
            InitiateBoard();
        }

        void CreateALetterSlotOnTheBoard(Transform parent, LetterSlot letterSlot, bool variant = false)
        {
            GameObject letterSlotPrefab = variant ? _letterSlotPrefabVariant : _letterSlotPrefab;


            // Instantiate prefab at calculated screen position
            GameObject slot = Instantiate(letterSlotPrefab, Vector3.zero, Quaternion.identity, parent);

            slot.GetComponentInChildren<TextMeshProUGUI>().SetText("");
        }


        Vector3 GridToScreenPosition(Vector2 ParentSize, Vector2 gridPosition)
        {
            Vector2 canvasOffset = _boardCanvas.GetComponent<RectTransform>().anchoredPosition;

            float xFactor = ParentSize.x / (_gridSize - 1);  // -1 because we start from zero
            float yFactor = ParentSize.y / (_gridSize - 1); // -1 because we start from zero

            float xPos = xFactor * gridPosition.x + canvasOffset.x;
            float yPos = yFactor * gridPosition.y + canvasOffset.y;


            return new Vector3(xPos, yPos, 0);
        }


        /// <summary>
        /// Initialize the board, by actually saving the correct grid position for each letter slot of each chosen word.
        /// </summary>
        /// <returns>GameReturnCodes as integer.</returns>
        private int InitiateBoard()
        {

            if (_gameWordsHolderList.Length != 0)
            {

                //if (_board != null)
                //{
                //    Debug.Log("Board already created");
                //    return (int)GameReturnCodes.InvalidParameter;
                //}

                //create a new board
                _board = new Board.Board(_gameWordsHolderList);

                Debug.Log("Board created");

                //debug initialization ends here
                if (_runDebugGridMode) return (int)GameReturnCodes.Success;

                gridObjectsMatrix = new GameObject[_gridSize, _gridSize];

                GameObject GridGameObject = new GameObject("Grid");
                GridLayoutGroup layoutGroup;
                layoutGroup = GridGameObject.AddComponent<GridLayoutGroup>();
                layoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                layoutGroup.startCorner = GridLayoutGroup.Corner.LowerLeft;
                layoutGroup.childAlignment = TextAnchor.LowerLeft;
                layoutGroup.constraintCount = _gridSize;
                GridGameObject.transform.SetParent(_boardCanvas.transform, false);


                GridGameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                GridGameObject.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
                GridGameObject.GetComponent<RectTransform>().pivot = new Vector2(0, 0);

                for (int y = 0; y < _gridSize; y++)
                {
                    for (int x = 0; x < _gridSize; x++)
                    {
                        // Instantiate prefab at calculated screen position
                        GameObject slot = Instantiate(_letterSlotPrefab, Vector3.zero, Quaternion.identity, GridGameObject.transform);
                        slot.GetComponent<Image>().enabled = false;
                        slot.GetComponentInChildren<TextMeshProUGUI>().SetText("");

                        gridObjectsMatrix[x, y] = slot;
                    }
                }

                foreach (Word word in _board.Words)
                {

                    foreach (LetterSlot letterSlot in word.LetterSlots)
                    {
                        gridObjectsMatrix[(int)letterSlot.GridPosition.x,(int)letterSlot.GridPosition.y]
                            .GetComponent<Image>().enabled = true;

                        gridObjectsMatrix[(int)letterSlot.GridPosition.x, (int)letterSlot.GridPosition.y].GetComponent<UILetterSlotHolder>()
                            .StashLetterSlotReference(letterSlot);

                    }
                   
                    word.OnWordUpdated+= OnWordUpdated;

                }

                return (int)GameReturnCodes.Success;




            }
            else
            {
                Debug.LogError("No words were found in the game words holder list");
                return (int)GameReturnCodes.InvalidParameter;
            }
        }

        private void OnWordUpdated(object sender, Word.WordEventHandler e)
        {
            Word word = (Word)sender;

            foreach (LetterSlot letterSlot in word.LetterSlots)
            {
                if (letterSlot.IsLocked)
                {
                    gridObjectsMatrix[(int)letterSlot.GridPosition.x,(int)letterSlot.GridPosition.y].GetComponentInChildren<TextMeshProUGUI>().SetText(letterSlot.CorrectLetter.ToString());
                }

            }
        }

        private void SetupLayoutGroup(GridLayoutGroup layoutGroup, GameObject worldHolder)
        {
            // You can then set various layout properties
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;

            // Attach the new object to the Canvas

        }

#if UNITY_EDITOR

        /// <summary>
        /// Draws the board grid and words using Gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {

            if (!_runDebugGridMode) return;

            InitiateBoard();

            //draws the grid
            Gizmos.color = Color.white;
            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    Vector3 cubeLocation = new Vector3(i * 100, j * 100, 0);
                    Gizmos.DrawWireCube(cubeLocation, Vector3.one * 100);

                    GUIStyle handleGUIStyle = new GUIStyle
                    {
                        alignment = TextAnchor.UpperCenter,
                        fontSize = 10,
                    };
                    handleGUIStyle.normal.textColor = Color.grey;

                    Handles.Label(cubeLocation + new Vector3(0, -20, 0), $"({i},{j})", handleGUIStyle);
                }
            }

            //draws the words
            for (int x = 0; x < _gameWordsHolderList.Length; x++)
            {

                Word actualWordEquivalent = _board.Words[x];

                for (int i = 0; i < actualWordEquivalent.GetWord.Length; i++)
                {

                    Vector2 letterGridPosition = actualWordEquivalent.LetterSlots[i].GridPosition;
                    Vector3 slotGridLocation = new Vector3(letterGridPosition.x * 100, letterGridPosition.y * 100, 0);


                    //check if its over the grid
                    if (actualWordEquivalent.LetterSlots[i].IsThisSlotOutsideOfTheGrid(_gridSize))
                    {
                        Debug.Log(i, this);
                        string horizontalState = actualWordEquivalent.IsWordHorizontal ? "Horizontal Word" : "Vertical Word";
                        Debug.LogError("Word " + actualWordEquivalent.GetWord + " is out of the grid"
                        + $"\n the grid has size {_gridSize}\n This word is a {horizontalState} with size {actualWordEquivalent.GetWord.Length}" +
                        $"\n The word starts at position ({actualWordEquivalent.WordInitialGridPosition.x},{actualWordEquivalent.WordInitialGridPosition.y})");
                        Gizmos.color = Color.yellow;



                    }
                    else
                    {
                        Gizmos.color = Color.cyan;
                    }

                    Gizmos_ValidateOverlapingLetterSlots(letterGridPosition, actualWordEquivalent, i, actualWordEquivalent);

                    Gizmos.DrawWireCube(slotGridLocation, Vector3.one * 100);

                    GUIStyle handleGUIStyle = new GUIStyle
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 25,
                    };
                    handleGUIStyle.normal.textColor = Gizmos.color;

                    Handles.Label(slotGridLocation, $"{actualWordEquivalent.LetterSlots[i].CorrectLetter}", handleGUIStyle);

                    GUIStyle gridPosHandleGUIStyle = new GUIStyle
                    {
                        alignment = TextAnchor.UpperCenter,
                        fontSize = 12,
                    };
                    gridPosHandleGUIStyle.normal.textColor = Gizmos.color;



                    Handles.Label(slotGridLocation + new Vector3(0, 40, 0), $"({actualWordEquivalent.LetterSlots[i].GridPosition.x}" +
                                                                            $",{actualWordEquivalent.LetterSlots[i].GridPosition.y})", gridPosHandleGUIStyle);

                }


            }
        }
        /// <summary>
        /// Validate whether a letter slot in the grid is already being used.
        /// </summary>
        /// <param name="letterGridPosition">Position of the letter in the grid.</param>
        /// <param name="word">Word being drawn.</param>
        /// <param name="letterIndexInsideWord">Index of the letter inside the word.</param>
        /// <param name="boardWord">BoardWords object that represents the word.</param>
        private void Gizmos_ValidateOverlapingLetterSlots(Vector2 letterGridPosition, Word word, int letterIndexInsideWord,
            Word boardWord)
        {
            if (Gizmos_IsLetterSlotOverlaping(letterGridPosition, out LetterSlot alreadyUsedChosenSlot))
            {
                //we already have a letter here. 

                //Check if its the same letter
                if (alreadyUsedChosenSlot.CorrectLetter == word.LetterSlots[letterIndexInsideWord].CorrectLetter)
                {
                    Debug.LogWarning("Word " + boardWord.GetWord + " is overlapping with another word with the CORRECT letter");
                    Gizmos.color = Color.green;
                }
                else
                {
                    //does not match
                    Debug.LogError("Word " + boardWord.GetWord + " is overlapping with another word with the WRONG letter -> "
                                   + "the letter you are trying to overlap with is " + word.LetterSlots[letterIndexInsideWord].CorrectLetter + " the letter being overlaped is " + alreadyUsedChosenSlot.CorrectLetter);
                    Gizmos.color = Color.red;
                }
            }
        }

        /// <summary>
        /// Check if a letter slot is overlapping with another.
        /// </summary>
        /// <param name="letterGridPosition">Position of the letter in the grid.</param>
        /// <param name="chosenSlot">Out parameter to return the overlapped slot.</param>
        /// <returns>True if overlapping, else false.</returns>
        private bool Gizmos_IsLetterSlotOverlaping(Vector2 letterGridPosition, out LetterSlot chosenSlot)
        {
            //check if word is overlapping

            Word anotherLetterIsHere = _board.Words.FirstOrDefault(word => word.LetterSlots.Any((slot => slot.GridPosition.Equals(letterGridPosition))));

            if (anotherLetterIsHere == null)
            {
                chosenSlot = null;
                return false;
            }

            chosenSlot = anotherLetterIsHere.LetterSlots.FirstOrDefault(slot => slot.GridPosition == letterGridPosition);

            if (chosenSlot != null)
            {
                return true;
            }
            return false;
        }

#endif


    }
}
