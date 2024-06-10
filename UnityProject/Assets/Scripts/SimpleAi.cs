using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleAi : MonoBehaviour
{
    public enum AiState {ComingToShoooop, TravelingRandomPositions, TravelingCinta, GivingItemsIntoTheCintaaa, ComingToVendedor, WaitiingVendedor, WaitingMoney, Leaving}
    public AiState AiActualState = AiState.ComingToShoooop;
    private bool IAmWaiting;
    private Coroutine WaitingRoutine;
    private int StopLenght;
    private Vector3 ActualPosition;

    private bool FinishedSpawning;
    private Coroutine SpawningRoutine;

    [Header("PointsToTravel")]
    public Transform EntradaTienda;
    public Transform[] RandomPositions;
    public Transform CintaPosition;
    public Transform CajaRegistradoraPosition;
    public Transform Salida;

    [Header("Speeds")]
    public float RotationSpeed;

    public Transform PlayerPosition;

    [Header("Dependencies")]
    public Animator _animator;
    public NavMeshAgent agent;
    public GameObject MeshCharacter;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void StartAi()
    {
        agent.destination = EntradaTienda.position;

        _animator.SetBool("IsRunning", true);
        _animator.SetTrigger("ChangeAnimation");

        if (GameManager.Instance == null) Debug.LogError("Falta el GameManager");

        GameManager.Instance.CreateCustomerCart();
    }

    // Update is called once per frame
    void Update()
    {
        switch (AiActualState)
        {
            case AiState.ComingToShoooop:
                ComingShop();
                break;
            case AiState.TravelingRandomPositions:
                TravelingRandomPositions();
                break;
            case AiState.TravelingCinta:
                TravelingCinta();
                break;
            case AiState.GivingItemsIntoTheCintaaa:
                GivingItemsToCinta();
                break;
            case AiState.ComingToVendedor:
                ComingToVendedor();
                break;
            case AiState.WaitiingVendedor:
                WaitiingVendedor();
                break;
            case AiState.WaitingMoney:
                WaitingMoney();
                break;
            case AiState.Leaving:
                Leaving();
                break;
        }


    }

    public void ComingShop()
    {
        agent.speed = 3;
        MeshCharacter.transform.rotation = Quaternion.RotateTowards(MeshCharacter.transform.rotation, Quaternion.Euler(0.0f, 0, 0.0f), RotationSpeed * Time.deltaTime);

        if (agent.remainingDistance <= 0.05f)
        {
            StopLenght = Random.Range(3, 6);

            _animator.SetBool("IsWalking", true);
            _animator.SetBool("IsRunning", false);
            _animator.SetTrigger("ChangeAnimation");

            AiActualState = AiState.TravelingRandomPositions;
        }
    }

    public void ChangePositionToRandomPosition()
    {
        agent.speed = Random.Range(0.7f, 2);

        Vector3 DifferentPosition = agent.destination = RandomPositions[(int)Random.Range(0, RandomPositions.Length)].position;

        while (Vector3.Equals(ActualPosition, DifferentPosition))
        {
            DifferentPosition = agent.destination = RandomPositions[(int)Random.Range(0, RandomPositions.Length)].position;
        }

        ActualPosition = DifferentPosition;

        IAmWaiting = false;
        WaitingRoutine = null;
        StopLenght--;
        Mathf.Clamp(StopLenght, 0, StopLenght);

        if(StopLenght == 0)
        {
            AiActualState = AiState.TravelingCinta;
            ChangePositionCinta();
        }
    }

    public void TravelingRandomPositions()
    {
        if (agent.remainingDistance <= 0.05f && WaitingRoutine == null && !IAmWaiting)
        {
            WaitingRoutine = StartCoroutine(WaitingPosition());
        }
    }

    IEnumerator WaitingPosition()
    {
        IAmWaiting = true;
        _animator.SetBool("IsWalking", false);

        yield return new WaitForSeconds(0.5f);
        _animator.SetBool("IsWalking", true);
        _animator.SetTrigger("ChangeAnimation");
        ChangePositionToRandomPosition();
    }

    public void ChangePositionCinta()
    {
        agent.destination = CintaPosition.position;
    }

    public void TravelingCinta()
    {
        if(agent.remainingDistance <= 1)
        {
            Vector3 difference = MeshCharacter.transform.position - CintaPosition.forward * 2;
            float rotationZ = Mathf.Atan2(difference.x, difference.z) * Mathf.Rad2Deg;
            MeshCharacter.transform.rotation = Quaternion.RotateTowards(MeshCharacter.transform.rotation, Quaternion.Euler(0.0f, rotationZ + 90, 0.0f), 200 * Time.deltaTime);
        }

        if (agent.remainingDistance <= 0.05f)
        {
            _animator.SetBool("IsWalking", false);
            agent.isStopped = true;
            AiActualState = AiState.GivingItemsIntoTheCintaaa;
        }
    }

    public void GivingItemsToCinta()
    {

        // Cambiar a SpawnItems del GameManager

        if (SpawningRoutine != null) return;

        // si la cantidad es 0 entonces
        if (FinishedSpawning)
        {
            AiActualState = AiState.ComingToVendedor;
            agent.destination = CajaRegistradoraPosition.position;
            agent.isStopped = false;
            _animator.SetBool("IsWalking", true);
            _animator.SetTrigger("ChangeAnimation");
            MeshCharacter.transform.localRotation = Quaternion.Euler(Vector3.zero);

            
        }
        else
        {
            SpawningRoutine = StartCoroutine(SpawningItem());
        }
    }

    public IEnumerator SpawningItem()
    {
        yield return new WaitForSeconds(1.2f);

        bool TemporalBool = GameManager.Instance.SpawnCartItemSingle();
        _animator.SetTrigger("Dropping");
        yield return new WaitForSeconds(1.2f);

        if (TemporalBool)
        {
            SpawningRoutine = null;
            FinishedSpawning = true;
        }

        SpawningRoutine = null;
    }

    public void ComingToVendedor()
    {
        Debug.Log("Traveling Random Positions BOYYY");
        if (agent.remainingDistance <= 0.3f)
        {
            WatchingPlayer();
        }

        if(agent.remainingDistance <= 0.01f)
        {
            agent.isStopped = true;
            _animator.SetBool("IsWalking", false);
            AiActualState = AiState.WaitiingVendedor;
            Scanner.Instance.NpcYaaDejadoBainas();

            _animator.SetTrigger("Dropping");
            // Poner caja.
            GameManager.Instance.ItemCrate.SetActive(true);

        }
    }

    public void WaitiingVendedor()
    {
        WatchingPlayer();
        if (Scanner.Instance.JugadorQuiereCobrar)
        {
            AiActualState = AiState.WaitingMoney;
        }
    }

    public void WaitingMoney()
    {
        WatchingPlayer();
        if (Scanner.Instance.CobrarNpc())
        {
            agent.speed = 3;
            agent.isStopped = false;
            agent.destination = Salida.position;
            MeshCharacter.transform.localRotation = Quaternion.Euler(Vector3.zero);
            _animator.SetBool("IsRunning", true);
            _animator.SetTrigger("ChangeAnimation");
            AiActualState = AiState.Leaving;

            _animator.SetTrigger("Dropping");
            GameManager.Instance.ItemCrate.SetActive(false);
        }
    }

    public void Leaving()
    {
        if (agent.remainingDistance <= 0.01f)
        {
            
            Destroy(gameObject);

            if(GameManager.Instance.gameTimer.Value > 0)
            {
                GameManager.Instance.SpawnNPC();
            }
        }
    }

    public void WatchingPlayer()
    {
        Vector3 difference = MeshCharacter.transform.position - PlayerPosition.position;
        float rotationZ = Mathf.Atan2(difference.x, difference.z) * Mathf.Rad2Deg;
        MeshCharacter.transform.rotation = Quaternion.RotateTowards(MeshCharacter.transform.rotation, Quaternion.Euler(0.0f, rotationZ + 180, 0.0f), 100 * Time.deltaTime);
    }
}
