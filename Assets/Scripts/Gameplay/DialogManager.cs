using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }
    private InputManager inputManager;

    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    public int LettersPerSecond;

    private Dialog dialog;
    private Action onDialogFinished;

    private int currentLine = 0;
    private bool isTyping;

    public bool IsShowing { get; private set; }

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    private void Awake()
    {
        Instance = this;
        inputManager = InputManager.Instance;
    }

    public void HandleUpdate()
    {
        if (inputManager.GetPlayerInteract() && !isTyping)
        {
            currentLine++;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {
                IsShowing = false;
                currentLine = 0;
                dialogBox.SetActive(false);
                onDialogFinished?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }

    public IEnumerator ShowDialog(Dialog dialog, Action onFinished = null)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();

        IsShowing = true;
        this.dialog = dialog;
        onDialogFinished = onFinished;

        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / LettersPerSecond);
        }
        isTyping = false;
    }
}
