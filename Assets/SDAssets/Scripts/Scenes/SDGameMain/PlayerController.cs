/*
 * File Name: PlayerController.cs
 * Description: Takes care of player's movement, direction, and baseSpeed. 
 *
 */


using System;
using UnityEngine;


namespace SD
{
    // Takes care of the size of boundary of the entire screen
    [System.Serializable]
    public class Boundary
    {
        public float xMin, xMax, yMin, yMax, zMin, zMax;
    }

    public class PlayerController : MonoBehaviour
    {
        public Boundary boundary;

        public float forwardAcceleration;
        public float baseMaxSpeed;
        public float absoluteMaxSpeedLimit;

        public float maxRotationSpeed;
        public float maxRotationRadiansPerSecond;
        public float minimumSpeedToTurningRatio;

        public float maxHealth;
        public float maxStamina;
        public float staminaDrainRate;

        public float minStaminaForBoost;
        public float staminaSpeedBoostFactor;
        public float timeBetweenStaminaRecovery;

        private Rigidbody rb;
        private GameObject playerModel;
        private Vector3 playerToMouseVector;
        private Vector3 mousePosition;
        private bool facingRight = true;

        private GameController gameController;

        private float currentStamina;
        private bool canBoost = true;
        private float currentSpeedLimit;

        private static SD.GameManager sdGameManager;
        // Detects the player object, and reads the 'GameController' Object
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            currentSpeedLimit = baseMaxSpeed;
            rb.maxAngularVelocity = maxRotationRadiansPerSecond;

            sdGameManager = SD.GameManager.getInstance();
            gameController = GameController.getInstance();
            gameController.SetMaxHealth(maxHealth);
            gameController.SetMaxStamina(maxStamina);
            gameController.SetStaminaDelay(timeBetweenStaminaRecovery);

            playerModel = transform.Find("Model").gameObject;
        }

        private void Update()
        {
            if (gameController.getIsGameTimeTicking())
            {
                // Update stamina amount.
                currentStamina = gameController.GetStamina();

                // Check to see if stamina boosting is possible.
                if (!canBoost && currentStamina > minStaminaForBoost)
                {
                    canBoost = true;
                }
                
                // Give the player a speed boost via max speed increase if they hold space and have the stamina for it.
                if (Input.GetKey(KeyCode.Space) && canBoost && currentStamina > 0.0f)
                {
                    float newStaminaAmount = currentStamina - staminaDrainRate * Time.deltaTime;

                    if (newStaminaAmount <= 0.0)
                    {
                        canBoost = false;
                        currentStamina = 0.0f;
                    }
                    else
                    {
                        currentSpeedLimit = baseMaxSpeed * staminaSpeedBoostFactor;

                        if (currentSpeedLimit > absoluteMaxSpeedLimit)
                        {
                            currentSpeedLimit = absoluteMaxSpeedLimit;
                        }
                        gameController.SetStamina(newStaminaAmount);
                    }
                }
                else
                {
                    currentSpeedLimit = baseMaxSpeed;
                }

                // Handle player model rotations.
                HandleRotations();

                // Mouse input section.
                if (Input.GetMouseButton(0))
                {
                    // Calculate the rotation in angles between the player and the mouse pointer.
                    mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
                    mousePosition.z = 0;
                    // Now normalize the vector.
                    playerToMouseVector = (mousePosition - new Vector3(transform.position.x, transform.position.y, 0)).normalized;
                    float playerToMouseAngle = Mathf.Atan2(playerToMouseVector.y, playerToMouseVector.x) * Mathf.Rad2Deg;
                    // Use to clamp the rotation rate.
                    float currentToMaxSpeedRatio = Mathf.Clamp(rb.velocity.magnitude / currentSpeedLimit, minimumSpeedToTurningRatio, Mathf.Infinity);
                    
                    // Rotate if angle between mouse vector and player direction vector is significant.
                    if (Math.Abs(playerToMouseAngle) > .001)
                    {
                        // Clamp the maximum angle to limit slerp amount.
                        Mathf.Clamp(playerToMouseAngle, -Math.Abs(Mathf.Rad2Deg * maxRotationRadiansPerSecond), Math.Abs(Mathf.Rad2Deg * maxRotationRadiansPerSecond));
                        // Convert the target rotation angle to a quaternion.
                        Quaternion targetQuatRotation = Quaternion.Euler(new Vector3(0f, 0f, playerToMouseAngle * currentToMaxSpeedRatio));
                        // Now apply the rotation slerp.
                        rb.rotation = Quaternion.Slerp(rb.rotation, targetQuatRotation, Time.deltaTime * maxRotationSpeed);
                    }
                    
                    // Then add force in the new direction if applicable, or set max speed otherwise.
                    if (rb.velocity.magnitude < currentSpeedLimit)
                        rb.AddForce(transform.rotation * Vector3.right * forwardAcceleration, ForceMode.Force);
                    else if(rb.velocity.magnitude >= currentSpeedLimit)
                        rb.velocity = transform.rotation * Vector3.right * currentSpeedLimit;
                    
                    // Clamp the player's position to within the playable area.
                    rb.position = new Vector3(Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
                                              Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax),
                                              0.0f);
                }
            }
        }

        private void HandleRotations()
        {
            // Invert player model if we rotate beyond a vertical angle.
            // Do this by flagging for a slerp.
            if (facingRight && (transform.eulerAngles.z > 90f && transform.eulerAngles.z < 270f))
            {
                facingRight = false;
            }
            else if (!facingRight && (transform.eulerAngles.z <= 90f || transform.eulerAngles.z >= 270f))
            {
                facingRight = true;
            }

            // Continued slerping if transition is not complete or facing direction has changed.
            if (!facingRight && (Mathf.Abs(transform.eulerAngles.x - 180f) > 0.01 || Mathf.Abs(transform.eulerAngles.y - 90f) > 0.01))
            {
                Quaternion targetQuatRotation = Quaternion.Euler(new Vector3(-180f, -90f, 0f));
                playerModel.transform.localRotation = Quaternion.Slerp(
                    playerModel.transform.localRotation,
                    targetQuatRotation,
                    Time.deltaTime * maxRotationSpeed * 2);
            }
            else if(facingRight && (Mathf.Abs(transform.eulerAngles.x) > 0.01 || Mathf.Abs(transform.eulerAngles.y + 90f) > 0.01))
            {
                Quaternion targetQuatRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                playerModel.transform.localRotation = Quaternion.Slerp(
                    playerModel.transform.localRotation,
                    targetQuatRotation,
                    Time.deltaTime * maxRotationSpeed * 2);
            }
        }

        // Returns true if the player is moving.
        // Otherwise returns false.
        bool isMoving()
        {
            if (rb.velocity.x != 0 || rb.velocity.y != 0)
            {
                return true;
            }
            return false;
        }
    }
}

