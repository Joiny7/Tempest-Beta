using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;

public class Player : NetworkBehaviour
{

    public GameObject ThirdPersonCameraPrefab;
    public AutoCam playerCam;
    [SyncVar]
    public string PlayerNick;
    [SyncVar]
    public Color PlayerColor;
    public short LocalPlayerId;

    [HideInInspector] public GameObject Ui;
    [HideInInspector] public Image HealthBar;
    [HideInInspector] public GameObject WeaponNew;
    [HideInInspector] public GameObject Score2;
    [HideInInspector] public GameObject Crosshairs;
    [HideInInspector] public GameObject CrosshairMachinegun;
    [HideInInspector] public GameObject CrosshairRockets;
    [HideInInspector] public GameObject CrosshairHeatseek;
    [HideInInspector] public GameObject CrosshairSpread;
    [HideInInspector] public Image SpreadCircle;
    [HideInInspector] public Image SpreadAmmoEmpty;
    [HideInInspector] public Image SpreadAmmo;
    [HideInInspector] public Image MGActive;
    [HideInInspector] public Image MGInactive;
    [HideInInspector] public Image RocketActive;
    [HideInInspector] public Image RocketInactive;
    [HideInInspector] public Image SpreadActive;
    [HideInInspector] public Image MineActive;
    [HideInInspector] public Image HeatSeekActive;
    [HideInInspector] public Image MGOverheat;
    [HideInInspector] public Image RocketOverheat;
    [HideInInspector] public Text MineAmount;
    [HideInInspector] public Text HeatAmount;
    [HideInInspector] public Text SpreadAmount;
    [HideInInspector] public Text RocketAmount;
    [HideInInspector] public Text YourScore;   // dis
	[HideInInspector] public Text TimeLeft;
    [HideInInspector] public Text HighestScore;   // dis
    [HideInInspector] public Text KillConfirm;
    [HideInInspector] public Text Leader;   // dis
    [HideInInspector] public Text You;   // dis
    [HideInInspector] public Text WinLoseText;
    [HideInInspector] public Image CrossLock;
    [HideInInspector] public Image Hitmark;
    [HideInInspector] public Image MGInfinity;
    [HideInInspector] public Image MGInfinityFrame;
    [HideInInspector] public CanvasGroup HeatCanvas;
    [HideInInspector] public CanvasGroup ScatterCanvas;
    [HideInInspector] public Image DamageImage;
    [HideInInspector] public Image HeatAmmo;

