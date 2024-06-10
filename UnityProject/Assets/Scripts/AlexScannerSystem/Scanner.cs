using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scanner : MonoBehaviour
{

    public float priceCont = 0;

    [SerializeField] LayerMask layerEtiquetas;
    private RaycastHit hit;
    private Etiqueta Etiq;

    [SerializeField] private TMP_Text cajaregistradora;
    private GameObject lastGO;

    public static Scanner Instance;

    public bool NPCesperando;
    public bool JugadorQuiereCobrar;
    public bool NPCyaCobrado;

    public Coroutine ColorRoutine;

    void Awake()
    {
        Instance = this;
        resetNewCliente();
        GameManager.Instance.ScanerColor.SetColor("_Glow", GameManager.Instance.RedColor);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.up*10, Color.yellow);
        ScanearBaina();

    }

    public void ScanearBaina()
    {
        if (JugadorQuiereCobrar)
        {
            return;
        }
        if (Physics.Raycast(transform.position, transform.up, out hit, 2f,layerEtiquetas))
        {
            
            //Si sigo escaneando el mismmo objeto todo el rato lo ignoro
            if (lastGO == hit.collider.gameObject)
            {
                return;
            }
            else
            {
                //Si el objeto no es igual cojo el componente etiqueta y pillo el precio y lo enseño en la caja registradora
                if (hit.collider.gameObject.TryGetComponent<Etiqueta>(out Etiq) )
                {
                    //Scanned significa que el objeto a sido scaneado hace poco tiempo, para no spamear
                    if (Etiq.scanned == false)
                    {
                        if (Vector3.Angle(transform.up, Etiq.transform.up * -1) < 51f)
                        {
                            Debug.Log("SCANEADO");
                            GameManager.Instance.ScanerColor.SetColor("_Glow", GameManager.Instance.GreenColor);
                            if(ColorRoutine != null)
                            {
                                StopCoroutine(ColorRoutine);
                                ColorRoutine = null;
                            }

                            ColorRoutine = StartCoroutine(ResetColor());

                            Etiq.Scaneado();
                            lastGO = hit.collider.gameObject;
                            priceCont += Etiq.price;

                            //Test si redondeo va bien o carlos se a fumado 4 porros y no hizo merge de mi commit testear
                            /*
                            priceCont = priceCont * 100;
                            priceCont = Mathf.Floor(priceCont);
                            priceCont = priceCont / 100;
                            */ 

                            cajaregistradora.text = string.Format("{0:#0.0}", priceCont);
                            Debug.Log("HIT OBJETO QUE BUSCO");
                            return;
                        }
                        
                    }
                    
                }
            }
           
            
            Debug.Log("Did Hit");
        }
        else
        {
            //Si no detecto ningun objeto borro la referencia al obj anterior
            lastGO = null;
            Debug.DrawRay(transform.position, transform.up * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }

    IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(0.4f);
        GameManager.Instance.ScanerColor.SetColor("_Glow", GameManager.Instance.RedColor);
    }

    public void NpcYaaDejadoBainas()
    {
        NPCesperando = true;
    }

    public bool CobrarNpc()
    {
        if (NPCyaCobrado)
        {
            GameManager.Instance.NPCCheck(priceCont);
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void yacrobrado()
    {
        NPCyaCobrado = true;
    }

    //Jugador le da a un boton para cobrar
    public void CambiarAModoCobro()
    {
        cajaregistradora.text = "pOr FaVoR pAsE lA tArGjeta";
        JugadorQuiereCobrar = true;
        NPCyaCobrado = true;
    }

    public void resetNewCliente()
    {
        NPCesperando = false;
        JugadorQuiereCobrar = false;
        NPCyaCobrado = false;
        priceCont = 0;
        cajaregistradora.text = string.Format("{0:#0.00}", priceCont);
    }
}
