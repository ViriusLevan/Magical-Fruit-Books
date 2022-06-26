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
    public bool used=false;
    
    public delegate void OnBookDestroyed();
    public static event OnBookDestroyed bookDestroyed;

    [SerializeField] private GameObject containerSphere;
    [SerializeField] private Material[] glassMaterials;

    // Start is called before the first frame update
    void Start()
    {
        AdjustName();
        AdjustColor();
        DetermineFruitN();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdjustColor()
    {
        containerSphere.GetComponent<Renderer>().material=glassMaterials[(int)fruitBookType];
    }

    public void SwitchBookType(int i)
    {
        fruitBookType = (FruitType)i;
        AdjustName();
        AdjustColor();
        DetermineFruitN();
    }

    private void DetermineFruitN()
    {
        fruitN = Random.Range(1+(2-((int)fruitBookType)),6-((int)fruitBookType)*2);
    }

    private void AdjustName()
    {
        this.gameObject.name 
            = $"{fruitBookType.ToString()} Book";
    }

    public void OpenBook()
    {
        openedBook?.Invoke(fruitBookType, fruitN, this);
        used=true;
    }

    public void DestroyBook()
    {
        bookDestroyed?.Invoke();
        Destroy(this?.gameObject);
    }
}
