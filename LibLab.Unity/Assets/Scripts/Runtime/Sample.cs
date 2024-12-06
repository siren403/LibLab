using UnityEngine;

public class Sample : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var value = ExampleSourceGenerated.ExampleSourceGenerated.Print();
        Debug.Log(value);
    }

    // Update is called once per frame
    void Update() 
    {
    }
}