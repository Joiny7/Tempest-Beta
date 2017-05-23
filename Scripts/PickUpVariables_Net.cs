using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickUpVariables_Net : NetworkBehaviour
{

    public short ID;
    public GameObject PUC;

    private void Start()
    {
         PUC = GameObject.FindWithTag("Controller");
    }

    public void RespawnMines()
    {
        PUC.GetComponent<PickUpController_Net>().RespawnMines(ID);
    }

    public void RespawnHeatseeking()
    {
        PUC.GetComponent<PickUpController_Net>().RespawnHeatseeking(ID);
    }
    
    public void RespawnScatter()
    {
        PUC.GetComponent<PickUpController_Net>().RespawnScatter(ID);
    }

    public void SetID(short x)
    {
        ID = x;
    }
}