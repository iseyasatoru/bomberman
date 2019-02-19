using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject explosionPrefab;
    public LayerMask levelMask;
    private bool exploded = false;

    void Start()
    {
        Invoke("Explode", 3f);

        
    }

   
    void Explode()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        GetComponent<MeshRenderer>().enabled = false;
        exploded = true;
        StartCoroutine(CreateExplosions(Vector3.forward));
        StartCoroutine(CreateExplosions(Vector3.left));
        StartCoroutine(CreateExplosions(Vector3.right));
        StartCoroutine(CreateExplosions(Vector3.back));



        transform.Find("Collider").gameObject.SetActive(false);
        Destroy(gameObject, 0.3f);

    }
    private IEnumerator CreateExplosions(Vector3 direction)
    {
        for (int i = 1; i < 3; i++) 
        {
            RaycastHit hit;
            Physics.Raycast(
                transform.position + new Vector3(0,0.5f,0),direction,out hit,i,levelMask
            );
            if (!hit.collider)
            {
                Instantiate(explosionPrefab, transform.position + (i * direction),explosionPrefab.transform.rotation);

            }
            else
            {
                break;

            }
            yield return new WaitForSeconds(0.05f);

        }
        
    }
    public void OnTriggerEnter(Collider other)
    {
        if(!exploded && other.CompareTag("Explosion"))
        {
            CancelInvoke("Explode");

            Explode();
        }

    }

}
