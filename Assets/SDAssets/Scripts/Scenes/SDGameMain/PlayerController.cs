/*
 * File Name: PlayerController.cs
 * Description: Takes care of player's movement, direction, and baseSpeed. 
 *
 */


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
        public float baseSpeed;
        public float maxSpeed;
        public float maxRotationSpeed;
        public float maxHealth;
        public float maxStamina;
        public float staminaDrainRate;
        public float minBoostAmount;
        public float speedBoostFactor;
        public float timeBetweenStaminaRecovery;

        private Rigidbody rb;
        private GameController gameController;

        private float currentStamina;
        private bool canBoost = true;
        private float currentSpeed;

        private static SD.GameManager sdGameManager;
        // Detects the player object, and reads the 'GameController' Object
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            //scale = rb.transform.localScale;
            currentSpeed = baseSpeed;

            sdGameManager = SD.GameManager.getInstance();
            gameController = GameController.getInstance();
            gameController.SetMaxHealth(maxHealth);
            gameController.SetMaxStamina(maxStamina);
            gameController.SetStaminaDelay(timeBetweenStaminaRecovery);
        }

        private void Update()
        {
            if (gameController.getIsGameTimeTicking())
            {
                // Mouse input section.
                if (Input.GetMouseButton(0))
                {
                    // Calculate the rotation and turn the player.
                    Vector3 mousePosition = Input.mousePosition;
                    var screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
                    Vector2 playerToMouseOffset = new Vector2(
                        mousePosition.x - screenPoint.x,
                        mousePosition.y - screenPoint.y);

                    var playerToMouseAngle = Mathf.Atan2(
                        playerToMouseOffset.y,
                        playerToMouseOffset.x) * Mathf.Rad2Deg;

                    float yAngle = 0.0f;
                    if (playerToMouseAngle >= -90 && playerToMouseAngle <= 90)
                    {
                        // invert the angle to avoid upside down movement.
                        playerToMouseAngle = 180 - playerToMouseAngle;
                        yAngle = -180.0f;
                    }
                    transform.rotation = Quaternion.Euler(transform.rotation.x - 180, yAngle, -playerToMouseAngle);

                    // Finally, update the game controller with new positional data.
                    mousePosition.z = transform.position.z - Camera.main.transform.position.z;
                    mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                    transform.position = Vector3.MoveTowards(transform.position, mousePosition, currentSpeed * Time.fixedDeltaTime);

                    // Clamp the player's position to within the playable area.
                    rb.position = new Vector3(Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
                                              Mathf.Clamp(rb.position.y, boundary.yMin, boundary.yMax),
                                              0.0f);
                }

                // Give the player a speed boost if they hold space and have the stamina for it.
                currentStamina = gameController.GetStamina();
                if (!canBoost && currentStamina > minBoostAmount)
                {
                    canBoost = true;
                }

                if (Input.GetKey(KeyCode.Space) && currentStamina > 0.0f && canBoost)
                {
                    float newStaminaAmount = currentStamina - staminaDrainRate * Time.fixedDeltaTime;
                    if (newStaminaAmount <= 0.0)
                    {
                        canBoost = false;
                        currentStamina = 0.0f;
                    }
                    else
                    {
                        currentSpeed = baseSpeed * speedBoostFactor;
                        if (currentSpeed > maxSpeed)
                        {
                            currentSpeed = maxSpeed;
                        }
                        gameController.SetStamina(newStaminaAmount);
                    }
                }
                else
                {
                    currentSpeed = baseSpeed;
                }
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

