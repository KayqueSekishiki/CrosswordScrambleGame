using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionModalText : MonoBehaviour
{
    public TextMeshProUGUI ModalText;
    public string TextToPrint;

    public List<GameObject> buttonsList;

    private void OnEnable()
    {
        ModalText.text = TextToPrint;
    }
}
