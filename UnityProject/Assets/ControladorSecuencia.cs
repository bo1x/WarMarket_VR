using UnityEngine;
using System.Collections;

public class ControladorSecuencia : MonoBehaviour
{
    public GameObject[] modelos; // Asigna los modelos en el Inspector
    private int indiceModeloActual = 0;

    // Agregar referencia est�tica para la instancia
    private static ControladorSecuencia _instance;
    public static ControladorSecuencia Instance => _instance;

    void Awake()
    {
        // Asignar la instancia en el Awake
        _instance = this;
    }

    void OnEnable()
    {
        // Activa el primer modelo y comienza su animaci�n al activarse el objeto
        modelos[indiceModeloActual].SetActive(true);
        StartCoroutine(ReproducirSiguienteAnimacion());
    }

    // Resto del c�digo sin cambios

    // Nueva corutina est�tica para ser referenciada desde otro script
    public IEnumerator ReproducirSiguienteAnimacion()
    {
        // Obt�n el componente Animator del modelo actual
        Animator animatorActual = modelos[indiceModeloActual].GetComponent<Animator>();

        // Espera hasta que la animaci�n actual haya terminado
        yield return new WaitForSeconds(animatorActual.GetCurrentAnimatorStateInfo(0).length);

        // Desactiva el modelo actual
        modelos[indiceModeloActual].SetActive(false);

        // Incrementa el �ndice del modelo actual
        indiceModeloActual++;

        // Si hemos alcanzado el final de los modelos, reinicia desde el principio
        if (indiceModeloActual >= modelos.Length)
        {
            indiceModeloActual = 0;

            // Desactiva el objeto que contiene el script al finalizar la secuencia
            gameObject.SetActive(false);
        }
        else
        {
            // Activa el nuevo modelo y comienza su animaci�n
            modelos[indiceModeloActual].SetActive(true);

            // Reproduce la siguiente animaci�n en el nuevo modelo
            StartCoroutine(ReproducirSiguienteAnimacion());
        }
    }
}
