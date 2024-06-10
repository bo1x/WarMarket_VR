using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine.VFX;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public WinLoseManager winLoseManager;

    public Timer gameTimer;

    public Transform npcSpawnPoint;
    public Transform objectSpawnPoint; // Determina donde se instanciaran los productos (deberia estar sobre la cinta)
    public VisualEffect SpawnParticle;

    public bool gameOver;

    [Header("Product List/Shopping List")] 
    public List<GameObject> itemList; // Los objetos de los que se puede elegir al crear la lista de la compra
    public GameObject[] customerItems; // Los objetos actualmente en el carrito de la compra del NPC 
    public GameObject ItemCrate;
    public int maxCartItems; // Determina la cantidad maxima de objetos que puede comprar un NPC
    private int nextItemIndex = 0; 

    [Header("Current Purchase Variables")] 
    public float currentPurchaseValue; // El valor de los objetos escaneados hasta el momento

    [Header("Conveyor Belt")] 
    public float conveyorSpeed = 2f;

    [Header("Customer Variables")]
    [SerializeField] private int _maxHappyCustomers;
    [SerializeField] private int _maxAngryCustomers;
    private int _happyCustomers;
    private int _angryCustomers;
    
    [Header("Score Values")]
    public int _happyCustomerPoints = 100;
    public int _angryCustomerPoints = 45;
    [HideInInspector] public int playerScore = 0;

    [Header("Timer Values")] 
    public float timeBetweenSpawn = 1f;

    [Header("UI Elements")] 
    public GameObject winText;
    public GameObject loseText;

    [Header("Physical Button References")] 
    public GameObject exitButton;
    public GameObject continueRetryButton;

    [Header("VFX")]
    public VisualEffect WeaponEffectTouchBox;
    public Material ScanerColor;
    [ColorUsageAttribute(true, true)]
    public Color GreenColor;
    [ColorUsageAttribute(true, true)]
    public Color RedColor;

    public SimpleAi Inteligencia;
    public GameObject NPCPrefab;
    public Transform NPCSpawn;

    public Transform EntradaTienda;

    public Transform[] RandomPositions;

    public Transform CintaPosition;
    public Transform CajaRegistradora;
    public Transform Salida;

    public Transform PlayerPosition;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameTimer.SetTimer();
        nextItemIndex = 0;
        SpawnNPC();
        //StartCoroutine(WaitForCartGen());
    }
    
    void Update()
    {
        
    }

    #region Callable Methods


    ////AÑADIDO POR ALEX-Si el precio no es el mismo ya sea por que se a perdido un objeto o se a cobrado varias veces alguno
    //Se que es un poco redundante las 3 funciones pero si alguien quiere meter vfx/sonidos queda mejor asi dependiendo

    public void SpawnNPC()
    {
        GameObject NPC = Instantiate(NPCPrefab, NPCSpawn);
        Inteligencia = NPC.GetComponent<SimpleAi>();

        Inteligencia.EntradaTienda = EntradaTienda;
        Inteligencia.RandomPositions = RandomPositions;
        Inteligencia.CintaPosition = CintaPosition;
        Inteligencia.CajaRegistradoraPosition = CajaRegistradora;
        Inteligencia.Salida = Salida;
        Inteligencia.PlayerPosition = PlayerPosition;

        Inteligencia.StartAi();
    }

    public void NPCCheck(float precioEnCaja)
    {
        if (currentPurchaseValue == precioEnCaja)
        {
            Debug.Log("Bien cobrado");
            NPCHappy();
        }
        else
        {
            Debug.Log("Mal cobrado");
            NPCAngry();
        }

        //CheckWinCondition
        //if no win win
        //spawn new npc
    }
    public void NPCHappy()
    {
      _happyCustomers++;
      playerScore += _happyCustomerPoints;
      if (CheckWinLose()) return;
      SpawnNPC();
    }

    public void NPCAngry()
    {
        _angryCustomers++;
        playerScore -= _angryCustomerPoints;
        if (CheckWinLose()) return;
        SpawnNPC();
    }

    public void CreateCustomerCart() // Esta función se debe llamar cuando el NPC llegue a la cinta, o cuando aparezca en la tienda
    {
        if (customerItems != null)
        {
            customerItems = null;
        }
        customerItems = new GameObject[Random.Range(1, maxCartItems + 1)];

        for (int i = 0; i < customerItems.Length; i++)
        {
            customerItems[i] = itemList[Random.Range(0, itemList.Count)];
        }




        //AÑADIDO POR ALEX-Calculo el precio de todos los objetos 
        foreach (var item in customerItems)
        {
            Etiqueta etiq = item.GetComponentInChildren<Etiqueta>();
            currentPurchaseValue += etiq.price;
        }
        //SpawnAllCartItems();
    }

    public bool SpawnCartItemSingle() // La funcion que se llama desde el NPC, probablemente desde un evento de animacion
    {
        SpawnParticle.Play();
        Instantiate(customerItems[nextItemIndex], objectSpawnPoint);
        nextItemIndex++;
        if (nextItemIndex >= (customerItems.Length - 1))
        {
            nextItemIndex = 0; // Resetear el valor una vez el NPC haya depositado todos los objetos en la cinta
            return true;
        }
        return false;
    }

    #endregion

    #region IEnumerators
    private IEnumerator WaitForCartGen()
    {
        // THIS IS FOR TESTING PURPOSES, CREATECUSTOMERCART() SHOULD BE CALLED FROM THE NPC
        yield return new WaitForSeconds(4f);
        Debug.Log("Done Waiting");
        CreateCustomerCart();
    }

    private IEnumerator WaitForObjectSpawn()
    {
        foreach (var item in customerItems)
        {
            yield return new WaitForSeconds(timeBetweenSpawn);
            Instantiate(item, objectSpawnPoint);
        }
        Debug.Log("Done Spawning");
    }
    
    #endregion

    private void SpawnAllCartItems() // Esto es para spawnear todos los elementos del carrito
    {
        StartCoroutine(WaitForObjectSpawn());
    }

    #region Win/Lose Methods

    private bool CheckWinLose()
    {
        if (_happyCustomers >= _maxHappyCustomers)
        {
            EnablePhysicalButtons();
            ShiftEnded();
            return true;
        }
        if (_angryCustomers >= _maxAngryCustomers)
        {
            EnablePhysicalButtons();
            GotFired();
            return true;
        }

        return false;
    }

    private void EnablePhysicalButtons()
    {
        exitButton.SetActive(true);
        continueRetryButton.SetActive(true);
    }
    
    public void ShiftEnded()
    {
        gameTimer.PauseTimer();
        winLoseManager.PlayerWins(playerScore, _happyCustomers, _angryCustomers);
    }

    public void GotFired()
    {
        gameTimer.PauseTimer();
        winLoseManager.PlayerLoses(playerScore, _happyCustomers, _angryCustomers);
    }

    #endregion

    #region UI Button Methods

    public void ButtonQuit()
    {
        Application.Quit();
    }

    public void ButtonGameResetOrContinue()
    {
        if (winLoseManager.winCanvas.isActiveAndEnabled)
        {
            ContinuePlaying();
        }
        else if(winLoseManager.loseCanvas.isActiveAndEnabled)
        {
            ResetGame();
        }
    }

    private void ResetGame()
    {
        playerScore = 0;
        _happyCustomers = 0;
        _angryCustomers = 0;
        winLoseManager.PlayerContinueOrRetry();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ContinuePlaying()
    {
        _happyCustomers = 0;
        _angryCustomers = 0;
        winLoseManager.PlayerContinueOrRetry();
        gameTimer.SetTimer();
        nextItemIndex = 0;
        SpawnNPC();
    }

    #endregion
}
