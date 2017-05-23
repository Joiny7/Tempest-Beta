using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class KillFeed : NetworkBehaviour {

    public List<KillFeedInfo> KFI = new List<KillFeedInfo>();
    public List<Texture> Guns = new List<Texture>();
    private GUIStyle style;
    public Font font;

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 10;
        style.font = font;
        style.normal.textColor = Color.white;
        style.richText = true;

        GUILayout.BeginArea(new Rect(Screen.width - 300, 0, 300, 100));
        foreach (KillFeedInfo k in KFI)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(k.killer, style);
            GUILayout.Label(k.gun);
            GUILayout.Label(k.killed, style);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    [ClientRpc]
    public void RpcSendKillFeed(string Killer, string Gun, string Killed)
    {
        switch (Gun)
        {
            case "[Missile]":
                ConvertParamsToClass(Killer, Guns[0], Killed);
                break;
            case "[MachineGun]":
                ConvertParamsToClass(Killer, Guns[1], Killed);
                break;
            case "[Heatseeking]":
                ConvertParamsToClass(Killer, Guns[2], Killed);
                break;
            case "[Mine]":
                ConvertParamsToClass(Killer, Guns[3], Killed);
                break;
            case "[Shotgun]":
                ConvertParamsToClass(Killer, Guns[4], Killed);
                break;
            default:
                break;
        }     
    }

    private void ConvertParamsToClass(string Killer, Texture Gun, string Killed)
    {
        KillFeedInfo k = new KillFeedInfo();
        k.killer = Killer;
        k.gun = Gun;
        k.killed = Killed;
        KFI.Add(k);
        StartCoroutine(RemoveFromFeed(5, k));
    }

    IEnumerator RemoveFromFeed(short x, KillFeedInfo k)
    {
        yield return new WaitForSeconds(x);
        KFI.Remove(k);
    }

    [System.Serializable]
    public class KillFeedInfo
    {
        public string killer;
        public Texture gun;
        public string killed;
    }
}