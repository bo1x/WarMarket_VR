using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Etiqueta : MonoBehaviour
{

    [SerializeField] public float price;
    [SerializeField] public float CDscanTIME;
    [SerializeField] public bool scanned;

    
    public void Scaneado()
    {
        if (!scanned)
        {
            StartCoroutine(CDscan());
        }
    }

    IEnumerator CDscan()
    {
        scanned = true;
        yield return new WaitForSeconds(CDscanTIME);
        scanned = false;
    }

}
