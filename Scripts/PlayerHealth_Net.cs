
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;

public class PlayerHealth_Net : NetworkBehaviour
{
    public int MaxHealth;
    AutoCam playerCam;
    private VibrationController Vib;
    private PlayerVariables_Net PV;
    public float spawnTime = 5;
    public bool Alive = false;
    public bool dead = false;
    public Text RespawnText;
    public Image DamageImage;
    private bool damaged;
    private Color flashColour = new Color(1f, 0f, 0f, 0.1f);
    private float flashSpeed = 2f;

    [SyncVar(hook = "OnChangeHealth")]
    public int Health;
    [SyncVar]
    public Vector3 respawnPos;

    public short Id;
    private SkinnedMeshRenderer SMR;

    [ServerCallback]
    void Update()
    {
        if (damaged)
        {
            DamageImage.color = flashColour;
        }
        else
        {
            DamageImage.color = Color.Lerp(DamageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }

        void Start()
    {
        DamageImage = GetComponent<Player>().DamageImage;
        Vib = GetComponent<VibrationController>();
        PV = GetComponent<PlayerVariables_Net>();
        //_boost = GetComponent<BoostBar_Net>();
        
        if (isLocalPlayer)
        {
            Id = GetComponent<Player>().LocalPlayerId;
            
        }
        if (isServer)
        {
            DeathMatchManager.AddPlayer(this);
        }
        Health = MaxHealth;
    }

    public short getID()
    {
        return Id;
    }

    public void DisableColliders()
    {
        foreach (Collider c in GetComponents<Collider>())
        {
            if (c.enabled)
            {
                c.enabled = false;
            }
        }
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            if (c.enabled)
            {
                c.enabled = false;
            }
        }
        transform.Find("Drone_P1").GetComponent<Collider>().enabled = false;
    }

    public void EnableColliders()
    {
        foreach (Collider c in GetComponents<Collider>())
        {
            if (!c.enabled)
            {
                c.enabled = true;
            }
        }
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            if (!c.enabled)
            {
                c.enabled = true;
            }
        }
        transform.Find("Drone_P1").GetComponent<Collider>().enabled = true;
    }

    [ClientRpc]
    public void RpcRespawn()
    {
        Alive = false;
        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        transform.Find("Drone_P1").Find("Player1").GetComponent<SkinnedMeshRenderer>().enabled = false;
        transform.Find("Drone_P1").Find("Player1").GetComponent<SkinnedMeshRenderer>().gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        Health = MaxHealth;
        StartCoroutine(SpawnWait());
    }

    IEnumerator SpawnWait()
    {
        var spawnPoints = NetworkManager.singleton.startPositions;
        PV.setRespawning(true);      
        yield return new WaitForSeconds(spawnTime);
        PV.setRespawning(false);
        PV.resetRespawnTimer();
        transform.Find("Drone_P1").Find("Player1").GetComponent<SkinnedMeshRenderer>().enabled = true;  
        gameObject.layer = LayerMask.NameToLayer("Player");
        transform.Find("Drone_P1").Find("Player1").GetComponent<SkinnedMeshRenderer>().gameObject.layer = LayerMask.NameToLayer("Player");
        transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        EnableColliders();
        Health = MaxHealth;
        Alive = true;
    }

    public void TakeDamage(int damage, GameObject g, string gunName)
    {
        if (!isServer || Health < 0)
        {
            return;
        }

        StartCoroutine(Vib.DamageVib());
        Health -= damage;

        if (!damaged)
        {
            damaged = true;
        }else{
            damaged = false;
        }

        if (Health > 0)
        {
            return;
        }

        Health = 0;
        RpcDeathSound();

        if (gameObject != g)
        {
            RpcIncreaseScore(g);
            StartCoroutine(KillConfirm(g));
            RpcKilledSomeone(g, gunName);    
        }

        StartCoroutine(Vib.DeathVib());

        if (!Score_Net.WinCheck())
        {
            DisableColliders();
            RpcRespawn();
        }
        else
        {
            RpcWin(g);  //If WinCheck is true, someone wins the game
            Invoke("BackToLobby", 20f); //Return to lobby
            GetComponent<Score_Net>().ResetStuff();
        }
    }

    IEnumerator KillConfirm(GameObject g)
    {
        g.GetComponent<Player>().KillConfirm.text = "+1 score";
        yield return new WaitForSeconds(1f);
        g.GetComponent<Player>().KillConfirm.text = "";
    }

    [ClientRpc]
    void RpcIncreaseScore(GameObject g)
    {
        Score_Net.ScoreBoard[g]++;
    }

    void OnChangeHealth(int health)
    {
        Health = health;
        if (GetComponent<Player>().HealthBar)
            GetComponent<Player>().HealthBar.fillAmount = health / (float)MaxHealth;
    }

    //[ClientRpc]
    private void RpcKilledSomeone(GameObject gk, string gun)
    {
        string killed = GetComponent<Player>().PlayerNick;
        string killer = gk.GetComponent<Player>().PlayerNick;
        GetComponent<KillFeed>().RpcSendKillFeed(killer, gun, killed);
    }

    [ClientRpc]
    public void RpcWin(GameObject g)
    {
        Debug.Log("RpcWin");
        GetComponent<Player>().WinLoseText.text = "Game Over " + g.GetComponent<Player>().PlayerNick + " IS THE WINNER!! \n\n Returning to lobby...";
    }

    [ClientRpc]
    void RpcDeathSound()
    {
        GetComponent<AudioController_Net>().PlayOneshotSound(GetComponent<AudioSource>(), 2, 1f);
    }
}