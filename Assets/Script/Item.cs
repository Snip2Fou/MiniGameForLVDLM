using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : MonoBehaviour
{
    public int point;
    public string name;
    public Sprite icon;

    private void Start()
    {
        name = gameObject.name;
    }
}

