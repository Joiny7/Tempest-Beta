using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Missile_Net : NetworkBehaviour
{
    public float ProjectileSpeed = 100f;
    public float LifeTime = 5f;
    public int Damage = 10;
    public short Owner;
    public GameObject Explosion;
    private float _age;
    private Rigidbody _body;
    private PlayerHealth_Net _hitPlayer;
    public PlayerVariables_Net shooter;
    public GameObject _owner;

    private Vector3 lastPos;

    private void Start()
    {
        lastPos = transform.position;
        _body = GetComponent<Rigidbody>();
        PredictCollisionWithPlayer();
    }

    public void setOwner(GameObject g)
    {
        _owner = g;
    }

    [ServerCallback]
    private void Update()
    {
        if(_hitPlayer)
        {
            _hitPlayer.TakeDamage(Damage, _owner, "[Missile]");
            Explode();
            NetworkServer.Destroy(gameObject);
            return;
        }            
        if (PredictCollisionWithPlayer())
            return;
        
        lastPos = transform.position;
        _age += Time.deltaTime;

        if (_age >= LifeTime)
            NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    public void Explode()
    {
        GameObject explosion = (GameObject)Instantiate(Explosion, transform.position, Quaternion.identity);
        NetworkServer.Spawn(explosion);
        var trail = transform.Find("Trail");
        trail.parent = null;
        trail.gameObject.AddComponent<NetworkTransform>();
        Destroy(explosion, 3f);
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
                player.TakeDamage(Damage, _owner, "[Missile]");
                Explode();
                NetworkServer.Destroy(gameObject);
            }
        }
    }

    public bool PredictCollisionWithPlayer()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + _body.velocity.normalized, _body.velocity.normalized, out hit, (lastPos - transform.position).magnitude + 5f, LayerMask.GetMask("Player"));

        if (hit.collider == null)
        {
            return false;
        }  
        var player = (hit.collider.isTrigger) ? hit.collider.GetComponent<PlayerHealth_Net>() : hit.collider.GetComponentInParent<PlayerHealth_Net>();

        if (player.Id == Owner)
        {
            return false;
        }           
        _hitPlayer = player;
        transform.position = hit.point;
        return true;
    }
}
