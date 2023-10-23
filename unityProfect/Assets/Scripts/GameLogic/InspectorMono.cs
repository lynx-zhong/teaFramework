using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InspectorMono : MonoBehaviour
{
    public int testInt = 10;
    public Button button;

    private void Start()
    {
        button.onClick.AddListener(Tes);
    }

    void OnDestory()
    {
        
    }

    void Tes(){}
}
