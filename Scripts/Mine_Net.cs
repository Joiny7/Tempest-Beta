using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Mine_Net : NetworkBehaviour
{
    [SyncVar]
    public int Damage = 50;
    public short Owner;
    public GameObject Explosion;
    private PlayerHealth_Net _hitPlayer;
    public bool _armed = false;
    public int _mineHealth = 100;
    public float LifeTime = 50f;
    private float _age;
    private GameObject _owner;

    private void Start ()
    {
        StartCoroutine(ArmMine());
    }

    [ServerCallback]
    void Update ()
    {
        if (_mineHealth <= 0)
        {
            Explode();
            //Destroy(this.gameObject);
            NetworkServer.Destroy(gameObject);
        }
        _age += Time.deltaTime;

        if (_age >= LifeTime)
            NetworkServer.Destroy(gameObject);
    }

    
    public void setOwner(GameObject g)
    {
        _owner = g;
    }

    [ServerCallback]
    private void Explode()
    {
        GameObject explosion = (GameObject)Instantiate(Explosion, transform.position, Quaternion.identity);
        NetworkServer.Spawn(explosion);
        Destroy(explosion, 3f);
    }

    public IEnumerator ArmMine()
    {
        yield return new WaitForSeconds(2f);
        _armed = true;
        gameObject.tag = "Mine";
        //make it glow
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isServer)
            return;

        if (other.CompareTag("Player") && _armed)
        {
            var player = other.GetComponent<PlayerHealth_Net>();
            player.TakeDamage(Damage, _owner, "[Mine]");
            Explode();
            //other.GetComponent<AudioScript>().PlayOneShotList(0, other.GetComponent<AudioSource>(), 1);
            NetworkServer.Destroy(gameObject);
        }
        if (other.gameObject.CompareTag("Missile"))
        {
            _mineHealth -= 25;
        }

        if (other.gameObject.CompareTag("Bullet"))
        {
            _mineHealth -= 10;
        }

        if (other.gameObject.CompareTag("HeatMissile"))
        {
            _mineHealth -= 50;
        }

        if (other.gameObject.CompareTag("ScatterLine"))
        {
            _mineHealth -= 10;
        }
    }
}
