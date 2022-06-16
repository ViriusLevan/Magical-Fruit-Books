using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Typer : MonoBehaviour
{
    public GameObject bookPanel;
    public bool isBookOpen=false;
    public WordBank wordbank = null;
    public TextMeshProUGUI wordOutput=null;

    private string remainingWord = string.Empty;
    private string currentWord = string.Empty;
    private Book.FruitType fruitToClaim;
    private int fruitClaimN;
    private Book bookPendingDestruction;
    public delegate void OnWordCompleted(Book.FruitType ft, int i);
    public static event OnWordCompleted wordCompleted;
    public delegate void OnBookClosed();
    public static event OnBookClosed bookClosed;

    void Start()
    {
        SetCurrentWord();
        Book.openedBook += OpenBook;
    }

    void OnDestroy()
    {
        Book.openedBook -= OpenBook;
    }

    void Update()
    {
        if(isBookOpen){
            CheckInput();
        }
    }

    public void OpenBook(Book.FruitType ft, int ftN, Book bpd)
    {
        bookPendingDestruction = bpd;
        fruitToClaim = ft;
        fruitClaimN = ftN;
        isBookOpen=true;
        bookPanel.SetActive(true);
        SetCurrentWord();
    }

    public void CloseBook()
    {
        isBookOpen=false;
        bookPanel.SetActive(false);
        bookClosed?.Invoke();
    }

    private void SetCurrentWord()
    {
        currentWord = wordbank.GetWord();
        SetRemainingWord(currentWord);
    }

    private void SetRemainingWord(string nString)
    {
        remainingWord = nString;
        wordOutput.text = remainingWord;
    }

    private void CheckInput()
    {
        if(Input.anyKeyDown)
        {
            string keysPressed = Input.inputString;

            if(keysPressed.Length==1)
            {
                EnterLetter(keysPressed);
            }
        }
    }

    private void EnterLetter(string typedLetter)
    {
        if(IsCorrectLetter(typedLetter))
        {
            RemoveLetter();

            if(IsWordComplete())
            {
                wordCompleted?.Invoke(fruitToClaim, fruitClaimN);
                CloseBook();
                bookPendingDestruction.DestroyBook();
            }
        }
    }

    private bool IsCorrectLetter(string letter)
    {
        return remainingWord.IndexOf(letter) == 0;
    }

    private void RemoveLetter()
    {
        string newString = remainingWord.Remove(0,1);
        SetRemainingWord(newString);
    }

    private bool IsWordComplete()
    {
        return remainingWord.Length == 0;
    }
}
