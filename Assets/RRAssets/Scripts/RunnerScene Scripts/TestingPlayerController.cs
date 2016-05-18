using UnityEngine;
using System.Collections;

namespace RR {
	[RequireComponent(typeof(TestingPlayerPhysics))]
	public class TestingPlayerController : MonoBehaviour
	{
	
		// Player Handling
		public float gravity = 200;
		public float speed = 12;
		public float acceleration = 32;
		public float jumpHeight = 12;
		
		private Animator anim;
		private float currentSpeed;
		private float targetSpeed;
		private Vector2 amountToMove;

		private bool wallJumped;

		private TestingPlayerPhysics playerPhysics;


		void Start()
		{
			playerPhysics = GetComponent<TestingPlayerPhysics>();
			anim = GetComponent<Animator>();
		}

		void Update()
		{
			int direction;
			direction = doKeyboardInput();

			wallJumped = true;

			//Reseting speeds if collide with wall
			if (playerPhysics.movementStopped)
			{
				targetSpeed = 0;
				currentSpeed = 0;

				if (!playerPhysics.grounded)
					wallJumped = false;
			}

			targetSpeed = direction * speed;
			currentSpeed = IncrementTowards(currentSpeed, targetSpeed, acceleration);

			//Animation code
			anim.SetFloat ("speed", currentSpeed);

			// Check if on ground then jump
			if (playerPhysics.grounded) {
				amountToMove.y = 0;
				anim.SetBool ("jumping", false);	

				// Jump
				if (Input.GetButtonDown ("Jump")) {
					amountToMove.y = jumpHeight;
				}
			} else if(!playerPhysics.grounded){
				anim.SetBool ("jumping", true);			
			}

			// Wall jump
			if (!playerPhysics.grounded && playerPhysics.movementStopped && targetSpeed != 0)
			{
				if (Input.GetButtonDown("Jump"))
				{
					amountToMove.y = jumpHeight;
					currentSpeed = IncrementTowards(10 * ((targetSpeed / Mathf.Abs(targetSpeed) * -1)), targetSpeed * -1, 100);
				}
			}

			amountToMove.x = currentSpeed;
			amountToMove.y -= (gravity * Time.deltaTime);
			playerPhysics.Move(amountToMove * Time.deltaTime);
		}

		// Increase n towards target by speed
		private float IncrementTowards(float n, float target, float a)
		{
			if (n == target)
			{
				return n;
			}
			else
			{
				float dir = Mathf.Sign(target - n); // must n be increased or decreased to get closer to target
				n += a * Time.deltaTime * dir;
				return (dir == Mathf.Sign(target - n)) ? n : target; // if n has now passed target then return target, otherwise return n
			}
		}

		private int doKeyboardInput()
		{
			foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
			{
				if (Input.GetKey(vKey))
				{
					switch (vKey)
					{
						case KeyCode.LeftArrow:
							if(currentSpeed > 0)
							{
								targetSpeed *= -.6f;
								currentSpeed *= -.6f;
							}
							anim.SetInteger("facing", 1);
							return -1;
					
						case KeyCode.RightArrow:
							if(currentSpeed < 0)
							{
								targetSpeed *= -.6f;
								currentSpeed *= -.6f;
							}
							anim.SetInteger ("facing", 0);
							return 1;

						case KeyCode.UpArrow:
							//GameObject test = GameObject.Find("AfricanWildDog(Clone)");
							//TestingPlayerController temp = test.GetComponent<TestingPlayerController>();
							//temp.acceleration = 1000;
							//temp.speed = 500;
							return 0;
							break;

						case KeyCode.DownArrow:   
							//GameObject test2 = GameObject.Find("AfricanWildDog(Clone)");
							//TestingPlayerController temp2 = test2.GetComponent<TestingPlayerController>();
							//temp2.acceleration = 32;
							//temp2.speed = 8;
							targetSpeed = 0;
							currentSpeed = 0;
							return 0;
							break;
					}
				}
			}
			return 0;
			// Space.. Jump logic, tell the server etc
			//if (Input.GetKeyDown(KeyCode.Space))
		}
	}
}