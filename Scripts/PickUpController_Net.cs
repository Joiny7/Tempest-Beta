using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PickUpController_Net : NetworkBehaviour
{

    public float mineRespawn;           //For respawning mines
    public float mineStartSpawn;        //For spawning mines
    public float heatseekingRespawn;    //For respawning heatseekings
    public float heatseekingStartSpawn; //For spawning heatseekings
    public float scatterRespawn;
    public float scatterStartSpawn;
    public Transform[] spawnPoints;
    public GameObject minePickupPrefab;
    public GameObject heatseekingPickupPrefab;
    public GameObject ScatterPickUpPrefab;
    public float matchTimer;

    void Start()
    {
        StartCoroutine(StartScatterPickUp(0));
        StartCoroutine(StartHeatseekingPickUp(1));
        StartCoroutine(StartMinePickUp(2));
    }

    IEnumerator StartScatterPickUp(short x)
    {
        yield return new WaitForSeconds(scatterStartSpawn);
        var pickUp = Instantiate(ScatterPickUpPrefab, spawnPoints[x].position, transform.rotation);
        pickUp.GetComponent<PickUpVariables_Net>().SetID(x);
    }

    public void RespawnScatter(short x)
    {
        StartCoroutine(RespawnScatterPickup(x));
    }

    IEnumerator RespawnScatterPickup(short x)
    {
        yield return new WaitForSeconds(scatterRespawn);
        var pickUp = Instantiate(ScatterPickUpPrefab, spawnPoints[x].position, transform.rotation);
        pickUp.GetComponent<PickUpVariables_Net>().SetID(x);
    }

    IEnumerator StartMinePickUp(short x)
    {
        yield return new WaitForSeconds(mineStartSpawn);
        var pickUp = Instantiate(minePickupPrefab, spawnPoints[x].position, transform.rotation);
        pickUp.GetComponent<PickUpVariables_Net>().SetID(x);
    }
    
    public void RespawnMines(short x)
    {
       StartCoroutine(RespawnMinePickUp(x));
    }

    IEnumerator RespawnMinePickUp(short x)
    { 
        yield return new WaitForSeconds(mineRespawn);
        var pickUp = Instantiate(minePickupPrefab, spawnPoints[x].position, transform.rotation);
        pickUp.GetComponent<PickUpVariables_Net>().SetID(x);
    }

    //Heatseeking pickup
    IEnumerator StartHeatseekingPickUp(short x)
    {
        yield return new WaitForSeconds(heatseekingStartSpawn);
        var pickUp = Instantiate(heatseekingPickupPrefab, spawnPoints[x].position, transform.rotation);
        pickUp.GetComponent<PickUpVariables_Net>().SetID(x);
    }

    public void RespawnHeatseeking(short x)
    {
        StartCoroutine(RespawnHeatseekingPickup(x));
    }

    IEnumerator RespawnHeatseekingPickup(short x)
    {
        yield return new WaitForSeconds(heatseekingRespawn);
        var pickUp = Instantiate(heatseekingPickupPrefab, spawnPoints[x].position, transform.rotation);
        pickUp.GetComponent<PickUpVariables_Net>().SetID(x);
    }
}
