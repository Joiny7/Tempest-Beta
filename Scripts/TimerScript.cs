using UnityEngine;
using UnityEngine.Networking;

public class TimerScript : NetworkBehaviour { 

    public float matchTimer;
    private bool spamProtection;
    public bool Ending = false;
    public bool Running = false;
    public GameObject LobbyManager;

    void Start()
    {
        Ending = false;
    }

    private void Update()
    {
        if (matchTimer > 0)
        {
            matchTimer -= Time.deltaTime;
            Running = true;
        }
        if (matchTimer <= 0 && !spamProtection)
        {
            matchTimer = 0;
            Ending = true;
            Debug.Log("Time ran out, highest score is the winner! Returning to lobby...");
            spamProtection = true;
            Invoke("BackToLobby", 10f);
        }
    }

    private void BackToLobby()
    {
        FindObjectOfType<NetworkLobbyManager>().ServerReturnToLobby();
    }
}
