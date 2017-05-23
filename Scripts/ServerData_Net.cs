using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerData_Net : NetworkBehaviour {

    [SyncVar]
    public Color TrailColour;
    public TrailRenderer ThrusterTrailLeft;
    public TrailRenderer ThrusterTrailRight;
    private NetworkIdentity NetID;

    // Use this for initialization
    void Start () {

        ThrusterTrailLeft = transform.Find("Drone_P1").Find("Root").Find("L_Engine").Find("L_Engine_01").Find("L_Engine_02").Find("Thruster_Trail_Left").GetComponent<TrailRenderer>();
        ThrusterTrailRight = transform.Find("Drone_P1").Find("Root").Find("R_Engine").Find("R_Engine_01").Find("R_Engine_02").Find("Thruster_Trail_Right").GetComponent<TrailRenderer>();
        TrailColour = gameObject.GetComponent<Player>().PlayerColor;
        ThrusterTrailLeft.startColor = TrailColour;
        ThrusterTrailRight.startColor = TrailColour;
        CmdSetColors(TrailColour);

        if (isLocalPlayer)
        {
            //ThrusterTrailLeft.startColor = TrailColour;
            //ThrusterTrailRight.startColor = TrailColour;
            //CmdSetColors(TrailColour);
        }
        if(isServer)
        {
            RpcSetColor(TrailColour);
        }
        //CmdSetColors(TrailColour);
    }
 
    [Command]
    public void CmdSetColors(Color c)
    {
        NetID = gameObject.GetComponent<NetworkIdentity>();
        NetID.AssignClientAuthority(connectionToClient);
        c = TrailColour;
        ThrusterTrailLeft.startColor = c;
        ThrusterTrailRight.startColor = c;
        RpcSetColor(c);
        //NetID.RemoveClientAuthority(connectionToClient);
    }

    [ClientRpc]
    void RpcSetColor(Color c)
    {
        c = TrailColour;
        ThrusterTrailLeft.startColor = c;
        ThrusterTrailRight.startColor = c;
    }
}
