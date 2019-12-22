using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject expEffect;

    public Mesh[] meshes;
    private MeshFilter meshFilter;

    public Texture[] textures;
    private MeshRenderer meshRenderer;

    private int hitCount = 0;

    private Rigidbody rb;

    public float expRadius = 10.0f;

    private AudioSource audioSource;
    public AudioClip expSfx;

    private Shake shake;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = textures[Random.Range(0, textures.Length)];
        audioSource = GetComponent<AudioSource>();
        //shake = GameObject.Find("CameraRig").GetComponent<Shake>();
        StartCoroutine(GetShake());
    }

    IEnumerator GetShake()
    {
        while (!UnityEngine.SceneManagement.SceneManager.GetSceneByName("Play").isLoaded)
        {
            yield return null;
        }
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    void ExpBarrel()
    {
        //Instantiate(expEffect, transform.position, Quaternion.identity);
        //rb.mass = 1.0f;
        //rb.AddForce(Vector3.up * 1000.0f);

        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2.0f);

        IndirectDamage(transform.position);


        int idx = Random.Range(0, meshes.Length);
        meshFilter.sharedMesh = meshes[idx];

        audioSource.PlayOneShot(expSfx, 1.0f);

        StartCoroutine(shake.ShakeCamera(0.1f, 0.2f, 0.5f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET"))
        {
            if (++hitCount == 3)
            {
                ExpBarrel();
            }
        }
    }

    void IndirectDamage(Vector3 pos)
    {
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 8);

        foreach (var coll in colls)
        {
            var rbComp = coll.GetComponent<Rigidbody>();
            rbComp.mass = 1.0f;
            rbComp.AddExplosionForce(1200.0f, pos, expRadius, 1000.0f);
        }
    }

}
