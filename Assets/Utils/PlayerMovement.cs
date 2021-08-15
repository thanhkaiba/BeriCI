using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 15f;
	public Transform characters;
	public Transform rotateObject;

	Vector3 move;

	private void Start()
	{
		characters.transform.eulerAngles = new Vector3(
			0,
			180,
			0
		);
}
	void Update()
	{
		move.x = Input.GetAxis("Horizontal");
		move.z = Input.GetAxis("Vertical");
		//if (move.x > 0)
		//{
		//	this.transform.localScale = new Vector3(-1f, 1f, 1f);
		//}
		//else if (move.x < 0)
		//{
		//	this.transform.localScale = new Vector3(1f, 1f, 1f);
		//}
	}

	void FixedUpdate()
	{
		// transform.Translate(move * Time.fixedDeltaTime * speed);
		transform.position = transform.position + move * Time.fixedDeltaTime * speed;
	}
}