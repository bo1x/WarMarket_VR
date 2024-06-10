using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ModoCobro : MonoBehaviour
{

    [SerializeField] VisualEffect visualCoins;
    public void cambiarModo()
    {
        if (GameManager.Instance.Inteligencia == null) return;
        if (GameManager.Instance.Inteligencia.AiActualState != SimpleAi.AiState.WaitiingVendedor) return;

        if (Scanner.Instance.priceCont != 0)
        {
            visualCoins.Play();
            Scanner.Instance.CambiarAModoCobro();
        }


    }
}
