using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firing : MonoBehaviour
{
    public MainCamera MainCamera;
    public float RateOfFire;
    public float GunReloadSpeed = 5.0f;
    public int FireButton;

    public float CameraShakeDuration;
    public float CameraShakeAmount;

    public GameObject BulletObject;
    public Transform BulletSpawnPoint;

    private bool fire;
    private float elapsed = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(FireButton) && !fire)
        {
            fire = true;
            elapsed = 0.0f;

            StartCoroutine(MainCamera.CameraShakeComponent.Shake(CameraShakeDuration, CameraShakeAmount));
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, -0.25f);

            GameObject Bullet = Instantiate(BulletObject, BulletSpawnPoint.position, BulletObject.transform.rotation);
            Bullet.transform.forward = this.transform.forward;
        }

        if(fire)
        {
            if (elapsed >= RateOfFire)
                fire = false;

            elapsed += Time.deltaTime;
        }

        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0.25f), Time.deltaTime * GunReloadSpeed);
    }
}
