using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    private Vector3 startPos; //Starting position of our platform
    public Vector3 endPos; //How far off to move from the startPos
    private float time; //The current time of the transition
    public float moveSpeed = 0.25f; //Speed multiplier
    public Transform moveObject; //Object to move

    void Start ()
    {
        if (!moveObject) moveObject = transform; //Set object to this if unfilled
        startPos = moveObject.position; //Set to our objects world position
        StartCoroutine(Forward()); //Start the object going to endPos
	}
	
	private IEnumerator Forward() //From startPos to offset
    {
        while (time < 1) //if we haven't reached the end
        {
            time = Mathf.Clamp(time + moveSpeed * Time.deltaTime, 0, 1); //Add to time
            moveObject.position = Vector3.Lerp(startPos, startPos + endPos, time); //Change position
            yield return null; //Keep doing it until we are there
        }
        StartCoroutine(Backward()); //Once finished, go back to startPos
    }

    private IEnumerator Backward() //From offset to startPos, works the same as Forward()
    {
        while (time > 0) 
        {
            time = Mathf.Clamp(time - moveSpeed * Time.deltaTime, 0, 1);
            moveObject.position = Vector3.Lerp(startPos + endPos, startPos, 1 - time);
            yield return null;
        }
        StartCoroutine(Forward()); //Once finished, go back to endPos

    }

    private void OnTriggerEnter(Collider other)
    { other.transform.SetParent(transform); } //Set objects in our trigger to be children

    private void OnTriggerExit(Collider other)
    { other.transform.SetParent(null); } //Set objects that exit to have no parent
    
}
