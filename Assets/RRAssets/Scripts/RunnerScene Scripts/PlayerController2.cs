using UnityEngine;
using System.Collections;

namespace RR {
	[RequireComponent(typeof(PlayerPhysics))]
	public class PlayerController2 : MonoBehaviour {
		
		// Player Handling
		public float gravity = 20;
		public float speed = Running.BASE_SPEED;
		public float acceleration = 64;
		public float jumpHeight = 12;
		
		private float currentSpeed;
		private float targetSpeed;
		private Vector2 amountToMove;
	
		private bool wallJumped;
	
    private Animator anim;
		private PlayerPhysics playerPhysics;
		public  ArrayList allkeys;
		public int keytype, key;
		public int direction;
		public bool jump;
	
		
		void Start () {
			speed = Running.BASE_SPEED;
			playerPhysics = GetComponent<PlayerPhysics>();
	        anim = GetComponent<Animator>();
			keytype = 0;
			key = 0;
			jump = false;
		}
		
		void Update () {
			wallJumped = true;
			if (keytype == 1) {
				direction = key;
			}
			if (keytype == 2) {
				if(key > 0)
				{
					jump = true;
				} else
				{
					jump = false;
				}
	
			}
	
      //Updating animation variables
      updateAnimation();
		
			//Reseting speeds if collide with wall
			if (playerPhysics.movementStopped) {
				targetSpeed = 0;
				currentSpeed = 0;
	
				if(!playerPhysics.grounded)
					wallJumped = false;
	
			}	
	
			targetSpeed = direction * speed;
			currentSpeed = IncrementTowards(currentSpeed, targetSpeed, acceleration);
	
      //Animation code
      anim.SetFloat("speed", currentSpeed);
	
			// Check if on ground then jump
			if (playerPhysics.grounded) {
				amountToMove.y = 0;
				
				// Jump
				if (jump) {
					amountToMove.y = jumpHeight;	
	
				}
			}
	
			// Wall jump
			if (!playerPhysics.grounded && playerPhysics.movementStopped && targetSpeed != 0) {
	
				if(jump) {
					amountToMove.y = jumpHeight;
					currentSpeed = IncrementTowards ( 10 * ((targetSpeed/Mathf.Abs(targetSpeed)*-1)) , targetSpeed * -1, 100);
				} 
			}
			jump = false;
			amountToMove.x = currentSpeed;
			amountToMove.y -= (gravity * Time.deltaTime);
	
			playerPhysics.Move(amountToMove * Time.deltaTime);
		}
	
	    private void updateAnimation()
	    {
        if (keytype == 1)
        {
          if (key == 1)
          {
              anim.SetInteger("facing", 0);
          }
          else if (key == -1)
          {
              anim.SetInteger("facing", 1);
          }
          else
          {
              //Look into what needs to be here
          }
        }
        else if (keytype == 2)
        {
          if (key == 1)
          {
              anim.SetBool("jumping", true);
          }
          else
          {
              anim.SetBool("jumping", false);
          }
        }
	    }
		
		// Increase n towards target by speed
		private float IncrementTowards(float n, float target, float a) {
			if (n == target) {
				return n;	
			}
			else {
				float dir = Mathf.Sign(target - n); // must n be increased or decreased to get closer to target
				n += a * Time.deltaTime * dir;
				return (dir == Mathf.Sign(target-n))? n: target; // if n has now passed target then return target, otherwise return n
			}
		}
	}
}