using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioController_Net : NetworkBehaviour
{

    private AudioClip[] MachineGunList;
    private AudioClip[] MissileList;
    private AudioClip[] OneShotList;
    private AudioClip[] ShotgunList;
    private AudioClip[] HeatseekingList;
    private AudioClip[] UIList;
    private AudioClip[] PlayerSoundList; 


    //Använd [Rpc] för att spela ljud över server

    void Start () {
        StartPrep();
    }

    #region StartPrep, fill lists
    private void StartPrep()
    {
        PlayerSoundList = new AudioClip[]
		{
			(AudioClip)Resources.Load("Audio/Player/Engine_idle"),                      //0
			(AudioClip)Resources.Load("Audio/Player/Engine_speed"),                     //1
			(AudioClip)Resources.Load("Audio/Player/Boost"),                            //2
            (AudioClip)Resources.Load("Audio/Player/PlayerDeath"),                      //3
		};

        ShotgunList = new AudioClip[]
        {
            (AudioClip)Resources.Load("Audio/Projectiles/Shotgun/ShotgunFire1"),
            (AudioClip)Resources.Load("Audio/Projectiles/Shotgun/ShotgunFire2"),
            (AudioClip)Resources.Load("Audio/Projectiles/Shotgun/ShotgunFire3"),
            (AudioClip)Resources.Load("Audio/Projectiles/Shotgun/ShotgunFire4"),
            (AudioClip)Resources.Load("Audio/Projectiles/Shotgun/ShotgunFire5"),
            (AudioClip)Resources.Load("Audio/Projectiles/Shotgun/ShotgunFire6"),
        };

        MachineGunList = new AudioClip[]
        {
            (AudioClip)Resources.Load("Audio/Projectiles/MachineGun/MachineGunFire1"),
            (AudioClip)Resources.Load("Audio/Projectiles/MachineGun/MachineGunFire2"),
        };
        MissileList = new AudioClip[]
       {
            (AudioClip)Resources.Load("Audio/Projectiles/Missile/MissileFire3"),
            (AudioClip)Resources.Load("Audio/Projectiles/Missile/MissileFire4"),
       };
                HeatseekingList = new AudioClip[]
       {
            (AudioClip)Resources.Load("Audio/Projectiles/Heatseeking/HeatseekingFire"),
       };

        OneShotList = new AudioClip[]
       {
            (AudioClip)Resources.Load("Audio/OneShotSounds/Mine/MineExplosion"),        //0
            (AudioClip)Resources.Load("Audio/OneShotSounds/Pickups/OldPickUp"),         //1
            (AudioClip)Resources.Load("Audio/Player/PlayerDeath"),                      //2
            (AudioClip)Resources.Load("Audio/Player/Overheat"),                         //3
			(AudioClip)Resources.Load("Audio/Player/Overheat_done"),                    //4
            (AudioClip)Resources.Load("Audio/Projectiles/Heatseeking/LockOn"),          //5
            (AudioClip)Resources.Load("Audio/Player/Boost"),                            //6
            (AudioClip)Resources.Load("Audio/Player/Engine_idle"),                      //7
            (AudioClip)Resources.Load("Audio/Player/Engine_speed"),                     //8
            (AudioClip)Resources.Load("Audio/OneShotSounds/Pickups/MinePickup"),        //9
            (AudioClip)Resources.Load("Audio/OneShotSounds/Pickups/ShotgunPickup"),     //10
            (AudioClip)Resources.Load("Audio/OneShotSounds/Pickups/HeatseekingPickup"), //11
            (AudioClip)Resources.Load("Audio/Projectiles/Heatseeking/HeatseekingWarningBeep"), //12
		};

        UIList = new AudioClip[]
        {
            (AudioClip)Resources.Load("Audio/UI/MenuSelect1"),                          //0
            (AudioClip)Resources.Load("Audio/UI/ShotgunSwitch"),                        //1
            (AudioClip)Resources.Load("Audio/UI/HeatseekingSwitch"),                    //2
            (AudioClip)Resources.Load("Audio/UI/MachineGunSwitch"),                     //3
            (AudioClip)Resources.Load("Audio/UI/RocketSwitch"),                         //4
        };
    }
    #endregion 

    public void PlayMissileSound(AudioSource AS, float y) //Audio Source & Volume
    {
        int x = Random.Range(0, MissileList.Length);
        AS.PlayOneShot(MissileList[x], y);

        //if (!AudioS.isPlaying)
        //{
        //    int x = Random.Range(0, MachineGunList.Length);
        //    AudioS.PlayOneShot(MissileList[x], 1);
        //}
    }


    public void PlayShotgunSound(AudioSource AS, float y) //Audio Source & Volume
    {
        int x = Random.Range(0, ShotgunList.Length);
        AS.PlayOneShot(ShotgunList[x], y);

        //if (!AudioS.isPlaying)
        //{
        //    int x = Random.Range(0, MachineGunList.Length);
        //    AudioS.PlayOneShot(MissileList[x], 1);
        //}
    }

    public void PlayMachineGunSound(AudioSource AS, float y) //Audio Source & Volume
    {
        int x = Random.Range(0, MachineGunList.Length);
        AS.PlayOneShot(MachineGunList[x], y);

        //if (!AudioS.isPlaying)
        //{
        //    int x = Random.Range(0, MachineGunList.Length);
        //    AudioS.PlayOneShot(MachineGunList[x], 1);
        //}
    }

    public void PlayHeatseekingList(AudioSource AS, float y) //Audio Source & Volume
    {
        int x = Random.Range(0, HeatseekingList.Length);
        AS.PlayOneShot(HeatseekingList[x], y);

        //if (!AudioS.isPlaying)
        //{
        //    int x = Random.Range(0, MachineGunList.Length);
        //    AudioS.PlayOneShot(MachineGunList[x], 1);
        //}
    }

    public void PlayOneshotSound(AudioSource AS, int x, float y) //Audio Source, Vilket ljud listan & Volume
    {
        AS.PlayOneShot(OneShotList[x], y);

        //if (!AudioS.isPlaying)
        //{
        //    AudioS.PlayOneShot(OneShotList[x], 1);
        //}
    }

    public void PlayUIsound(AudioSource AS, int x) //Audio Source, Vilket ljud i listan
    {
        if (!AS.isPlaying)
        {
            AS.PlayOneShot(UIList[x], 1);
        }
    }

    public void PlayPlayerSoundList(AudioSource AS, int x) //Audio Source, Vilket ljud i listan
    {
        AS.PlayOneShot(PlayerSoundList[x], 1);

        //if (!AudioS.isPlaying)
        //{
        //    AudioS.PlayOneShot(UIList[x], 1);
        //}
    }
}