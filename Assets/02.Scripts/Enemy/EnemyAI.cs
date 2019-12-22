using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    public State state = State.PATROL;

    private Transform playerTr;
    private Transform enemyTr;

    public float attackDist = 5.0f;
    public float traceDist = 10.0f;

    public bool isDie = false;

    private WaitForSeconds ws;
    private MoveAgent moveAgent;
    private Animator animator;
    private EnemyFire enemyFire;
    private EnemyFOV enemyFOV;

    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDieIndex = Animator.StringToHash("DieIndex");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");

    private void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("PLAYER");

        if (player != null)
        {
            playerTr = player.GetComponent<Transform>();
        }

        enemyTr = GetComponent<Transform>();

        moveAgent = GetComponent<MoveAgent>();
        animator = GetComponent<Animator>();
        enemyFire = GetComponent<EnemyFire>();
        enemyFOV = GetComponent<EnemyFOV>();

        ws = new WaitForSeconds(0.3f);
        //Cycle Offset 값을 불규칙하게 변경
        animator.SetFloat(hashOffset, Random.Range(0.0f, 1.0f));
        //Speed 값을 불규칙하게 변경
        animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 1.2f));

    }

    IEnumerator CheckState()
    {
        while(!isDie)
        {
            if (state == State.DIE) yield break;

            float dist = Vector3.Distance(playerTr.position, enemyTr.position);

            if (dist <= attackDist)
            {
                if (enemyFOV.IsViewPlayer())
                {
                    state = State.ATTACK;
                }
                else
                {
                    state = State.TRACE;
                }

            }
            else if (enemyFOV.IsTracePlayer())
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }

            yield return ws;
        }
    }

    IEnumerator Action()
    {
        while (!isDie)
        {
            yield return ws;

            switch (state)
            {
                case State.PATROL:
                    moveAgent.Patrolling = true;
                    enemyFire.isFire = false;
                    animator.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    moveAgent.TraceTarget = playerTr.position;
                    enemyFire.isFire = false;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    moveAgent.Stop();
                    enemyFire.isFire = true;
                    animator.SetBool(hashMove, false);
                    break;
                case State.DIE:
                    gameObject.tag = "Untagged";
                    isDie = true;
                    moveAgent.Stop();
                    enemyFire.isFire = false;
                    animator.SetInteger(hashDieIndex, Random.Range(0, 3));
                    animator.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
        }
    }

    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());

        Damage.OnPlayerDie += OnPlayerDie;
    }

    private void OnDisable()
    {
        Damage.OnPlayerDie -= OnPlayerDie;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat(hashSpeed, moveAgent.Speed);
    }

    public void OnPlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        StopAllCoroutines();

        animator.SetTrigger(hashPlayerDie);
    }

}
