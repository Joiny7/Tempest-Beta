using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class BoostBar_Net : NetworkBehaviour
{
    public float MaxBoost = 100F;
    public float DepletionSpeed = 10F;
    public float RechargeSpeed = 5F;
    public float DepletedCooldown = 0.3F;

    public ParticleSystem BoostEngineLeft;
    public ParticleSystem BoostEngineRight;
    private ParticleSystem HeatDistLeft;
    private ParticleSystem HeatDistRight;
    private TrailRenderer ThrusterTrailLeft;
    private TrailRenderer ThrusterTrailRight;

    private float _currentBoost;
    private bool _canRecharge = true;

    [HideInInspector]
    public bool Boosting;
    [HideInInspector]
    public Image BoostFilledTP;

    void Start()
    {
        if(!isLocalPlayer)
        {
            Destroy(this);
            return;
        }
        BoostEngineLeft = transform.Find("Drone_P1").Find("Root").Find("L_Engine").Find("L_Engine_01").Find("L_Engine_02").Find("Boost").GetComponentInChildren<ParticleSystem>();
        BoostEngineRight = transform.Find("Drone_P1").Find("Root").Find("R_Engine").Find("R_Engine_01").Find("R_Engine_02").Find("Boost").GetComponentInChildren<ParticleSystem>();
        HeatDistLeft = transform.Find("Drone_P1").Find("Root").Find("L_Engine").Find("L_Engine_01").Find("L_Engine_02").Find("Heat_Distortion_Left").GetComponentInChildren<ParticleSystem>();
        HeatDistRight = transform.Find("Drone_P1").Find("Root").Find("R_Engine").Find("R_Engine_01").Find("R_Engine_02").Find("Heat_Distortion_Right").GetComponentInChildren<ParticleSystem>();
        ThrusterTrailLeft = transform.Find("Drone_P1").Find("Root").Find("L_Engine").Find("L_Engine_01").Find("L_Engine_02").Find("Thruster_Trail_Left").GetComponent<TrailRenderer>();
        ThrusterTrailRight = transform.Find("Drone_P1").Find("Root").Find("R_Engine").Find("R_Engine_01").Find("R_Engine_02").Find("Thruster_Trail_Right").GetComponent<TrailRenderer>();
        //BoostEngineRight.Stop();
        //BoostEngineLeft.Stop();
        _currentBoost = MaxBoost;
        
    }

    public void StartShit()
    {
        HeatDistRight.gameObject.SetActive(true);
        HeatDistLeft.gameObject.SetActive(true);
        ThrusterTrailLeft.gameObject.SetActive(true);
        ThrusterTrailRight.gameObject.SetActive(true);
        HeatDistLeft.Play();
        HeatDistRight.Play();
        ThrusterTrailLeft.enabled = true;
        ThrusterTrailRight.enabled = true;
        ThrusterTrailLeft.gameObject.layer = LayerMask.NameToLayer("Player");
        ThrusterTrailRight.gameObject.layer = LayerMask.NameToLayer("Player");
        HeatDistLeft.gameObject.layer = LayerMask.NameToLayer("Player");
        HeatDistRight.gameObject.layer = LayerMask.NameToLayer("Player");
    }

	void Update ()
    {
        BoostFilledTP.fillAmount = _currentBoost / MaxBoost;

        if (Boosting && GetComponent<PlayerHealth_Net>().Alive)
        {
           Boosting1();
            if (_canRecharge)
                _currentBoost -= DepletionSpeed * Time.deltaTime;

            if (_currentBoost <= 1)
            {
                _currentBoost = 0F;
                Boosting = false;
                Cruising();
            }
        }
        else
        {
            if (GetComponent<PlayerHealth_Net>().Alive)
            {
                Cruising();
                if (_currentBoost <= 0)
                    StartCoroutine(RechargeCooldown());
                if (_canRecharge)
                    _currentBoost += RechargeSpeed * Time.deltaTime;
            }
        }
        _currentBoost = Mathf.Clamp(_currentBoost, 0F, MaxBoost);
    }

    public void Boosting1()
    {
        ThrusterTrailLeft.gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        ThrusterTrailRight.gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        HeatDistLeft.gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        HeatDistRight.gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        BoostEngineRight.gameObject.layer = LayerMask.NameToLayer("Player");
        BoostEngineLeft.gameObject.layer = LayerMask.NameToLayer("Player");
        ThrusterTrailLeft.gameObject.SetActive(false);
        ThrusterTrailRight.gameObject.SetActive(false);
        HeatDistRight.gameObject.SetActive(false);
        HeatDistLeft.gameObject.SetActive(false);
        BoostEngineRight.gameObject.SetActive(true);
        BoostEngineLeft.gameObject.SetActive(true);
        //BoostEngineLeft.Play();
        //BoostEngineRight.Play();
        //HeatDistLeft.Stop();
        //HeatDistRight.Stop();
    }

    public void Cruising()
    {
        ThrusterTrailLeft.gameObject.layer = LayerMask.NameToLayer("Player");
        ThrusterTrailRight.gameObject.layer = LayerMask.NameToLayer("Player");
        HeatDistLeft.gameObject.layer = LayerMask.NameToLayer("Player");
        HeatDistRight.gameObject.layer = LayerMask.NameToLayer("Player");
        BoostEngineLeft.gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        BoostEngineRight.gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        ThrusterTrailLeft.gameObject.SetActive(true);
        ThrusterTrailRight.gameObject.SetActive(true);
        BoostEngineRight.gameObject.SetActive(false);
        BoostEngineLeft.gameObject.SetActive(false);
        //BoostEngineLeft.Stop();
        //BoostEngineRight.Stop();
        HeatDistRight.gameObject.SetActive(true);
        HeatDistLeft.gameObject.SetActive(true);
        HeatDistLeft.Play();
        HeatDistRight.Play();
    }

    public bool TryBoosting()
    {
        Boosting = true;
        return _currentBoost > 1F;
    }

    private IEnumerator RechargeCooldown()
    {
        _canRecharge = false;
        float t = DepletedCooldown;

        while(t > 0F)
        {
            t -= Time.deltaTime;
            yield return 0;
        }
        _canRecharge = true;
        _currentBoost = 0.01F;
        yield return 0;
    }

    public void IncreaseBoost(int x)
    {
        _currentBoost += x;
    }

}
