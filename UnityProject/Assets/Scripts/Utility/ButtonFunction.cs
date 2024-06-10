using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class ButtonFunction : MonoBehaviour
{
    public bool NoRepeat;

    public UnityEvent OnClick;

    private bool Enable = true;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<XRSimpleInteractable>().selectEntered.AddListener(x => OnClickButton());
    }

    public void OnClickButton()
    {
        if (!Enable)
            return;

        Debug.Log("Se Clickeo el boton");
        if (OnClick == null)
            return;

        OnClick.Invoke();

        if (NoRepeat)
        {
            Enable = false;
        }

    }
}
