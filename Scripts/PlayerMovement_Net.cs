using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Cameras;

public class PlayerMovement_Net : NetworkBehaviour
{
    [Header("Movement")]
    public float CruisingSpeed = 500F;
    public float BoostSpeed = 1500F;
    public float RotationSpeed = 150F;
    public float MaxZRotation = 60F;
    public float MaxXRotation = 45F;
    private float _movementSpeed = 500F;
    public bool invertedYRot;
    public bool invertedXRot;
    private int switched;
    private float _shotTimer;
    private Rigidbody _playerBody;
    private Transform _model;
    private Quaternion _originalRotation;
    private BoostBar_Net _boost;
    private Animator anim;
    private VibrationController Vib;
    private PlayerHealth_Net PH;
    private float currentDownWeight;
    private float currentUpWeight;
    public TrailRenderer ThrusterTrailLeft;
    public TrailRenderer ThrusterTrailRight;
    private ParticleSystem BoostEngineLeft;
    private ParticleSystem BoostEngineRight;
    private ParticleSystem HeatDistLeft;
    private ParticleSystem HeatDistRight;
    private GameObject Boost1;
    private GameObject Boost2;
    private Player PScript;
    private NetworkIdentity ID;
    public AudioSource engineSound;
    private float enginePitch;
    private const float LowPitch = -1f;
    private const float HighPitch = 1.0f;
    private const float SpeedToRevs = 0.01f;
    Vector3 myVelocity;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            Destroy(this);
            return;
        }
        Vib = GetComponent<VibrationController>();
        PH = GetComponent<PlayerHealth_Net>();
        PScript = GetComponent<Player>();
        _movementSpeed = CruisingSpeed;
        _playerBody = GetComponent<Rigidbody>();
        _boost = GetComponent<BoostBar_Net>();
        var rend = GetComponentInChildren<SkinnedMeshRenderer>();
        rend.enabled = true;
        _model = rend.transform;
        anim = GetComponent<Animator>();
        _originalRotation = transform.rotation;
        _shotTimer = 0.0F;
        invertedXRot = false;
        ThrusterTrailLeft = transform.Find("Drone_P1").Find("Root").Find("L_Engine").Find("L_Engine_01").Find("L_Engine_02").Find("Thruster_Trail_Left").GetComponent<TrailRenderer>();
        ThrusterTrailRight = transform.Find("Drone_P1").Find("Root").Find("R_Engine").Find("R_Engine_01").Find("R_Engine_02").Find("Thruster_Trail_Right").GetComponent<TrailRenderer>();
        BoostEngineLeft = transform.Find("Drone_P1").Find("Root").Find("L_Engine").Find("L_Engine_01").Find("L_Engine_02").Find("Boost").GetComponent<ParticleSystem>();
        BoostEngineRight = transform.Find("Drone_P1").Find("Root").Find("R_Engine").Find("R_Engine_01").Find("R_Engine_02").Find("Boost").GetComponent<ParticleSystem>();
        HeatDistLeft = transform.Find("Drone_P1").Find("Root").Find("L_Engine").Find("L_Engine_01").Find("L_Engine_02").Find("Heat_Distortion_Left").GetComponentInChildren<ParticleSystem>();
        HeatDistRight = transform.Find("Drone_P1").Find("Root").Find("R_Engine").Find("R_Engine_01").Find("R_Engine_02").Find("Heat_Distortion_Right").GetComponentInChildren<ParticleSystem>();
        Boost1 = transform.Find("Drone_P1").Find("Root").Find("R_Engine").Find("R_Engine_01").Find("R_Engine_02").Find("Boost").gameObject;
        Boost2 = transform.Find("Drone_P1").Find("Root").Find("L_Engine").Find("L_Engine_01").Find("L_Engine_02").Find("Boost").gameObject;
    }

    private void FixedUpdate()
    {
        if (PH.Alive)
        {
            _model.gameObject.SetActive(true);
            Move();
            Rotate();
            Buttons();
            EngineAudio();
        }
        else
        {
            StopStuff();
        }
    }

    private void Update()
    {
        _shotTimer += Time.deltaTime;
        _originalRotation = Quaternion.Euler(new Vector3(_playerBody.transform.localEulerAngles.x, _playerBody.transform.localEulerAngles.y, 0));
    }

    private void SetColor()
    {
        ThrusterTrailLeft.startColor = PScript.PlayerColor;
        ThrusterTrailRight.startColor = PScript.PlayerColor;
        ThrusterTrailLeft.endColor = PScript.PlayerColor;
        ThrusterTrailRight.endColor = PScript.PlayerColor;
    }

    private void Move()
    {
        float verti = Input.GetAxis("VerticalMovementP1");
        float hori = Input.GetAxis("HorizontalMovementP1");

        if (Input.GetButton("DecreaseAltitudeP1"))
        {
            _playerBody.AddRelativeForce(Vector3.down * _movementSpeed);
            currentDownWeight = Mathf.Lerp(currentDownWeight, 1.0f, (Time.deltaTime * 20));
            anim.SetLayerWeight(2, currentDownWeight);
        }
        if (!Input.GetButton("DecreaseAltitudeP1"))
        {
            currentDownWeight = Mathf.Lerp(currentDownWeight, 0.0f, (Time.deltaTime * 10));
            anim.SetLayerWeight(2, currentDownWeight);
        }
        if ((Input.GetButton("DecreaseAltitudeP1")) && ((Input.GetAxis("HorizontalMovementP1") > 0) || (Input.GetAxis("HorizontalMovementP1") < 0) || 
            (Input.GetAxis("VerticalMovementP1") > 0) || (Input.GetAxis("VerticalMovementP1") < 0) || (Input.GetButton("IncreaseAltitudeP1"))))
        {
            currentDownWeight = Mathf.Lerp(currentDownWeight, 0.5f, (Time.deltaTime * 10));
            anim.SetLayerWeight(2, currentDownWeight);
        }
        if (Input.GetButton("IncreaseAltitudeP1"))
        {
            _playerBody.AddRelativeForce(Vector3.up * _movementSpeed);
            currentUpWeight = Mathf.Lerp(currentUpWeight, 1.0f, (Time.deltaTime * 20));
            anim.SetLayerWeight(3, currentUpWeight);
        }
        if (!Input.GetButton("IncreaseAltitudeP1"))
        {
            currentUpWeight = Mathf.Lerp(currentUpWeight, 0.0f, (Time.deltaTime * 20));
            anim.SetLayerWeight(3, currentUpWeight);
        }
        if ((Input.GetButton("IncreaseAltitudeP1")) && ((Input.GetAxis("HorizontalMovementP1") > 0) || (Input.GetAxis("HorizontalMovementP1") < 0) ||
            (Input.GetAxis("VerticalMovementP1") > 0) || (Input.GetAxis("VerticalMovementP1") < 0) || (Input.GetButton("IncreaseAltitudeP1"))))
        {
            currentUpWeight = Mathf.Lerp(currentUpWeight, 0.5f, (Time.deltaTime * 10));
            anim.SetLayerWeight(3, currentUpWeight);
        }
        if (Input.GetAxis("HorizontalMovementP1") > 0)
        {
            _playerBody.AddRelativeForce(Vector3.right * _movementSpeed * hori);
            anim.SetFloat("AnimDirection", hori);
        }
        if (Input.GetAxis("HorizontalMovementP1") < 0)
        {
            _playerBody.AddRelativeForce(Vector3.left * -_movementSpeed * hori);
            anim.SetFloat("AnimDirection", hori);
        }
        if (Input.GetAxis("VerticalMovementP1") > 0)
        {
            _playerBody.AddRelativeForce(Vector3.forward * _movementSpeed * verti);
            anim.SetFloat("AnimSpeed", verti);
        }
        if (Input.GetAxis("VerticalMovementP1") < 0)
        {
            _playerBody.AddRelativeForce(Vector3.back * -_movementSpeed * verti);
            anim.SetFloat("AnimSpeed", verti);
        }
        if (Input.GetAxis("VerticalMovementP1") == 0 && (Input.GetAxis("HorizontalMovementP1")) == 0)
        {
            anim.SetFloat("AnimSpeed", 0f);
            anim.SetFloat("AnimDirection", 0f);
        }
    }

    public void Rotate() // boomshackalacka
    {
        float rStickY = Input.GetAxis("HorizontalRotationP1");
        float rStickX = Input.GetAxis("VerticalRotationP1");
		MaxZRotation = 35f;

        if (Input.GetAxis("HorizontalRotationP1") < 0.2)
        {
            if (invertedYRot)
            {
                rStickY *= -1;
            }
            transform.Rotate(new Vector3(0, rStickY, 0) * -rStickY * RotationSpeed * Time.deltaTime);
			if(Input.GetAxis("HorizontalRotationP1") < -0.7){
            if(_model.localEulerAngles.z < MaxZRotation || _model.localEulerAngles.z > 360 - MaxZRotation)
                _model.Rotate(new Vector3(0, 0, -rStickY) * RotationSpeed * Time.deltaTime);
			}
        }
        if (Input.GetAxis("HorizontalRotationP1") > 0.2)
        {
            if (invertedYRot)
            {
                rStickY *= -1;
            }
            transform.Rotate(new Vector3(0, rStickY, 0) * rStickY * RotationSpeed * Time.deltaTime);
			if(Input.GetAxis("HorizontalRotationP1") > 0.7){
            if (_model.localEulerAngles.z < MaxZRotation || _model.localEulerAngles.z > 360 - MaxZRotation)
                _model.Rotate(new Vector3(0, 0, -rStickY) * RotationSpeed * Time.deltaTime); 
			}
        }
        if (Input.GetAxis("VerticalRotationP1") < 0.2)
        {
            if (invertedXRot)
            {
                rStickX *= -1;
            }
            transform.Rotate(new Vector3(rStickX, 0, 0) * RotationSpeed * Time.deltaTime);
        }
        if (Input.GetAxis("VerticalRotationP1") > 0.2)
        {
            if (invertedXRot)
            {
                rStickX *= -1;
            }
            transform.Rotate(new Vector3(rStickX, 0, 0) * RotationSpeed * Time.deltaTime);
        }
        if ((Input.GetAxis("VerticalRotationP1") == 0 && Input.GetAxis("HorizontalRotationP1") == 0) && _shotTimer > 0.3)
        {
            transform.rotation = Quaternion.Lerp(_playerBody.rotation, _originalRotation, Time.deltaTime * 10f); //originalRotation får den att kolla rakt fram dirr, vill bryta ut någon axel.
            _model.rotation = Quaternion.Lerp(_model.rotation, _originalRotation, Time.deltaTime * 10f);
        }
    }

    public void Buttons()
    {
        if (Input.GetButtonDown("LevelOutP1") || Input.GetAxis("LevelOutP1") > 0.5)
        {
            transform.rotation = Quaternion.LerpUnclamped(_playerBody.rotation, _originalRotation, 2f);
            _model.rotation = Quaternion.LerpUnclamped(_playerBody.rotation, _originalRotation, 2f);
        }
        if (Input.GetButtonDown("BoostP1") || Input.GetAxis("BoostP1") > 0.5)
        {
            if (!_boost.TryBoosting())
            {
                _movementSpeed = CruisingSpeed;
                StopBoosting();
                anim.SetLayerWeight(1, 0);
                return;
            }
            //GetComponent<BoostBar>().setBoosting(true);
            _movementSpeed = BoostSpeed;
            StartCoroutine(Vib.BoostVibStart());           
            anim.SetLayerWeight(1, 1);
        }
        if (Input.GetButtonDown("BoostP1") || Input.GetAxis("BoostP1") < 0.5)
        {
            _boost.Boosting = false;
            //GetComponent<BoostBar>().setBoosting(false);
            StopBoosting();
            anim.SetLayerWeight(1, 0);
        }
        if (Input.GetButtonDown("SwitchInvert"))
        {
            switched++;            
            if(switched > 1)
            {
                switched = 0;
            }
            if(switched == 1)
            {
                invertedXRot = true;
                Debug.Log(switched);
            }
            if (switched == 0)
            {
                invertedXRot = false;
                Debug.Log(switched);
            }
        }
    }

    private void StopBoosting()
    {
        _movementSpeed = CruisingSpeed;
    }

    private void EngineAudio()
    {
        myVelocity = _playerBody.velocity;
        float Speed = (transform.InverseTransformDirection(_playerBody.velocity).z + transform.InverseTransformDirection(_playerBody.velocity).y + transform.InverseTransformDirection(_playerBody.velocity).x);
        float engineRevs = Mathf.Abs(Speed) * SpeedToRevs;
        engineSound.pitch = Mathf.Clamp(engineRevs, LowPitch, HighPitch);
    }

    public void StopStuff()
    {
        BoostEngineLeft.Stop();
        BoostEngineRight.Stop();
        Boost1.gameObject.SetActive(false);
        Boost2.gameObject.SetActive(false);
        _model.gameObject.SetActive(false);
        ThrusterTrailLeft.gameObject.SetActive(false);
        ThrusterTrailRight.gameObject.SetActive(false);
        BoostEngineLeft.gameObject.SetActive(false);
        BoostEngineRight.gameObject.SetActive(false);
        HeatDistLeft.gameObject.SetActive(false);
        HeatDistRight.gameObject.SetActive(false);
        ThrusterTrailLeft.gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        ThrusterTrailRight.gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        HeatDistLeft.gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        HeatDistRight.gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        BoostEngineRight.gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        BoostEngineLeft.gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
    }
}
