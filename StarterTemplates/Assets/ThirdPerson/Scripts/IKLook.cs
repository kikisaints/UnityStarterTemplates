using UnityEngine;

public class IKLook : MonoBehaviour {

    private Animator anim;
    public Transform target;

    private float weight;
    public float maxDistance = 5;

	private void Start ()
    {
        anim = GetComponent<Animator>();
	}
	
	public void SetTarget (Transform t)
    {
        target = t;
	}

    private void OnAnimatorIK(int layerIndex)
    {
        if (target)
        {
            if (Vector3.Distance(target.position, transform.position) < maxDistance)
            {
                anim.SetLookAtPosition(target.position + new Vector3(0, 1.5f, 0));
                weight = Mathf.Lerp(weight, 1, 5 * Time.deltaTime);
            }
            else weight = Mathf.Lerp(weight, 0, 3 * Time.deltaTime);
            anim.SetLookAtWeight(weight, 0.25f, 0.75f, 1, 1);
        }
    }

}
