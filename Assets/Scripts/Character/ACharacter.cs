using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class ACharacter : MonoBehaviour
{
    protected Stat stat;
    protected Animator animator;
    protected NavMeshAgent agent;
    protected GameObject target;
    protected Vector3 targetDir;
    protected float attackAnimSpeed;    // 공격 애니메이션 속도
    protected bool canAttack;           // 공격 쿨타임
    
    public GameObject Target { get { return target; } }

    protected abstract IEnumerator Attack();
    public abstract bool Attacked(int damage);
    protected abstract void Die();

    protected void Rotate(Vector3 dir)
    {
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
    }
}
    
//     // 데미지 text
//     private DamageTextSystem damageTextSystem;

//     // -------------------------------------------------------------
//     // 이동
//     // -------------------------------------------------------------
//     public void SetDestination(Vector3 position, bool isBack = false)
//     {
//         destinationPos = position;
//         if (isBack)
//         {
//             if (state != State.Back)
//             {
//                 state = State.Back;
//             }
//         }
//         else
//         {
//             if (state != State.Move)
//             {
//                 state = State.Move;
//             }
//         }
//         StopCoroutine("Attack");    // 공격 중이면 캔슬
//     }

//     protected void Move()
//     {
//         // Update에서 항시 동작

//         if (destinationPos == null || (state != State.Move && state != State.Back))
//         {
//             // 목적지가 없거나 상태가 Move 혹은 Back이 아닌 경우
//             animator.SetBool("isMove", false);
//             return;
//         }
        
//         // Rotate
//         Vector3 look = new Vector3(destinationPos.x, 0f, destinationPos.z) 
//             - new Vector3(this.transform.position.x, 0f, this.transform.position.z);

//         if (look.sqrMagnitude > float.Epsilon)
//         {
//             this.transform.rotation = Quaternion.Lerp(
//                 this.transform.rotation,
//                 Quaternion.LookRotation(look),
//                 Time.deltaTime * rotationSpeed
//             );
//         }

//         // Move
//         destinationDir = destinationPos - this.transform.position;      // 목적지 방향 벡터
//         this.transform.position += destinationDir.normalized * Time.deltaTime * stat.MoveSpeed;

//         animator.SetFloat("MoveSpeed", stat.MoveSpeed * .2f);   // Animation speed = actual speed * 5
//         animator.SetBool("isMove", true);

//         if (destinationDir.sqrMagnitude <= 0.05f)
//         {
//             // 목적지에 도착하면
//             state = State.Idle;
//             destinationPos = this.transform.position;
//             MoveDone();
//             return;
//         }
//     }    

//     protected virtual void MoveDone()
//     {
//         // for player
//     }
//     // -------------------------------------------------------------
//     // 공격
//     // -------------------------------------------------------------
//     public void SetTarget(GameObject gameObject)
//     {
//         // 타겟 지정
//         target = gameObject;
//         if (gameObject == null)
//         {
//             StopCoroutine("Attack");
//         }
//     }

//     public void CheckTarget()
//     {
//         // 타겟이 attackRange 안에 있는지 확인
//         if (target == null)
//         {
//             TargetDone();
//             // 타겟이 있는 경우에만 확인
//             return;
//         }

//         if (state == State.Back)
//         {
//             // 돌아가는 상태면 타겟 정보 계산 안함
//             return;
//         }

//         // 타겟 방향 벡터 및 거리 계산
//         targetDir = target.transform.position - this.transform.position;
//         targetDir.y = 0f;

//         if (targetDir.sqrMagnitude < Mathf.Pow(stat.AttackRange, 2))
//         {
//             
//             else
//             {
//                 // 공격 사거리 안에 있는데도, 바라보고 있지 않는 경우
//                 SetDestination(target.transform.position);
//             }
//         }
//     }


//     protected virtual IEnumerator Attack(float actualAttackSpeed)
//     {
//         canAttack = false;
//         StartCoroutine("StartAttackCoolTime", actualAttackSpeed);       // 공격 쿨타임 계산
//         yield return new WaitForSeconds(actualAttackSpeed * 0.5f);      // 공격 애니메이션 실행한지 50% 지나면
//         bool isDie = target.GetComponent<ACharacter>().Attacked(CalculateDamage());  // 데미지 계산
//         if (isDie)
//         {
//             // taget이 죽은 경우
//             SetTarget(null);
//         }
//         state = State.Idle;     // 공격 코루틴이 끝날 때 state 변경
//         yield break;
//     }

//     protected IEnumerator StartAttackCoolTime(float attackCoolTime)
//     {
//         yield return new WaitForSeconds(attackCoolTime);
//         canAttack = true;
//     }


//     // 피격 관련
//     public bool Attacked(int damage)
//     {
//         // 최종 데미지 - 데미지 리덕션 = 받는 데미지
//         int damageResult = (damage - stat.DamageReduction) > 0
//             ? damage - stat.DamageReduction
//             : 0;

//         stat.DecreaseHp(damageResult);
//         PlayAttackedSound();

//         UpdateHpBar();
        
//         if (damageTextSystem == null)
//         {
//             damageTextSystem = GameObject.Find("Damage Text System").GetComponent<DamageTextSystem>();
//         }
//         damageTextSystem.FloatDamageText(damage, this.transform.position);

//         if (stat.Hp < 1)
//         {
//             Die();
//             return true;
//         }
//         return false;
//     }
