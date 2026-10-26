using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text gunText;
    [SerializeField] private TMP_Text cameraText;
    [SerializeField] private TMP_Text radioText;

    private InteractController _interactController;

    private void Awake()
    {
        _interactController = FindAnyObjectByType<InteractController>();
    }

    private void Update()
    {
        if (gunText.text[0] == '|') gunText.text = gunText.text[2..];
        if (cameraText.text[0] == '|') cameraText.text = cameraText.text[2..];
        if (radioText.text[0] == '|') radioText.text = radioText.text[2..];

        var text = GetItemText(_interactController.currentItem);
        if (text.text[0] != '|') text.text = "| " + text.text;
    }

    TMP_Text GetItemText(Item item)
    {
        return item switch
        {
            Item.Gun => gunText,
            Item.Camera => cameraText,
            Item.Radio => radioText,
            _ => null
        };
    }
}