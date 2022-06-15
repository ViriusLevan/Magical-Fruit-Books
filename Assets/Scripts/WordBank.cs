using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WordBank : MonoBehaviour
{
    private List<string> originalWords = new List<string>()
    {
        "Fruits","Delicious","Vibrant"
    };

    private List<string> workingWords = new List<string>();

    private void Awake()
    {
        InitializeWorkingWords();
    }

    private void InitializeWorkingWords()
    {
        workingWords.AddRange(originalWords);
        ConvertToLower(workingWords);
        Shuffle(workingWords);
    }

    private void Shuffle(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int random = Random.Range(i, list.Count);
            string temporary = list[i];

            list[i] = list[random];
            list[random] = temporary;
        }
    }

    private void ConvertToLower(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i] = list[i].ToLower();
        }
    }

    public string GetWord()
    {
        string newWord = string.Empty;
        
        if(workingWords.Count==0)
        {
            InitializeWorkingWords();
        }
        
        newWord = workingWords.Last();
        workingWords.Remove(newWord);

        return newWord;
    }
}
