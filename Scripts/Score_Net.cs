using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Score_Net : NetworkBehaviour
{
    public static int highest = 0;
    private static int winScore = 20;
    public static Dictionary<GameObject, int> ScoreBoard = new Dictionary<GameObject, int>();
    public GameObject Winner;
    private TimerScript TS;
    public GameObject Leader;

    public void Start()
    {
        if (highest > 0)
        {
            highest = 0;
        }
        ScoreBoard.Clear();
    }

    public void Update()
    {
        playerCheck();
        if (TS == null)
        {
            var tObject = GameObject.FindWithTag("Controller");
            TS = tObject.GetComponent<TimerScript>();
        }
        if (TS.Ending)
        {
            GetWinner();
            ResetStuff();
        }
    }

    public void DebugScore()
    {
        foreach(var g in ScoreBoard)
        {
            Debug.Log(g);
        }
    }

    public void ResetStuff()
    {
        foreach (var g in ScoreBoard.Keys)
        {   
            g.GetComponent<PlayerVariables_Net>().HighestScore = 0;
            g.GetComponent<PlayerVariables_Net>().playerScore = 0;
            ScoreBoard.Clear(); //Empty dictionary
        }
        ScoreBoard.Clear();
    }

    private void playerCheck()
    {
        foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (!ScoreBoard.ContainsKey(p))
                ScoreBoard.Add(p, 0);
        }
    }

    // New Try
    public static bool WinCheck()
    {
        foreach (int i in ScoreBoard.Values)
        {
            if(ScoreBoard.ContainsValue(winScore))
            {
                return false; //NOTE: sätt den till true för att aktivera score win check igen
            }
        }
        return false;
    }

    public string GetWinner()
    {
        foreach (GameObject g in ScoreBoard.Keys)
        {
            var i = ScoreBoard[g];
            if (i == highest)
            {
                Winner = g;
            }
        }
        return Winner.GetComponent<Player>().PlayerNick;
    }

    public static int GetHighestScore()
    {        
        foreach(var g in ScoreBoard.Keys)
        {
            var i = ScoreBoard[g];
            if (i > highest)
                highest = i;
        }
        return highest;
    }

    public GameObject GetLeader()
    {
        foreach (GameObject g in ScoreBoard.Keys)
        {
            var i = ScoreBoard[g];
            if (i == GetHighestScore())
            {
                Leader = g;
            }
        } return Leader;
    }

	public static int GetPlayerScore(GameObject p)
	{
		int playerScore = 0;

		foreach (var g in ScoreBoard.Keys) {
            if (p == g)
            {
                var i = ScoreBoard[g];
                playerScore = i;
            }
		}
        return playerScore;
	}
}
