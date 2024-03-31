using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseSetActive : MonoBehaviour
{
    public GameObject dates;
    
    public void ReverseActive()
    {
        dates.SetActive(!dates.activeSelf);
    }
}
