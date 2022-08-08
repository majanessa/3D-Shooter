using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private PhotonView _photonView;
    public float speed = 5f, rotateSpeed = 4f, range = 100f, power = 10f, radius = 5f;
    public int _attackDamage = 25;
    private int _health = 100;
    public ParticleSystem shootEffect, explosionEffect;
    public AudioSource runningAudio, shootAudio;
    
    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody>();
        
        if(!_photonView.IsMine)
            Destroy(GetComponentInChildren<Camera>().gameObject);
    }

    private void FixedUpdate()
    {
        if (!_photonView.IsMine)
            return;

        MovePlayer();
    }

    private void Update()
    {
        if (!_photonView.IsMine)
            return;

        RotatePlayer();

        Shoot();
    }

    private void Shoot()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            shootEffect.Play();
            shootAudio.Play();
            Camera cam = transform.GetChild(0)?.GetComponent<Camera>();
            RaycastHit hit;
            Vector3 shootPos = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
            if (Physics.Raycast(shootPos, cam.transform.forward, out hit, range))
            {
                Debug.Log("Объект: " + hit.transform.gameObject.name);
                if (hit.collider.CompareTag("Player")) {
                     hit.collider.GetComponent<PlayerController>().Damage(_attackDamage);
                     hit.collider.GetComponent<Rigidbody>().AddForce(new Vector3(0, 100f, 0));
                }

                if (hit.rigidbody != null) {
                    Vector3 pos = hit.collider.GetComponent<Transform>().position;
                    ParticleSystem explode = Instantiate(explosionEffect, pos, Quaternion.identity);

                    Destroy(explode.transform.gameObject, 0.3f);
                }
            }
        }
    }

    public void Damage(int damage)
    {
        _photonView.RPC("PunDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void PunDamage(int damage)
    {
        if (!_photonView.IsMine)
            return;
        
        _health -= damage;
        if(_health <= 0)
            PhotonNetwork.Destroy(gameObject);
    }

    private void RotatePlayer()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Input.GetAxis("Horizontal"));
    }

    private void MovePlayer()
    {
        float vertMove = Input.GetAxis("Vertical");
        _rb.MovePosition(transform.position + (transform.forward * Time.fixedDeltaTime * speed * vertMove));
        if ((vertMove > 0 || vertMove < 0) && !runningAudio.isPlaying)
            runningAudio.Play();
        else if (vertMove == 0 && runningAudio.isPlaying)
            runningAudio.Stop();
    }
}
