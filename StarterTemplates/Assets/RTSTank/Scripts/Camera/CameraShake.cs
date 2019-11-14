using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float ShakeDamper = 1.5f;
    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0.0f;
        while(elapsed < duration)
        {
            float z = Random.Range(-1f, 1f) * magnitude;

            Vector3 ShakePosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z + z);
            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, ShakePosition, Time.deltaTime * ShakeDamper);

            elapsed += Time.deltaTime;

            yield return null;
        }
    }
}
