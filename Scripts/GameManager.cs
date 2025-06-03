using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerMulty : MonoBehaviour
{
    [Header("Defenders")]
    public GameObject[] defenders;
    private GameObject currentDraggingDefender;
    public int[] defendersPrice;
    public GameObject defenderProjectilePrefab;

    private List<GameObject> createdDefenders = new();
    private int createdDefCounts = 0;

    [Header("UI")]
    public Text coinsText;
    public Slider towerHealthSliderA;
    public Slider towerHealthSliderB;
    public Text towerHealthTextA;
    public Text towerHealthTextB;
    public int TotalCoins => localCoins;

    [Header("Tower Settings")]
    public int towerHealthA = 100;
    public int towerHealthB = 100;
    public int towerDamage = 1;

    [Header("UI Windows")]
    public GameObject gameLostWindow;
    public GameObject gameWinWindow;

    [HideInInspector] public int currentDefender;
    [HideInInspector] public bool canInstantiate = true;
    [HideInInspector] public bool isDraging = false;
    [HideInInspector] public bool dragOnViewSpace = true;

    private bool purchasedCurrentItem = true;
    public int localCoins = 5000;

    private string mySide;
    public string MySide => mySide;

    void OnEnable()
    {
        NetworkConnector.OnUDPMessageReceived += OnUDPMessage;
        NetworkConnector.OnMessageReceived += OnTCPMessage;
    }

    void OnDisable()
    {
        NetworkConnector.OnUDPMessageReceived -= OnUDPMessage;
        NetworkConnector.OnMessageReceived -= OnTCPMessage;
    }

    void Start()
    {
        mySide = FindObjectOfType<NetworkConnector>().isHost ? "A" : "B";

        currentDefender = 1;
        purchasedCurrentItem = true;
        localCoins = 5000;
        coinsText.text = localCoins.ToString();

        createdDefenders = new List<GameObject>();
        canInstantiate = true;

        UpdateTowerUI();
    }

    void Update()
    {
        CreateDefender();
        EndDraging();
    }

    public void SetDefenderID(int id)
    {
        currentDefender = id;
    }

    public void SetDraging(bool dragState)
    {
        dragOnViewSpace = false;
        if (localCoins >= defendersPrice[currentDefender - 1])
        {
            isDraging = dragState;
            canInstantiate = true;
            purchasedCurrentItem = false;
        }
    }

    public void EndDraging()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            dragOnViewSpace = true;
            isDraging = false;

            if (!purchasedCurrentItem)
            {
                localCoins -= defendersPrice[currentDefender - 1];
                coinsText.text = localCoins.ToString();
                purchasedCurrentItem = true;

                if (currentDraggingDefender)
                {
                    if (currentDraggingDefender.TryGetComponent(out Weapon weapon))
                        weapon.canShoot = true;

                    if (currentDraggingDefender.TryGetComponent(out Defender_AI ai))
                    {
                        ai.mySide = this.mySide;
                        ai.isPlaced = true; 
                    }

                    Vector3 pos = currentDraggingDefender.transform.position;
                    float rotY = currentDraggingDefender.transform.eulerAngles.y;

                    string msg = $"SPAWN_DEFENDER;{currentDefender - 1};{pos.x:F2};{pos.y:F2};{pos.z:F2};{rotY:F2};{mySide}";
                    NetworkConnector.Instance.SendMessageToPeer(msg);

                    currentDraggingDefender = null;
                }
            }
        }
    }



    void CreateDefender()
    {
        if (isDraging)
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000))
            {
                if (hit.transform.CompareTag("Ground"))
                {
                    if (canInstantiate)
                    {
                        canInstantiate = false;

                        
                        currentDraggingDefender = Instantiate(defenders[currentDefender - 1], hit.point, Quaternion.identity);
                        Renderer rend = currentDraggingDefender.GetComponentInChildren<Renderer>();
                        if (rend != null)
                        {
                            Color color = (mySide == "A") ? Color.blue : Color.red;
                            rend.material.color = color;
                        }
                        if (currentDraggingDefender.TryGetComponent(out Defender_AI ai))
                            ai.mySide = this.mySide;

                        createdDefenders.Add(currentDraggingDefender);
                        createdDefCounts++;

                        
                        if (currentDraggingDefender.TryGetComponent(out Weapon weapon))
                            weapon.canShoot = false;
                    }
                    else if (currentDraggingDefender != null)
                    {
                        
                        currentDraggingDefender.transform.position = hit.point;

                        Vector3 lookPos = FindClosestPoints("Center Point").position - currentDraggingDefender.transform.position;
                        lookPos.y = 0;
                        currentDraggingDefender.transform.rotation = Quaternion.Slerp(currentDraggingDefender.transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 1000);
                    }
                }
            }
        }
    }


    public void SpawnDefenderRemote(int index, Vector3 position, Quaternion rotation, string side)
    {
        if (index < 0 || index >= defenders.Length) return;

        GameObject def = Instantiate(defenders[index], position, rotation);
        Renderer rend = def.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            Color color = (mySide == "B") ? Color.blue : Color.red;
            rend.material.color = color;
        }

        if (def.TryGetComponent(out Weapon weapon))
            weapon.canShoot = false;

        if (def.TryGetComponent(out Defender_AI ai))
        {
            ai.mySide = side;
            ai.isPlaced = true; 
        }
    }


    void OnUDPMessage(string msg)
    {
        if (msg.StartsWith("DEF_SHOOT"))
        {
            string[] parts = msg.Split(';');
            if (parts.Length < 7) return;

            float x = float.Parse(parts[1]);
            float y = float.Parse(parts[2]);
            float z = float.Parse(parts[3]);
            float dx = float.Parse(parts[4]);
            float dy = float.Parse(parts[5]);
            float dz = float.Parse(parts[6]);

            Vector3 pos = new Vector3(x, y, z);
            Vector3 dir = new Vector3(dx, dy, dz);

            SimulateRemoteShoot(pos, dir);
        }

    }

    void OnTCPMessage(string msg)
    {
        if (msg.StartsWith("TOWER_HIT"))
        {
            string[] parts = msg.Split(';');
            if (parts.Length < 3) return;

            string side = parts[1];
            int damage = int.Parse(parts[2]);

            Reduce_Tower_Health(side, damage);
        }

        if (msg.StartsWith("ENEMY_DEAD"))
        {
            string[] parts = msg.Split(';');
            if (parts.Length >= 2)
            {
                string enemyId = parts[1];
                GameObject enemy = GameObject.Find(enemyId);
                if (enemy && enemy.TryGetComponent(out Health hp))
                {
                    hp.RemoteKill();
                }
            }
        }

    }

    void SimulateRemoteShoot(Vector3 pos, Vector3 dir)
    {
        GameObject bullet = Instantiate(defenderProjectilePrefab, pos, Quaternion.LookRotation(dir));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(dir.normalized * 100f); // та же сила, что у владельца
    }

    public void ReduceCoins(int value)
    {
        localCoins -= value;
        coinsText.text = localCoins.ToString();
    }

    public void AddCoins(int value)
    {
        localCoins += value;
        coinsText.text = localCoins.ToString();
    }

    public void Game_Lost()
    {
        Time.timeScale = 0;
        gameLostWindow.SetActive(true);
    }

    public void You_Win()
    {
        Time.timeScale = 0;
        gameWinWindow.SetActive(true);
    }

    public void Reduce_Tower_Health(string side, int value)
    {
        if (side == "A")
        {
            towerHealthA -= value;
            towerHealthSliderA.value = towerHealthA;
            towerHealthTextA.text = towerHealthA.ToString();
            if (towerHealthA <= 0)
            {
                if (mySide == "A")
                    Game_Lost();
                else You_Win();
            }
        }
        else
        {
            towerHealthB -= value;
            towerHealthSliderB.value = towerHealthB;
            towerHealthTextB.text = towerHealthB.ToString();
            if (towerHealthB <= 0) 
            {
                if (mySide == "B")
                    Game_Lost();
                else You_Win();
            }
        }
    }



    void UpdateTowerUI()
    {
        towerHealthSliderA.value = towerHealthA;
        towerHealthSliderB.value = towerHealthB;
        towerHealthTextA.text = towerHealthA.ToString();
        towerHealthTextB.text = towerHealthB.ToString();
    }

    Transform FindClosestPoints(string tag)
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag(tag);
        float closestDist = Mathf.Infinity;
        GameObject closest = null;
        Vector3 currentPos = createdDefenders[createdDefCounts - 1].transform.position;

        foreach (GameObject go in points)
        {
            float dist = (go.transform.position - currentPos).sqrMagnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = go;
            }
        }

        return closest?.transform;
    }
}
