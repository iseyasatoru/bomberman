﻿/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System;
using UniRx;


public class Player : MonoBehaviour
{
  [Range (1, 2)] 
    public int playerNumber = 1;

    public float moveSpeed = 5f;
    public bool canDropBombs = true;
   
    public bool canMove = true;
     public int shotCount = 3;

    private int bombs = 2;
    public bool dead = false;
    public GlobalStateManager globalmanager;
    public GameObject bombPrefab;

    //Cached components
    private Rigidbody rigidBody;
    private Transform myTransform;
    private Animator animator;

    // Use this for initialization
    void Start ()
    {
        //Cache the attached components for better performance and less typing
        rigidBody = GetComponent<Rigidbody> ();
        myTransform = transform;
        animator = myTransform.Find ("PlayerModel").GetComponent<Animator> ();


    }

    

    // Update is called once per frame
    void Update ()
    {
        UpdateMovement ();

    }
    public void modoru()
    {
        shotCount = 3;

    }


    private void UpdateMovement ()
    {
        animator.SetBool ("Walking", false);

        if (!canMove)
        { //Return if player can't move
            return;
        }

        //Depending on the player number, use different input for moving
        if (playerNumber == 1)
        {
            UpdatePlayer1Movement ();
        } else
        {
            UpdatePlayer2Movement ();
        }
    }

    /// <summary>
    /// Updates Player 1's movement and facing rotation using the WASD keys and drops bombs using Space
    /// </summary>
    private void UpdatePlayer1Movement ()
    {
        if (Input.GetKey (KeyCode.W))
        { //Up movement
            rigidBody.velocity = new Vector3 (rigidBody.velocity.x, rigidBody.velocity.y, moveSpeed);
            myTransform.rotation = Quaternion.Euler (0, 0, 0);
            animator.SetBool ("Walking", true);
        }

        if (Input.GetKey (KeyCode.A))
        { //Left movement
            rigidBody.velocity = new Vector3 (-moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            myTransform.rotation = Quaternion.Euler (0, 270, 0);
            animator.SetBool ("Walking", true);
        }

        if (Input.GetKey (KeyCode.S))
        { //Down movement
            rigidBody.velocity = new Vector3 (rigidBody.velocity.x, rigidBody.velocity.y, -moveSpeed);
            myTransform.rotation = Quaternion.Euler (0, 180, 0);
            animator.SetBool ("Walking", true);
        }

        if (Input.GetKey (KeyCode.D))
        { //Right movement
            rigidBody.velocity = new Vector3 (moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            myTransform.rotation = Quaternion.Euler (0, 90, 0);
            animator.SetBool ("Walking", true);
        }

        if (canDropBombs && Input.GetKeyDown (KeyCode.Space))
        { //Drop bomb
            if (shotCount < 1)
            {
                Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(_ => modoru());
                return;

                shotCount -= 1;
                DropBomb();
            }
        }
    }

    /// <summary>
    /// Updates Player 2's movement and facing rotation using the arrow keys and drops bombs using Enter or Return
    /// </summary>
    private void UpdatePlayer2Movement ()
    {
        if (Input.GetKey (KeyCode.UpArrow))
        { //Up movement
            rigidBody.velocity = new Vector3 (rigidBody.velocity.x, rigidBody.velocity.y, moveSpeed);
            myTransform.rotation = Quaternion.Euler (0, 0, 0);
            animator.SetBool ("Walking", true);
        }

        if (Input.GetKey (KeyCode.LeftArrow))
        { //Left movement
            rigidBody.velocity = new Vector3 (-moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            myTransform.rotation = Quaternion.Euler (0, 270, 0);
            animator.SetBool ("Walking", true);
        }

        if (Input.GetKey (KeyCode.DownArrow))
        { //Down movement
            rigidBody.velocity = new Vector3 (rigidBody.velocity.x, rigidBody.velocity.y, -moveSpeed);
            myTransform.rotation = Quaternion.Euler (0, 180, 0);
            animator.SetBool ("Walking", true);
        }

        if (Input.GetKey (KeyCode.RightArrow))
        { //Right movement
            rigidBody.velocity = new Vector3 (moveSpeed, rigidBody.velocity.y, rigidBody.velocity.z);
            myTransform.rotation = Quaternion.Euler (0, 90, 0);
            animator.SetBool ("Walking", true);
        }

        if (canDropBombs && (Input.GetKeyDown (KeyCode.KeypadEnter) || Input.GetKeyDown (KeyCode.Return)))
        {

            if (shotCount < 1)
            {
                Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(_ => modoru());
                return;
            }

            shotCount -= 1;
             DropBomb ();


            
        }
    }

    /// <summary>
    /// Drops a bomb beneath the player
    /// </summary>
    private void DropBomb ()
    {
        if (bombPrefab)

        { //Check if bomb prefab is assigned first
            var pos = new Vector3(
            Mathf.RoundToInt(myTransform.position.x),
            bombPrefab.transform.position.y,
            Mathf.RoundToInt(myTransform.position.z)
            );


            Instantiate(bombPrefab, pos, bombPrefab.transform.rotation);
        }
    }

    public void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag ("Explosion"))
        {
            Debug.Log ("P" + playerNumber + " hit by explosion!");
            dead = true;
            globalmanager.PlayerDied(playerNumber);
            this.gameObject.SetActive(false);


        }

    }

}