    public override void OnStartLocalPlayer()
    {
        LocalPlayerId = (short)UnityEngine.Random.Range(0, 10000);
        GetComponent<PlayerShooting_Net>().Id = LocalPlayerId;
        GetComponent<PlayerHealth_Net>().Id = LocalPlayerId;
        GetComponent<PlayerVariables_Net>().Id = LocalPlayerId;
        var cam = Instantiate(ThirdPersonCameraPrefab, Vector3.zero, Quaternion.identity);
        var model = transform.Find("Drone_P1");                            
        cam.GetComponent<AutoCam>().SetTarget(model);                           
        Ui = cam.transform.Find("ThirdPersonUI").gameObject;
        HealthBar = Ui.transform.Find("healthFilled").GetComponent<Image>();
        WeaponNew = Ui.transform.Find("WeaponNew").gameObject;
        Score2 = Ui.transform.Find("Score2").gameObject;
        MGActive = WeaponNew.transform.Find("MG/MachineActive").GetComponent<Image>();  //ÄNDRAD
        MGInactive = WeaponNew.transform.Find("MG/MachineInactive").GetComponent<Image>();  //ÄNDRAD
        MGInfinity = WeaponNew.transform.Find("MG/MGInfinityRed").GetComponent<Image>();
        MGInfinityFrame = WeaponNew.transform.Find("MG/MGInfinity").GetComponent<Image>();
        RocketActive = WeaponNew.transform.Find("Rocket/MissileActive").GetComponent<Image>(); //ÄNDRAD
        RocketInactive = WeaponNew.transform.Find("Rocket/MissileInactive").GetComponent<Image>(); //ÄNDRAD
        SpreadActive = WeaponNew.transform.Find("Spread/SpreadActive").GetComponent<Image>();
        ScatterCanvas = WeaponNew.transform.Find("Spread").GetComponent<CanvasGroup>();
        HeatSeekActive = WeaponNew.transform.Find("Heat/HeatActive").GetComponent<Image>();
        HeatCanvas = WeaponNew.transform.Find("Heat").GetComponent<CanvasGroup>();
        MineActive = WeaponNew.transform.Find("Mine/MineActive").GetComponent<Image>();
        MineAmount = WeaponNew.transform.Find("Mine/MineAmount").GetComponent<Text>();
        HeatAmount = WeaponNew.transform.Find("Heat/HeatAmount").GetComponent<Text>();
        RocketAmount = WeaponNew.transform.Find("Rocket/RocketAmount").GetComponent<Text>();
        SpreadAmount = WeaponNew.transform.Find("Spread/SpreadAmount").GetComponent<Text>();
        Crosshairs = Ui.transform.Find("Crosshairs").gameObject;
        CrosshairMachinegun = Crosshairs.transform.Find("CrosshairMachinegun").gameObject;
        CrosshairRockets = Crosshairs.transform.Find("CrosshairRockets").gameObject;
        CrosshairHeatseek = Crosshairs.transform.Find("CrosshairHeatseek").gameObject;
        HeatAmmo = Crosshairs.transform.Find("CrosshairHeatseek/HeatAmmoTwo").GetComponent<Image>();
        CrosshairSpread = Crosshairs.transform.Find("CrosshairSpread").gameObject;
        MGOverheat = Crosshairs.transform.Find("CrosshairMachinegun/CrosshairOverheated").GetComponent<Image>();
        CrossLock = Crosshairs.transform.Find("CrossLock").GetComponent<Image>();
        SpreadCircle = Crosshairs.transform.Find("CrosshairSpread/SpreadCircle").GetComponent<Image>();
        SpreadAmmoEmpty = Crosshairs.transform.Find("CrosshairSpread/SpreadAmmoEmpty").GetComponent<Image>();
        SpreadAmmo = Crosshairs.transform.Find("CrosshairSpread/SpreadAmmo").GetComponent<Image>();
        RocketOverheat = Crosshairs.transform.Find("CrosshairRockets/RocketFilled").GetComponent<Image>();
        YourScore = Score2.transform.Find("YourScore").GetComponent<Text>();
		TimeLeft = Score2.transform.Find("Timeleft").GetComponent<Text>();
        HighestScore = Score2.transform.Find("HighScore").GetComponent<Text>();
        DamageImage = Ui.transform.Find("dmgImg").GetComponent<Image>();
        Hitmark = Crosshairs.transform.Find("Hitmark").GetComponent<Image>();
        KillConfirm = Score2.transform.Find("KillConfirm").GetComponent<Text>();
        Leader = Score2.transform.Find("Leader").GetComponent<Text>();
        You = Score2.transform.Find("You").GetComponent<Text>();
        WinLoseText = Ui.transform.Find("WINLOSE").GetComponent<Text>();
        GetComponent<BoostBar_Net>().BoostFilledTP = Ui.transform.Find("boostFilled").GetComponent<Image>();
        GetComponent<PlayerVariables_Net>().DangerZoneWarning = Ui.transform.Find("DangerText").GetComponent<Text>();
        GetComponent<PlayerVariables_Net>().RespawnText = Ui.transform.Find("Respawning").GetComponent<Text>();
        GetComponent<PlayerShooting_Net>().PickUpText = Ui.transform.Find("PickUpText").GetComponent<Text>();
        playerCam = cam.GetComponent<AutoCam>();
        Hitmark.enabled = false;
        KillConfirm.text = "";
        You.GetComponent<Outline>().effectColor = PlayerColor;
        YourScore.GetComponent<Outline>().effectColor = PlayerColor;
    }

    [ClientRpc]
    void RpcChangeTrail(GameObject g)
    {
        g.GetComponent<PlayerMovement_Net>().ThrusterTrailLeft.startColor = g.GetComponent<Player>().PlayerColor;
        g.GetComponent<PlayerMovement_Net>().ThrusterTrailRight.startColor = g.GetComponent<Player>().PlayerColor;
    }

    [Command]
    public void CmdPaintTrail(GameObject g)
    {
        g.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        RpcChangeTrail(g);
        gameObject.GetComponent<NetworkIdentity>().RemoveClientAuthority(connectionToClient);
    }
}
