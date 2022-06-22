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
    
    public delegate void OnBookDestroyed();
    public static event OnBookDestroyed bookDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        AdjustName();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchBookType(int i){
        fruitBookType = (FruitType)i;
        fruitN = Random.Range(1,4);
    }

    private void AdjustName()
    {
        this.gameObject.name 
            = $"{fruitBookType.ToString()} Book";
    }

    public void OpenBook()
    {
        openedBook?.Invoke(fruitBookType, fruitN, this);
    }

    public void DestroyBook()
    {
        bookDestroyed?.Invoke();
        Destroy(this.gameObject);
    }
}
