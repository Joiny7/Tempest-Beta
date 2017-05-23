using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class HeatSeekingMissile_Net : NetworkBehaviour
{
    public float ProjectileSpeed = 100f;
    public float RotationSpeed;
    public float LifeTime = 10f;
    [SyncVar]
    public int Damage = 70;
    public short Owner;
    public GameObject Explosion;
    private Rigidbody _body;
    private PlayerHealth_Net _hitPlayer;
    public PlayerVariables_Net shooter;
    private GameObject _owner;
    public Transform Target;
    private Vector3 lastPos;
    private float _age;
    private bool Hit;
    public GameObject closestTarget;
    private Vector3 distanceDif;
    private Vector3 playerPos;
    private float currentDistance;
    private float oldDistance;
    private GameObject[] Targets;

    void Start () {
        lastPos = transform.position;
        _body = GetComponent<Rigidbody>();
        FindEnemy();
        GetComponent<AudioSource>().Play();
    }

    [ServerCallback]
    private void Update()
    {
        if (_hitPlayer)
        {
            _hitPlayer.TakeDamage(Damage, _owner, "[Heatseeking]");
            Explode();
            NetworkServer.Destroy(gameObject);
        }

        if (PredictCollisionWithPlayer())
            return;

        lastPos = transform.position;
        _age += Time.deltaTime;

        if (_age >= LifeTime)
            NetworkServer.Destroy(gameObject);
        FollowEnemy1();
    }

    public void setOwner(GameObject g)
    {
        _owner = g;
    }

    [ServerCallback]
    public void Explode()
    {
        GameObject explosion = (GameObject)Instantiate(Explosion, transform.position, Quaternion.identity);
        NetworkServer.Spawn(explosion);
        Destroy(explosion, 3f);
        NetworkServer.Destroy(gameObject);
    }

    private void FollowEnemy1()
    {
        if (closestTarget)
        {
            if (!Hit)
            {
                transform.LookAt(closestTarget.transform.position);
                transform.position = Vector3.MoveTowards(transform.position, closestTarget.transform.position, Time.deltaTime * ProjectileSpeed * 2);
                if (transform.position == closestTarget.transform.position)
                {
                    Hit = true;
                    closestTarget.gameObject.GetComponent<PlayerHealth_Net>().TakeDamage(Damage, _owner, "[Heatseeking]");
                    Explode();
                    NetworkServer.Destroy(gameObject);
                }
            }
        }
        else
        {
            gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * ProjectileSpeed;
        }          
    }

    public bool PredictCollisionWithPlayer()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + _body.velocity.normalized, _body.velocity.normalized, out hit, (lastPos - transform.position).magnitude + 20f, LayerMask.GetMask("Player"));

        if (hit.collider == null)
        {
            return false;
        }

        var player = (hit.collider.isTrigger) ? hit.collider.GetComponent<PlayerHealth_Net>() : hit.collider.GetComponentInParent<PlayerHealth_Net>();

        if (player.Id == Owner && player.Alive == false)
        {
            return false;
        }

        _hitPlayer = player;
        transform.position = hit.point;
        return true;
    }


    private void FindEnemy()
    {
        oldDistance = Mathf.Infinity;
        playerPos = _owner.transform.position;
        Targets = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < Targets.Length; i++)
        {
            distanceDif = Targets[i].transform.position - playerPos;
            currentDistance = distanceDif.sqrMagnitude;
            if (currentDistance < oldDistance && Targets[i].gameObject != _owner)
            {
               // if (Targets[i].gameObject.GetComponent<PlayerHealth_Net>().Alive)
                //{
                    closestTarget = Targets[i];
                    oldDistance = currentDistance;
                //}
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (!isServer)
            return;

        Explode();
        NetworkServer.Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerHealth_Net>();
            if (player.Id != Owner)
            {
                if (player.Health <= Damage)
                {
                    shooter.GetComponent<PlayerVariables_Net>().setPlayerScore();
                }
                player.TakeDamage(Damage, _owner, "[Heatseeking]");
                Explode();
                NetworkServer.Destroy(gameObject);
            }
        }

        if (other.CompareTag("Mine") || other.CompareTag("HeatseekBlocker"))
        {
            Explode();
        }
    }

}
