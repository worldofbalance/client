using UnityEngine;
using System.Collections;

namespace RR
{
    [RequireComponent(typeof(PlayerPhysics))]

    public class PlayerController : MonoBehaviour
    {
        // Player Handling
        public float gravity;
        // Cannot assign speed with other value here. Don't know why.
        public float speed;//Running.BASE_SPEED; // <- Doesn't work.
        public float acceleration;
        public float jumpHeight = 120;

        private Animator anim;
        private float currentSpeed;
        private float targetSpeed;
        private Vector2 amountToMove;

        private bool wallJumped;

        private PlayerPhysics playerPhysics;




        private float barDisplay = 0.0f;
        private Vector2 pos = new Vector2(620,20);
        private Vector2 size = new Vector2(300,30);
        public Texture2D progressBarEmpty;
        public Texture2D progressBarFull;




        void Start()
        {
            speed = Running.BASE_SPEED;
            // speed = 150;
			gravity = 175;
            acceleration = 64;
            DebugConsole.Log("!!!!!!!!!"+speed.ToString());
            DebugConsole.Log("!!!!!!!!!"+acceleration.ToString());
			DebugConsole.Log(gravity.ToString());
            playerPhysics = GetComponent<PlayerPhysics>();
            anim = GetComponent<Animator>();
        }

        void OnGUI ()
        {
            GUIStyle myStyle = new GUIStyle ();
            myStyle.normal.textColor = Color.black;
            myStyle.fontSize = 20;
            myStyle.alignment = TextAnchor.UpperCenter;


            GUI.Label (new Rect (520, 25, 100, 20),
                "Speed:",
                myStyle);

            GUI.BeginGroup (new Rect (pos.x, pos.y, size.x, size.y));
            GUI.Box (new Rect (0, 0, size.x, size.y),"");

            // draw the filled-in part:
            GUI.BeginGroup (new Rect (0, 0, size.x * barDisplay, size.y));
            GUI.Box (new Rect (0,0, size.x, size.y),"");
            GUI.EndGroup ();

            GUI.EndGroup ();
            
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

            barDisplay =  speed / Running.MAX_SPEED;

            //Animation code
            anim.SetFloat("speed", currentSpeed);

            // Check if on ground then jump
            if (playerPhysics.grounded)
            {
                amountToMove.y = 0;
                anim.SetBool("jumping", false);

                // Jump
                if (Input.GetButtonDown("Jump"))
                {
                    amountToMove.y = jumpHeight;
                }
            }
            else if (!playerPhysics.grounded)
            {
                anim.SetBool("jumping", true);
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

        	if (currentSpeed < 0)
        	{
        		targetSpeed *= -.6f;
        		currentSpeed *= -.6f;
        	}

        	anim.SetInteger("facing", 0);
        	return 1;

        // Space.. Jump logic, tell the server etc
        //if (Input.GetKeyDown(KeyCode.Space))
        }
    }
}