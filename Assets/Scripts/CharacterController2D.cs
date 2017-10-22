using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour {

	public GameObject character;
	Rigidbody2D characterBody;

	// Use this for initialization
	void Start () {
		characterBody = character.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.D)) {
			moveRight ();
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			moveLeft ();
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			moveJump ();
		}
	}

	public void moveRight(){
		character.GetComponent<SpriteRenderer> ().flipX = false;
		characterBody.AddForce(new Vector2(1, 0), ForceMode2D.Impulse);
	}
	public void moveLeft(){
		character.GetComponent<SpriteRenderer> ().flipX = true;
		characterBody.AddForce(new Vector2(-1, 0), ForceMode2D.Impulse);
	}
	public void moveJump(){
		characterBody.AddForce(new Vector2(0, 1), ForceMode2D.Impulse);
	}

	public void reset ()
	{
		character.transform.position = new Vector3 (0, 1.769f, 0);
	}
}
