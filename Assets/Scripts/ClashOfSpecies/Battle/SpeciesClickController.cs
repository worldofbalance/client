using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class SpeciesClickController : MonoBehaviour
{

    public float shootDistance = 10f;
    public float shootRate = .5f;
    public ClashBattleController clashBattleControllerScript;

    private Animator anim;
    private NavMeshAgent navMeshAgent;
    private Transform targetedEnemy;
    private GameObject allySelected;
    private Ray shootRay;
    private RaycastHit shootHit;
    private bool walking;
    private bool enemyClicked;
    private float nextFire;

    //    Color oldColor;

    // Use this for initialization
    void Awake()
    {
//        anim = GetComponent<Animator>();
//        navMeshAgent = GetComponent<NavMeshAgent>();
        allySelected = null;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetButtonDown("Fire1") && !clashBattleControllerScript.isSpawningSpecies())
        {
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.collider.CompareTag("Enemy") && allySelected != null)
                {
                    targetedEnemy = hit.transform;
                    enemyClicked = true;
                }
                else if (hit.collider.CompareTag("Ally"))
                {
//                    var colliders = Physics.OverlapSphere(hit.transform.position, 1 /*Radius*/);
//                    colliders[0].gameObject;
                    allySelected = hit.collider.gameObject;
                    navMeshAgent = allySelected.GetComponent<NavMeshAgent>();
                    anim = allySelected.GetComponent<Animator>();
//                    oldColor = allySelected.GetComponent<Renderer>().material.color;
//                    allySelected.GetComponent<Renderer>().material.color = Color.blue;
                }
                else
                {
                    walking = true;
                    enemyClicked = false;
                    if (allySelected)
                    {
//                        allySelected.GetComponent<Renderer>().material.color = oldColor;
                        allySelected = null;
                    }
                    if (navMeshAgent)
                    {
                        navMeshAgent.destination = hit.point;
                        navMeshAgent.Resume();
                    }
                }
            }
        }

        if (enemyClicked)
        {
            MoveAndShoot();
        }

        if (navMeshAgent && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (!navMeshAgent.hasPath || Mathf.Abs(navMeshAgent.velocity.sqrMagnitude) < float.Epsilon)
                walking = false;
        }
        else
        {
            walking = true;
        }
//
        if (anim)
            anim.SetBool("Walking", walking);
    }

    private void MoveAndShoot()
    {
        if (targetedEnemy == null)
            return;
        navMeshAgent.destination = targetedEnemy.position;
        if (navMeshAgent.remainingDistance >= shootDistance)
        {

            navMeshAgent.Resume();
            walking = true;
        }

        if (navMeshAgent.remainingDistance <= shootDistance)
        {

            transform.LookAt(targetedEnemy);
            Vector3 dirToShoot = targetedEnemy.transform.position - transform.position;
            if (Time.time > nextFire)
            {
                nextFire = Time.time + shootRate;
//                shootingScript.Shoot(dirToShoot);
            }
            navMeshAgent.Stop();
            walking = false;
        }
    }

}