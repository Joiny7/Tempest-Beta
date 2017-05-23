using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerShooting_Net : NetworkBehaviour
{
    public bool scattered = false;
    public float scatterTimer;
    public List<LineRenderer> Lines = new List<LineRenderer>();
    public float lineTime = 0.0007f;
    public int ScatterFragments = 10;
    public float ScatterRange = 300f;
    public float ScatterVariance = 10.0f;
    public float scatterAmmo;
    public float missileAmount;
    public float mineAmount;
    public float ShotCooldown;
    public float MineCooldown;
    public float HeatseekingCooldown;
    private bool heatseekOnCooldown = false;
    private bool minesOnCooldown = false;
    private bool shotgunOnCooldown = false;
    public List<GameObject> ShotPrefabs;
    public List<Transform> _shotOrigins;
    public int CurrentGun = 0;
    private int _activeGun = 0;
    private float _shotTimer;
    private float _mineTimer;
    private float _heatSeekingTimer;
    public short Id;                    //Används i Player.cs
    private VibrationController Vib;
    private PlayerVariables_Net PV;
    private Image UI;
    [SyncVar]
    private GameObject thisPlayer;
    private PlayerHealth_Net PH;
    public float gunOverheating;
    public float RocketAmmo;
    public float Overheated = 10f;
    public float MGshooting;
    public float maxRounds = 8f;
    public bool BurnedOut;
    public float rocketRefill;
    public bool hitDetection;
    public Transform Target;
    private Vector3 lastPos;
    private GameObject ScatterLines;
    public Text PickUpText;
    public Image PickUpImage;
    public GameObject muzzleFlashParticle;

    void Start()
    {
        if (!isLocalPlayer)
            return;

        _shotTimer = ShotCooldown;
        _mineTimer = MineCooldown;
        _heatSeekingTimer = HeatseekingCooldown;
        Vib = GetComponent<VibrationController>();
        PV = GetComponent<PlayerVariables_Net>();
        PH = GetComponent<PlayerHealth_Net>();
        thisPlayer = gameObject;
        GetComponent<Player>().MGActive.enabled = false;
        GetComponent<Player>().HeatSeekActive.enabled = false;
        GetComponent<Player>().RocketActive.enabled = true;
        GetComponent<Player>().CrosshairMachinegun.SetActive(false);
        GetComponent<Player>().CrosshairRockets.SetActive(true);
        GetComponent<Player>().MGOverheat.fillAmount = 1;
        GetComponent<Player>().MGInfinity.fillAmount = 0;
        GetComponent<Player>().CrossLock.enabled = false;
        ScatterLines = gameObject.transform.Find("ScatterLines").gameObject;
        RocketAmmo = 8;
        scatterAmmo = 0f;
        gunOverheating = 0;
        MGshooting = 2f;
        BurnedOut = false;
        GetComponent<Player>().HeatAmount.text = missileAmount.ToString();
        GetComponent<Player>().MineAmount.text = mineAmount.ToString();
        GetComponent<Player>().SpreadAmount.text = scatterAmmo.ToString();
        GetComponent<Player>().MGInactive.enabled = false;
        GetComponent<Player>().RocketInactive.enabled = false;
        GetComponent<Player>().MGInfinity.enabled = false;
        FindLineRenderers();
    }

    private void FindLineRenderers()
    {
        foreach (LineRenderer LR in gameObject.GetComponentsInChildren<LineRenderer>())
        {
            Lines.Add(LR);
        }
    }

    private void StopLines()
    {
        for (int i = 0; i < 10; i++)
        {
            Lines[i].enabled = false;
        }
    }

    private bool FindTarget()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + GetComponent<Rigidbody>().velocity.normalized, GetComponent<Rigidbody>().velocity.normalized, out hit, (lastPos - transform.position).magnitude + 1f, LayerMask.GetMask("Player"));
        if (hit.collider == null)
        {
            return false;
        }

        var player = (hit.collider.isTrigger) ? hit.collider.GetComponent<PlayerHealth_Net>() : hit.collider.GetComponentInParent<PlayerHealth_Net>();

        if (player.Id == Id)
            return false;

        Target = player.transform;
        return true;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetButtonDown("CameraSwitch"))
        {
            GetComponent<CameraSwitch>().ChangeCamera();
        }

        if (heatseekOnCooldown) //Cooldown för UI
        {
            GetComponent<Player>().HeatSeekActive.fillAmount += 1f / HeatseekingCooldown * Time.deltaTime;
            if (missileAmount <= 0)
            {
                GetComponent<Player>().HeatSeekActive.fillAmount = 0f;
                heatseekOnCooldown = false;
                CurrentGun = 0;
            }
        }

        if (minesOnCooldown) //Cooldown för UI
        {
            GetComponent<Player>().MineActive.fillAmount += 1f / MineCooldown * Time.deltaTime;
            if (mineAmount <= 0)
            {
                GetComponent<Player>().MineActive.fillAmount = 0f;
                minesOnCooldown = false;
            }
        }

        if (shotgunOnCooldown) //Cooldown för UI
        {
            GetComponent<Player>().SpreadActive.fillAmount += 1f / 0.5f * Time.deltaTime;
            if (scatterAmmo <= 0)
            {
                GetComponent<Player>().SpreadActive.fillAmount = 0f;
                shotgunOnCooldown = false;
                CurrentGun = 0;
            }
        }
        if (scatterAmmo == 0)
        {
            StopLines();
            scattered = false;
        }
        if (hitDetection == true)
        {
            GetComponent<Player>().CrossLock.enabled = true;
        }
        if (scatterTimer >= lineTime && scattered)
        {
            StopLines();
            scattered = false;
        }
        if (CurrentGun == 2)
        {
            FindTarget();
        }
        if (RocketAmmo <= 8 || gunOverheating > 0)
        {
            GetComponent<Player>().MGOverheat.fillAmount = gunOverheating / Overheated;
            GetComponent<Player>().RocketOverheat.fillAmount = RocketAmmo / maxRounds;
            GetComponent<Player>().RocketAmount.text = RocketAmmo.ToString();
        }

        if (gunOverheating > 0 && !BurnedOut)
        {
            GetComponent<Player>().MGInfinity.fillAmount = gunOverheating / Overheated;
        }

        _shotTimer += Time.deltaTime;
        _mineTimer += Time.deltaTime;
        _heatSeekingTimer += Time.deltaTime;

        if ((Input.GetButton("FireP1") || Input.GetAxis("FireP1") > 0) && (_shotTimer >= ShotCooldown))
        {
            if (PV.JustStarted || !PH.Alive)
            {
                return;
            } else
            {
                if (CurrentGun == 2 && missileAmount > 0 && _heatSeekingTimer >= HeatseekingCooldown)
                {
                    if (missileAmount > 0) {
                        StartCoroutine(Vib.ShotMissileVib());
                        --missileAmount;
                        updateHeatseekUI();
                        CmdShootHeatSeeking(Id);
                        _heatSeekingTimer = 0.0F;
                        GetComponent<Player>().HeatAmmo.fillAmount = missileAmount / 2;
                    }
                }
                if (CurrentGun == 3 && scatterAmmo != 0)
                {
                    StartCoroutine(Vib.ShotScatterVib());
                    CmdShootScatter(gameObject, Id);
                    scatterAmmo--;
                    updateSpreadUI();
                    scatterTimer = 0;
                    scattered = true;
                    GetComponent<Player>().SpreadAmmo.fillAmount = scatterAmmo / 5;
                }
                if (CurrentGun == 0 && RocketAmmo != 0)
                {
                    StartCoroutine(Vib.ShotMissileVib());
                    CmdShoot(Id);
                    RocketAmmo--;
                    rocketRefill = 0;
                }
                if (CurrentGun == 1 && gunOverheating < 10 && !BurnedOut)
                {
                    StartCoroutine(Vib.ShotBulletVib());
                    CmdShoot2(Id);
                }
                _shotTimer = 0.0F;
            }
        }

        //RocketAmmo
        rocketRefill += Time.deltaTime;
        if (RocketAmmo < 0)
        {
            RocketAmmo = 0;
        } else if (RocketAmmo >= 8)
        {
            RocketAmmo = 8;
        }
        if (rocketRefill >= 1 && RocketAmmo <= 8)
        {
            RocketAmmo++;
            rocketRefill = 0;
        }
        //Overheat på MachineGun
        if ((Input.GetButton("FireP1") || Input.GetAxis("FireP1") > 0) && (CurrentGun == 1) && !BurnedOut)
        {
            if (gunOverheating >= 10)
            {
                gunOverheating = 10;
                BurnedOut = true;
                GetComponent<Player>().MGInfinity.fillAmount = 1;
                GetComponent<AudioController_Net>().PlayOneshotSound(GetComponent<AudioSource>(), 3, 0.5f); //Play OverheatSound
            }
            gunOverheating += MGshooting * Time.deltaTime;

        } else if ((Input.GetButton("FireP1") || Input.GetAxis("FireP1") <= 0) && (gunOverheating != 0) || BurnedOut)
        {
            gunOverheating -= MGshooting * Time.deltaTime;
            if (gunOverheating <= 0)
            {
                gunOverheating = 0;
            }

            if (BurnedOut && gunOverheating <= 0)
            {
                OverheatEnd();
            }
        }
        //Overheat på MachineGun
        // UI FÖR VAPNENA
        if (CurrentGun == 0)
        {
            RocketsOn();
        } else {
            RocketsOff();
        }
        if (CurrentGun == 1)
        {
            MachinegunOn();
        } else {
            MachinegunOff();
        }
        if (CurrentGun == 2)
        {
            HeatSeekOn();
        } else
        {
            HeatSeekOff();
        }
        if (CurrentGun == 3)
        {
            scatterTimer += Time.deltaTime;
            ScatterOn();
        }
        else
        {
            ScatterOff();
        }
        if (scatterAmmo == 0)
        {
            GetComponent<Player>().ScatterCanvas.alpha = 0.25f;
        }
        else
        {
            GetComponent<Player>().ScatterCanvas.alpha = 1f;
        }
        if (mineAmount == 0)
        {
            GetComponent<Player>().MineActive.enabled = false;
        }
        if (missileAmount == 0)
        {
            GetComponent<Player>().HeatCanvas.alpha = 0.25f;
        }
        else
        {
            GetComponent<Player>().HeatCanvas.alpha = 1f;
        }
        //UI för vapnena

        if ((Input.GetButtonDown("ChangeWeaponA") || Input.GetAxis("ChangeWeaponA") > 0.5) && (_mineTimer >= MineCooldown))
        {
            if (PV.JustStarted || !PH.Alive)    //Prevent mines from being dropped during countdown
            {
                return;
            }
            else
            {
                if (mineAmount > 0)
                {
                    CmdLayMine(Id);
                    --mineAmount;
                    updateMineUI();
                    _mineTimer = 0.0F;
                }
            }
        }
        if (Input.GetButtonDown("ChangeWeaponX") && scatterAmmo != 0)
        {
            CurrentGun = 3;
            GetComponent<AudioController_Net>().PlayUIsound(GetComponent<AudioSource>(), 1);
        }
        if (Input.GetButtonDown("ChangeWeaponB") && missileAmount != 0)
        {
            CurrentGun = 2;
            GetComponent<AudioController_Net>().PlayUIsound(GetComponent<AudioSource>(), 2);
        }
        if (Input.GetButtonDown("ChangeWeaponY"))
        {
            if (CurrentGun > 0)
            {
                CurrentGun = 0;
                GetComponent<AudioController_Net>().PlayUIsound(GetComponent<AudioSource>(), 3);
            }
            else if (CurrentGun < 1)
            {
                CurrentGun = 1;
                GetComponent<AudioController_Net>().PlayUIsound(GetComponent<AudioSource>(), 4);
            }
        }
    }

    private void RocketsOn()
    {
        ShotCooldown = 0.3f;
        GetComponent<Player>().RocketActive.enabled = true;
        GetComponent<Player>().CrosshairRockets.SetActive(true);
        GetComponent<Player>().RocketInactive.enabled = false;
        GetComponent<Player>().MGInactive.enabled = false;
        GetComponent<Player>().MGInfinityFrame.enabled = false;
        GetComponent<Player>().RocketAmount.enabled = true;
    }

    private void RocketsOff()
    {
        GetComponent<Player>().RocketActive.enabled = false;
        GetComponent<Player>().CrosshairRockets.SetActive(false);
        GetComponent<Player>().RocketAmount.enabled = false;
    }

    private void MachinegunOn()
    {
        ShotCooldown = 0.15f;
        GetComponent<Player>().MGInfinity.enabled = true;
        GetComponent<Player>().MGActive.enabled = true;
        GetComponent<Player>().CrosshairMachinegun.SetActive(true);
        GetComponent<Player>().RocketInactive.enabled = false;
        GetComponent<Player>().MGInactive.enabled = false;
        GetComponent<Player>().MGInfinityFrame.enabled = true;
    }

    private void MachinegunOff()
    {
        GetComponent<Player>().MGActive.enabled = false;
        GetComponent<Player>().CrosshairMachinegun.SetActive(false);
        GetComponent<Player>().MGInfinityFrame.enabled = false;
    }

    private void HeatSeekOn()
    {
            GetComponent<Player>().RocketInactive.enabled = true;
            GetComponent<Player>().HeatSeekActive.enabled = true;
            GetComponent<Player>().HeatAmount.text = missileAmount.ToString();
            GetComponent<Player>().CrosshairMachinegun.SetActive(false);
            GetComponent<Player>().CrosshairRockets.SetActive(false);
            GetComponent<Player>().CrosshairHeatseek.SetActive(true);
    }

    private void HeatSeekOff()
    {
        GetComponent<Player>().CrosshairHeatseek.SetActive(false);
        GetComponent<Player>().HeatSeekActive.enabled = false;

    }

    private void ScatterOn()
    {
        //UI stuff
        ShotCooldown = 1f;
        GetComponent<Player>().RocketInactive.enabled = true;
        GetComponent<Player>().SpreadActive.enabled = true;
        GetComponent<Player>().CrosshairSpread.SetActive(true);
    }

    private void ScatterOff()
    {
        //UI Stuff
        scatterTimer = 0;
        GetComponent<Player>().SpreadActive.enabled = false;
        GetComponent<Player>().CrosshairSpread.SetActive(false);
    }

    [Command]
    private void CmdShoot(short owner)
    {
        var missile = Instantiate(ShotPrefabs[0], _shotOrigins[_activeGun].position, transform.rotation).GetComponent<Missile_Net>();
        RpcMuzzleFlash(_activeGun);
        missile.shooter = GetComponent<PlayerVariables_Net>();
        missile.setOwner(gameObject);
        
        //AUTOAIM
        var hits = Physics.BoxCastAll(_shotOrigins[_activeGun].position + missile.transform.forward * 10, Vector3.one * 6f, transform.forward, transform.localRotation, 200, LayerMask.GetMask("Player"));
        Transform target = null;
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.layer == 10)
            {
                target = hit.collider.transform;
                RpcHitMarker();
            }
        }
        if (target != null)
            missile.transform.forward = (target.position - missile.transform.position).normalized;

        missile.Owner = owner;
        missile.GetComponent<Rigidbody>().velocity = (missile.transform.forward * missile.ProjectileSpeed);
        NetworkServer.Spawn(missile.gameObject);
        RpcPlayAudio(0);
        _activeGun += 1;
        if (_activeGun > 1)
        {
            _activeGun = 0;
        }
    }

    [Command]
    private void CmdShoot2(short owner)
    {
        var bullet = Instantiate(ShotPrefabs[1], _shotOrigins[_activeGun].position, transform.rotation).GetComponent<Bullet_Net>();
        RpcMuzzleFlash(_activeGun);
        bullet.setOwner(gameObject);
        bullet.Owner = owner;
        bullet.shooter = GetComponent<PlayerVariables_Net>();  

        //AutoAim
        var hits = Physics.BoxCastAll(_shotOrigins[_activeGun].position + bullet.transform.forward * 10, Vector3.one * 6f, transform.forward, transform.localRotation, 200, LayerMask.GetMask("Player"));
        Transform target = null;
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.layer == 10)
            {
                target = hit.collider.transform;
                RpcHitMarker();
            }
        }
        if (target != null)
            bullet.transform.forward = (target.position - bullet.transform.position).normalized;

        bullet.GetComponent<Rigidbody>().velocity = (bullet.transform.forward * bullet.ProjectileSpeed);
        bullet.transform.Rotate(0, -90, 0);
        NetworkServer.Spawn(bullet.gameObject);
        RpcPlayAudio(1);
        _activeGun += 1;
        if (_activeGun > 1)
        {
            _activeGun = 0;
        }
    }

    [Command]
    private void CmdShootScatter(GameObject p, short owner)
    {
        RpcPlayAudio(3);
        RpcSyncLines(p, true);
        for (int i = 0; i < ScatterFragments; i++)
        {
            //Lines[i].enabled = true;
            //Lines[i].SetPosition(0, transform.position);
            RpcScatterLines( i, Vector3.zero);
            RaycastHit Hit;
            Quaternion fireRotation = Quaternion.LookRotation(transform.forward);
            Quaternion randomRotation = Random.rotation;
            fireRotation = Quaternion.RotateTowards(fireRotation, randomRotation, Random.Range(0.0f, ScatterVariance));
            
            if (Physics.Raycast(_shotOrigins[_activeGun].position, fireRotation * Vector3.forward, out Hit, ScatterRange))
            {
                if (Hit.collider.CompareTag("HeatMissile"))
                {
                    Hit.collider.GetComponent<HeatSeekingMissile_Net>().Explode();
                }
                else
                {
                    var player = (Hit.collider.isTrigger) ? Hit.collider.GetComponent<PlayerHealth_Net>() : Hit.collider.GetComponentInParent<PlayerHealth_Net>();
                    RpcScatterLines(i, Hit.point);

                    if (player != null && player != gameObject)
                    {
                        player.TakeDamage(15, gameObject, "Scatter Shot");
                        RpcHitMarker();
                    }
                }
            }     
        }
    }

    [ClientRpc]
    private void RpcSyncLines(GameObject p, bool on)
    {
        p.transform.Find("ScatterLinesSyncer").gameObject.SetActive(on);
        DisableScatterLines(p);
    }

    private void DisableScatterLines(GameObject p)
    {
        StartCoroutine(DisableScatter(p));
    }

    private IEnumerator DisableScatter(GameObject p)
    {
        yield return new WaitForSeconds(0.2f);
        p.transform.Find("ScatterLinesSyncer").gameObject.SetActive(false);
    }

    [ClientRpc]
    private void RpcScatterLines(int index, Vector3 hitPoint)
    {
        if (!isLocalPlayer) return;


        Lines[index].enabled = true;
        Lines[index].SetPosition(0, transform.position);
        if(hitPoint != Vector3.zero)
            Lines[index].SetPosition(1, hitPoint);
    }

    [Command]
    private void CmdShootHeatSeeking(short owner)
    {
            var heatMissile = Instantiate(ShotPrefabs[3], _shotOrigins[_activeGun].position, transform.rotation).GetComponent<HeatSeekingMissile_Net>();
            heatMissile.setOwner(gameObject);
            heatMissile.Target = Target;
            heatMissile.Owner = owner;
            NetworkServer.Spawn(heatMissile.gameObject);
            RpcPlayAudio(2);
            _activeGun += 1;
            if (_activeGun > 1)
            {
                _activeGun = 0;
            }
        }

    [ClientRpc]
    private void RpcHitMarker()
    {
        StartCoroutine(Hitflash());
    }

    IEnumerator Hitflash()
    {
        GetComponent<Player>().Hitmark.enabled = true;
        yield return new WaitForSeconds(0.3f);
        GetComponent<Player>().Hitmark.enabled = false;
    }


    [Command]
    private void CmdLayMine(short owner)
    {
            var mine = Instantiate(ShotPrefabs[2], _shotOrigins[2].position, transform.rotation).GetComponent<Mine_Net>();
            mine.setOwner(gameObject);
            mine.Owner = owner;
            NetworkServer.Spawn(mine.gameObject);
    }

    [ClientRpc]
    public void RpcMuzzleFlash(int x)
    {
        GameObject muzzleFlash = Instantiate(muzzleFlashParticle, _shotOrigins[(x + 3)].position, transform.rotation);
        NetworkServer.Spawn(muzzleFlash);
        Destroy(muzzleFlash, 0.5f);
    }

    [ClientRpc]
    public void RpcPlayAudio(short x)
    {
        if (!isLocalPlayer) return;

        switch (x)
        {
            case 0: //MissileSound
                GetComponent<AudioController_Net>().PlayMissileSound(GetComponent<AudioSource>(), 0.5f);
                break;
            case 1: //MachineGunSound
                GetComponent<AudioController_Net>().PlayMachineGunSound(GetComponent<AudioSource>(), 0.5f);
                break;
            case 2: //HeatseekingSound
                GetComponent<AudioController_Net>().PlayHeatseekingList(GetComponent<AudioSource>(), 1f);
                break;
            case 3: //ShotgunSound
                GetComponent<AudioController_Net>().PlayShotgunSound(GetComponent<AudioSource>(), 0.3f);
                break;
            default:
                break;
        }
    }

    //Recharge Mines
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MinePickup"))
        {
            other.GetComponent<PickUpVariables_Net>().RespawnMines();
            Destroy(other.gameObject);
            mineAmount = 3;
            GetComponent<Player>().MineAmount.text = "3";
            GetComponent<Player>().MineActive.fillAmount = 100;
            GetComponent<Player>().MineActive.enabled = true;
            GetComponent<AudioController_Net>().PlayOneshotSound(GetComponent<AudioSource>(), 9, 1f);
            StartCoroutine(ShowPickUpText(1));
        }

        if (other.gameObject.CompareTag("HeatseekingPickup"))
        {
            other.GetComponent<PickUpVariables_Net>().RespawnHeatseeking();
            Destroy(other.gameObject);
            missileAmount = 2;
            GetComponent<Player>().HeatAmount.text = "2";
            GetComponent<Player>().HeatSeekActive.fillAmount = 100;
            GetComponent<Player>().HeatAmount.text = missileAmount.ToString();
            GetComponent<Player>().HeatAmmo.fillAmount = missileAmount / 2;
            GetComponent<AudioController_Net>().PlayOneshotSound(GetComponent<AudioSource>(), 11, 1f);
            StartCoroutine(ShowPickUpText(2));
        }

        if (other.gameObject.CompareTag("PickUpSpread"))
        {
            other.GetComponent<PickUpVariables_Net>().RespawnScatter();
            Destroy(other.gameObject);
            scatterAmmo = 5;
            GetComponent<Player>().SpreadAmount.text = "5";
            GetComponent<Player>().SpreadActive.fillAmount = 1;
            GetComponent<Player>().SpreadAmmo.fillAmount = 1;
            GetComponent<Player>().SpreadAmount.text = scatterAmmo.ToString();
            GetComponent<AudioController_Net>().PlayOneshotSound(GetComponent<AudioSource>(), 10, 1f);
            StartCoroutine(ShowPickUpText(3));
        }
    }

    public void updateHeatseekUI()
    {
        heatseekOnCooldown = true;
        GetComponent<Player>().HeatSeekActive.fillAmount = 0f;
        GetComponent<Player>().HeatAmount.text = missileAmount.ToString();
        //GetComponent<Player>().HeatSeekActive.fillAmount = (missileAmount / 2);
    }

    public void updateSpreadUI()
    {
        shotgunOnCooldown = true;
        GetComponent<Player>().SpreadActive.fillAmount = 0f;
        GetComponent<Player>().SpreadAmount.text = scatterAmmo.ToString();
        //GetComponent<Player>().SpreadActive.fillAmount = (scatterAmmo / 5);
    }

    public void updateMineUI()
    {
        minesOnCooldown = true;
        GetComponent<Player>().MineActive.fillAmount = 0f;
        GetComponent<Player>().MineAmount.text = mineAmount.ToString();
        //GetComponent<Player>().MineActive.fillAmount = (mineAmount / 3);
    }

    public void OverheatEnd()
    {
        GetComponent<AudioController_Net>().PlayOneshotSound(GetComponent<AudioSource>(), 4, 0.3f); //Play overheatdone
        GetComponent<Player>().MGInfinity.fillAmount = 0;
        BurnedOut = false;
    }

    IEnumerator ShowPickUpText(short x)
    {
        if (x == 1) {
            PickUpText.text = "Proximity Mines Picked Up \nUse with [A]";
        }
        if (x == 2) {
            PickUpText.text = "Heatseeking Picked Up \nActivate with [B]";
        }
        if (x == 3)
        {
            PickUpText.text = "Scatter shot picked up \nActivate with [X]";
        }
        yield return new WaitForSeconds(3);
        PickUpText.text = "";
    }
}