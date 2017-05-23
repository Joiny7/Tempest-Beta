using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerVariables_Net : NetworkBehaviour
{
    public float MaxTimeOutOfBounds = 10.0F;
    public float StartCountdownTime = 5.0F;
    public int playerScore = 0;
    public int HighestScore = 0;
    private bool InDaZone;
    private float _respawnTimer;
    private Rigidbody _playerBody;
    public short Id;
    private float _startTimer;
    public bool JustStarted;
    private VibrationController Vib;
	public float timeLeft;
	public int minuteConvert;
    public bool respawning;
    public Text DangerZoneWarning;
    public Text RespawnText;
    public List<Sprite> ImageList = new List<Sprite>();
    private PlayerHealth_Net _playerHealth;
    public AudioClip countdown;
    private TimerScript TS;
    private Score_Net SN;

    void Start()
    {
        if (!isLocalPlayer)
        { 
            Destroy(this);
            return;
        }    
        Vib = GetComponent<VibrationController>();
        JustStarted = true;
        _startTimer = 0;
        _respawnTimer = 0;
        _playerHealth = GetComponent<PlayerHealth_Net>();
        _playerBody = GetComponent<Rigidbody>();
        SN = GetComponent<Score_Net>();
        StartCoroutine(Countdown());       
    }

    private IEnumerator Countdown()
    {
        GetComponent<AudioSource>().PlayOneShot(countdown, 1); //Fuckar ur i utbakad version????????
        _playerBody.constraints = RigidbodyConstraints.FreezeAll;
        _playerBody.isKinematic = true;
        _playerBody.freezeRotation = true;
        yield return new WaitForSecondsRealtime(StartCountdownTime);
        _playerBody.constraints = RigidbodyConstraints.None;
        _playerBody.freezeRotation = false;
        _playerBody.isKinematic = false;
        JustStarted = false;
        _playerHealth.Alive = true;
        GetComponent<PlayerMovement_Net>().engineSound.Play();
        HighestScore = 0;
        playerScore = 0;
    }

    public int getPlayerScore()
    {
        return playerScore;
    }

    public void setPlayerScore()
    {
        AddScore();
    }

    public void AddScore()
    {
        if (isLocalPlayer)
        {
            playerScore += 1;
        }
    }

    public void Timelimit()
    {
        var tObject = GameObject.FindWithTag("Controller");
        TS = tObject.GetComponent<TimerScript>();
        timeLeft = TS.matchTimer;
        float minutes = Mathf.Floor(timeLeft / 60);
        float seconds = Mathf.Round(timeLeft % 60);
        GetComponent<Player>().TimeLeft.text = (minutes.ToString() + ":" + seconds.ToString());
        if (seconds < 10)
        {
            GetComponent<Player>().TimeLeft.text = (minutes.ToString() + ":0" + seconds.ToString());
        }
        if (timeLeft <= 6.0f && !GetComponent<AudioSource>().isPlaying && timeLeft != 0)
        {
            GetComponent<AudioSource>().PlayOneShot(countdown, 1);
        }
    }

    void Update ()
	{
        if (!JustStarted) { 
        Timelimit();
            DangerZoneWarning.text = "";
        }
        if (JustStarted) {
			_startTimer += Time.deltaTime;
			var countdownS = (StartCountdownTime - _startTimer);
			DangerZoneWarning.text = "Get Ready!\n" + "Combat starts in: " + countdownS.ToString ("#");
		}
        if (respawning)
        {
            _respawnTimer += Time.deltaTime;
            var countdown = (GetComponent<PlayerHealth_Net>().spawnTime - _respawnTimer);
            RespawnText.text = "You died! Respawning in " + countdown.ToString("#0") + "..";
        }
        if (!respawning)
        {
            RespawnText.text = "";
        }
        if (TS.Ending)
        {
            HighestScore = 0;
            playerScore = 0;
            RespawnText.text = "Time ran out, " + SN.GetWinner() + " is the winner! Returning to lobby...";
        }
        GetComponent<Player>().YourScore.text = Score_Net.GetPlayerScore(gameObject).ToString();
        GetComponent<Player>().HighestScore.text = Score_Net.GetHighestScore().ToString();
        playerScore = Score_Net.GetPlayerScore(gameObject);
        HighestScore = Score_Net.GetHighestScore();
        ColourForLeader();
    }

    void ColourForLeader()
    {
        GetComponent<Player>().Leader.GetComponent<Outline>().effectColor = GetComponent<Score_Net>().GetLeader().GetComponent<Player>().PlayerColor;
        GetComponent<Player>().HighestScore.GetComponent<Outline>().effectColor = GetComponent<Score_Net>().GetLeader().GetComponent<Player>().PlayerColor;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Mine"))
        {
            _playerBody.freezeRotation = true;
        }
        if (col.gameObject.CompareTag("Ground"))
        {
            _playerBody.freezeRotation = true;
            Vib.CollisionVibStart();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Vib.CollisionVibStop();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _playerBody.freezeRotation = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {      
        if (other.gameObject.CompareTag("Ground"))
        {
            _playerBody.freezeRotation = true;
        }
        if (other.gameObject.CompareTag("BoostPickup"))
        {
            GetComponent<BoostBar_Net>().IncreaseBoost(5);
            other.GetComponent<AudioSource>().Play();
            Destroy(other);
        }
    }

    public void setRespawning(bool x)
    {
        respawning = x;
    }

    public void resetRespawnTimer()
    {
        _respawnTimer = 0;
    }
}
