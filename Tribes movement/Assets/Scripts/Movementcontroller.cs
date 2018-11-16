using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]

public class Movementcontroller : MonoBehaviour {

    //input variables
    private float x;
    private float z;
    private float mouseX;
    private float mouseY;

    private CharacterController controller;

    //settable multipliers
    [SerializeField]
    private float mouseSpeed;

    [SerializeField]
    private float moveSpeed;

    void Start ()
    {
        controller = GetComponent<CharacterController>();
	}
	
	
	void Update ()
    {
        updateMovementAxies();


	}


    private void updateMovementAxies()
    {
        x = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        z = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        mouseX = Input.GetAxis("MouseX") * Time.deltaTime * mouseSpeed;
        mouseY = Input.GetAxis("MouseY") * Time.deltaTime * mouseSpeed;
    }
}
