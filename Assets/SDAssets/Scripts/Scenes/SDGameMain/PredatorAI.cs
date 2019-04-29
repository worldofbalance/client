using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SD
{
    public class PredatorAI : MonoBehaviour
    {
        public List<Transform> prey, opponent;
        public Transform SelectedTarget;
        private GameController gameController;

        void Start()
        {
            gameController = GameController.getInstance();
            SelectedTarget = null;
            prey = new List<Transform>();
            opponent = new List<Transform>();
            AddPreyToList();
        }

        public void AddPreyToList()
        {
            //adds the numerous NPC fish to a prey list
            GameObject[] ItemsInList = GameObject.FindGameObjectsWithTag("NpcFish");
            foreach (GameObject _prey in ItemsInList)
            {
                prey.Add(_prey.transform);
            }

            //adds the opponent to a separate prey list
            GameObject[] evasionBoost = GameObject.FindGameObjectsWithTag("Opponent");
            foreach (GameObject enemy in evasionBoost)
            {
                opponent.Add(enemy.transform);
            }
        }

        /*
        public void AddTarget(Transform newprey)
        {
            prey.Add(newprey);
        }
        */

        public void CalculateDistance()
        {
            if (gameController.getEvasionBoostStatus() == false)
            {
                prey.Sort(delegate (Transform t1, Transform t2)
                {
                    return Vector3.Distance(t1.transform.position, transform.position).CompareTo(Vector3.Distance(t2.transform.position, transform.position));
                });
            }
            else
            {
                opponent.Sort(delegate (Transform t1, Transform t2)
                {
                    return Vector3.Distance(t1.transform.position, transform.position).CompareTo(Vector3.Distance(t2.transform.position, transform.position));
                });
            }

        }

        public void MoveToPrey()
        {
            if (SelectedTarget == null)
            {
                CalculateDistance();
                if (gameController.getEvasionBoostStatus() == false)
                {
                    SelectedTarget = prey[0];
                }
                else
                {
                    SelectedTarget = opponent[0];
                }
            }
            else
            {
                CalculateDistance();
                if (gameController.getEvasionBoostStatus() == false)
                {
                    SelectedTarget = prey[0];
                }
                else
                {
                    SelectedTarget = opponent[0];
                }
            }
        }

        void FixedUpdate()
        {
            MoveToPrey();
            float dist = Vector3.Distance(SelectedTarget.transform.position, transform.position);

            transform.position = Vector3.MoveTowards(transform.position, SelectedTarget.position, 60 * Time.deltaTime);
        }
    }
}

