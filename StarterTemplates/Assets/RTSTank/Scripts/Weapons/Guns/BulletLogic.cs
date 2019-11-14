using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLogic : MonoBehaviour
{
    public float BulletSpeed;
    public Rigidbody BulletRigidbody;

    void Update()
    {
        BulletRigidbody.velocity = this.transform.forward * (BulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);
    }
}
