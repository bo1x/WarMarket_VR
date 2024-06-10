using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemigo : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public GameObject[] players;
    public GameObject jugadorMasCercano = null;
    public GameObject jugadorMasCercano2 = null;
    public LayerMask playerLayer, sueloLayer;

    public bool esSuTurno;
    //Patrullar
    public Vector3 puntoAlQueVa;
    bool puntoEstablecido;
    public float rangoSiguientePunto;
    //
    public bool haAtacado;

    float distanciaMasCercana, distanciaMasCercana2;

    public float rangoPerseguir, rangoAtaque;
    public bool playerEnRangoPerseguir, playerEnRangoAtacar, playerEncontrado=false, persiguiendo=false;


    private void Awake()
    {
        players= GameObject.FindGameObjectsWithTag("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerEnRangoPerseguir = Physics.CheckSphere(transform.position, rangoPerseguir, playerLayer);
        playerEnRangoAtacar = Physics.CheckSphere(transform.position, rangoAtaque, playerLayer);

        if (!esSuTurno)
        {
            return;
        }
        if (!playerEnRangoPerseguir && !playerEnRangoAtacar && !persiguiendo)
        {
            Patrullar();
            
        }

        else if (playerEnRangoPerseguir && !playerEnRangoAtacar && !playerEncontrado )
        {
            BuscarPlayerMasCercano();
            
        }

        else if (playerEnRangoPerseguir && playerEnRangoAtacar && !haAtacado)
        {
            Atacar();
        }

       

    }

    void Patrullar()
    {
        playerEncontrado = false;
        haAtacado = false;
        //print("patrullando");
        if (!puntoEstablecido)
        {
            BuscarPuntoAndar();
        }
        else
        {
            navMeshAgent.SetDestination(puntoAlQueVa);
        }

        Vector3 distanciaAlPunto = transform.position - puntoAlQueVa;

        if (distanciaAlPunto.magnitude < 1)
        {
            puntoEstablecido = false;
            esSuTurno = false;
        }
    }

    void BuscarPuntoAndar()
    {
        float x = Random.Range(-rangoSiguientePunto, rangoSiguientePunto);
        float z = Random.Range(-rangoSiguientePunto, rangoSiguientePunto);

        puntoAlQueVa = new Vector3(transform.position.x+x, transform.position.y, transform.position.z+z);

        if (Physics.Raycast(puntoAlQueVa, -transform.up, 2f, sueloLayer))
        {
            //No está fuera del mapa
            puntoEstablecido = true;
        }
        else
        {
            BuscarPuntoAndar();
        }
    }

    
    void BuscarPlayerMasCercano()
    {
        playerEncontrado = true;
        
        distanciaMasCercana = Mathf.Infinity;
        float distanciaPlayer=0;
        bool encontrado1=false;
        float distanciaPlayer2 = 0;
        distanciaMasCercana2 = Mathf.Infinity;
        foreach (GameObject player in players)
        {
            if (player != null) 
            { 
                 distanciaPlayer = Vector3.Distance(transform.position, player.transform.position);

                if (distanciaPlayer < distanciaMasCercana)
                {
                    distanciaMasCercana = distanciaPlayer;
                    jugadorMasCercano = player;   
                }
                
            }
        }
        encontrado1 = true;

        foreach (GameObject player2 in players)
        {
            if (player2 != null)
            {
                distanciaPlayer2 = Vector3.Distance(transform.position, player2.transform.position);

                if (distanciaPlayer2 < distanciaMasCercana2 && encontrado1 && player2!=jugadorMasCercano)
                {
                    
                    distanciaMasCercana2 = distanciaPlayer2;
                    jugadorMasCercano2 = player2;
                }

            }
        }
       
            persiguiendo = true;
            Perseguir();
        
       

    }
    void Perseguir()
    {
        
        if (jugadorMasCercano != null && persiguiendo)
        {
            haAtacado = false;
            print("persiguiendo");
            float distanciaAPlayer1 = Vector3.Distance(transform.position,jugadorMasCercano.transform.position);
            float distanciaAPlayer2 = Vector3.Distance(transform.position,jugadorMasCercano2.transform.position);
            float distanciaTotal = distanciaAPlayer1 + distanciaAPlayer2;
            //como la distancia del 1 va a ser menor que la del 2 calculo los % al reves
            float porcentajeHaciaPlayer1 = (distanciaAPlayer2 / distanciaTotal)*100;
            float porcentajeHaciaPlayer2 = (distanciaAPlayer1 / distanciaTotal)*100;
            print("d1 : "+distanciaAPlayer1 + "% : " + porcentajeHaciaPlayer1);
            print("d2 : "+distanciaAPlayer2+ "% : " + porcentajeHaciaPlayer2);

            float x=( (porcentajeHaciaPlayer1 * jugadorMasCercano.transform.position.x) + (porcentajeHaciaPlayer2 * jugadorMasCercano2.transform.position.x))/100;
            float y= 1;
            float z= ((porcentajeHaciaPlayer1 * jugadorMasCercano.transform.position.z) + (porcentajeHaciaPlayer2 * jugadorMasCercano2.transform.position.z))/100;


            Vector3 puntoRelativo = new Vector3(x,y,z);
            print(puntoRelativo);
            ///

            navMeshAgent.SetDestination(puntoRelativo);
            persiguiendo = false;
            /*LimitarMovPerseguir
             * Transform puntoAntesPerseguir;
            if(Vector3.Distance(putnoAntesPerseguir.position, transform.position>rangoMaxPerseguirPorTurno)
            {
            navMeshAgent.SetDestination(transform.position);
            esSuTurno=false;
            }
            )
            */

            /* NINGUNO DE ESTOS FUNCIONA PORQUE NO ESTÁN EN EL UPDATE
            if (navMeshAgent.remainingDistance<=0.1f )
            {
                print("distance");
                /*
                playerEncontrado = false;
                persiguiendo = false;
                Atacar();
                
            }
            if(navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                print("path complete");
                //se ejectuta antes de llegar
                //Atacar();

            }
            if (navMeshAgent.isStopped == true)
            {
                print("stopped");
            }
            if (Vector3.Distance(puntoRelativo, transform.position) < 1)
            {
                print("transform");
            }

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    print("ha llegado");
                }
            }
            */
        }

    }

    void Atacar()
    {
        if (!persiguiendo)
        {
            print("atacado");
            navMeshAgent.SetDestination(transform.position);

            transform.LookAt(jugadorMasCercano.transform);
            if (!haAtacado)
            {
                //anim atacar
                //restar vida o lo q sea
                haAtacado = true;
                Destroy(jugadorMasCercano);
                esSuTurno = false;
                playerEncontrado=false;

            }
        }
        
    }
}
