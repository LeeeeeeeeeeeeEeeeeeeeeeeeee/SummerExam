using SUPERCharacter;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class Enemy : MonoBehaviour
{
    public static Enemy Instance;

    [SerializeField] public GameObject Player;
    private RigBuilder RigBuilder;
    public Animator Animator;
    public NavMeshAgent NavMeshAgent;
    public List<Transform> patroll;
    public EnemySight Sight;
    public Transform Searchpoint;

    [SerializeField] private Transform PatrolRoute;
    private Vector3 destination;
    public int patrollIndex;

    bool FirstHitted = false;

    


    enum StatEnum
    {
        None,
        Searching,
        Chase,
        Fire
    }

    private StatEnum State = StatEnum.None;

    void Start()
    {
        Instance = this;
        Sight.OnRayHitted += RayHitted;
        RigBuilder = GetComponent<RigBuilder>();
        Animator= GetComponent<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();

        patroll = PatrolRoute.GetComponentsInChildren<Transform>()
        .Where(t => t != PatrolRoute)
        .ToList();

        StartPatrol();
    }

    public bool IsActivateHeadAimToPlayer { set => RigBuilder.layers[0].active = value; }
    public bool IsActivateBodyRotateToPlayer { set => RigBuilder.layers[1].active = value; }

    public Vector3 Destination { get => destination; set { destination = value; if (NavMeshAgent.enabled == true) NavMeshAgent.destination = value; } }
    bool Shootable { get => Sight.Isee; }

    
    void Update()
    {
        if (Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Rifle Idle") 
        {
            NavMeshAgent.speed = 0;
        }else
        {
            NavMeshAgent.speed = 2;
        }
    }

    public void StartPatrol()
    {
        ChangeSiteRange("Idle");
        IsActivateHeadAimToPlayer = false;
        Animator.SetTrigger("Walk");
        patroll[FindnearPatroll(patroll)].gameObject.SetActive(true);
        Destination = patroll[FindnearPatroll(patroll)].position;
    }

    public int FindnearPatroll(List<Transform> patroll)
    {
        float nearDis = 10000.0f;

        for (int i = 0; i < patroll.Count; i++)
        {
            float distance = Vector3.Distance(patroll[i].position, transform.position);
            if (distance < nearDis)
            {
                patrollIndex = i;
                nearDis = distance;
            }
        }

        return patrollIndex;
    }

    public void RayHitted()
    {
        if (!FirstHitted)
        {
            FirstHitted = true;
            Debug.Log("Hitted");
            switch (State)
            {
                case StatEnum.None:
                    SearchingMode();
                    State = StatEnum.Searching;
                    break;

                case StatEnum.Searching:
                    ChaseMode();
                    State = StatEnum.Chase;
                    break;

                case StatEnum.Chase:
                    ShootMode();
                    State = StatEnum.Fire;
                    break;


                default:
                    break;
            }
        }

    }


    public void SearchingMode()
    {
        NavMeshAgent.speed = 1;
        Animator.SetTrigger("Searching");
        ChangeSiteRange("Searching");

        Destination = Player.transform.position;
        Searchpoint.position = Destination;

        StartCoroutine(DelaySight());
    }

    public void ChaseMode()
    {
        IsActivateHeadAimToPlayer = true;
        NavMeshAgent.speed = 5;
        Animator.SetTrigger("Find");
        StartCoroutine(ChaseStart());

        StartCoroutine(DelaySight());
    }

    public void ShootMode()
    {
        StartCoroutine(AbleShoot());
    }
    private IEnumerator AbleShoot()
    {
        while(gameObject)
        {
            if(Vector3.Distance(transform.position, Player.transform.position) <= 5 && Sight.Isee)
            {
                IsActivateBodyRotateToPlayer= true;
                Player.transform.GetChild(0).transform.LookAt(transform.position);
                Animator.SetTrigger("Fire");
                NavMeshAgent.enabled= false;
                Player.GetComponent<SUPERCharacterAIO>().enabled=false;
                Time.timeScale = 0; 

                break;
            }
            yield return null;
        }
    }


    private IEnumerator ChaseStart()
    {
        while (gameObject)
        {
            Destination = Player.transform.position;
            yield return null;
        }
    }

    private IEnumerator DelaySight()
    {
        Debug.Log("Wait");
        yield return new WaitForSeconds(1f);
        Debug.Log("WaitEnd");
        FirstHitted = false;
    }




    private void ChangeSiteRange(string i)
    {
        if (i == "Idle")
        {
            Sight.transform.localScale = new Vector3(4,3,1);
        }
        else if (i == "Searching")
        {
            Sight.transform.localScale = new Vector3(6, 6, 1.5f);
        }
    }

}
