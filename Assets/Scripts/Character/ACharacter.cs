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
    public NavMeshAgent Agent { get { return agent; } }

    protected DamageTextSystem damageTextSystem;    // !!! 임시

    protected abstract IEnumerator Attack();
    public abstract bool Attacked(int damage);
    protected abstract void Die();

    protected void Rotate(Vector3 dir)
    {
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
    }

    public void SetTarget(GameObject targetObject)
    {
        target = targetObject;
    }
}
