using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    public delegate void OnBookOpened(FruitType ft, int fruitN, Book b);
    public static event OnBookOpened openedBook;

    public enum FruitType{Apple, Banana, Watermelon};
    public FruitType fruitBookType;
    public int fruitN;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchBookType(int i){
        fruitBookType = (FruitType)i;
        fruitN = Random.Range(1,4);
    }

    public void OpenBook()
    {
        openedBook?.Invoke(fruitBookType, fruitN, this);
    }

    public void DestroyBook()
    {
        Destroy(this.gameObject);
    }
}
