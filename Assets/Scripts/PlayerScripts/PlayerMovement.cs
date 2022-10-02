using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState{
    walk,
    attack,
    interact,
    stagger,
    idle
}

public class PlayerMovement : MonoBehaviour {


    public PlayerState currentState;
    public float speed;
    private Rigidbody2D myRigidbody;
    private Vector3 change;
    private Animator animator;
    public FloatValue currentHealth;
    public SignalSender playerHealthSignal;
    private float rollSpeed;
    public float rollLength = .6f;
    public float rollCooldown = 5f;
    private float activeMoveSpeed;
    public bool isRolling;
    public int coins;

	// Use this for initialization
	void Start () {
        rollSpeed = speed * 2.5f;
        activeMoveSpeed = speed;
        currentState = PlayerState.walk;
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);

	}
	
	// Update is called once per frame
	void Update () {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        
       if(Input.GetButtonDown("attack") && currentState != PlayerState.attack 
           && currentState != PlayerState.stagger)
        {
            StartCoroutine(AttackCo());
        }
      
        else if (currentState == PlayerState.walk || currentState == PlayerState.idle)
        {
            
            UpdateAnimationAndMove();
        }
	}

    private IEnumerator AttackCo()
    {
        animator.SetBool("attacking", true);
        currentState = PlayerState.attack;
        yield return null;
        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(.3f);
        currentState = PlayerState.walk;
    }
  
    private IEnumerator RollCo()
    {
        isRolling=true;
        activeMoveSpeed = rollSpeed;
        yield return new WaitForSeconds(rollLength);
        activeMoveSpeed = speed;
        yield return new WaitForSeconds(rollCooldown);
        isRolling=false;
    }
  
    void UpdateAnimationAndMove()
    {
        if (change != Vector3.zero)
        {
            MoveCharacter();
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    void MoveCharacter()
    {
        change.Normalize();
        
        myRigidbody.MovePosition(
            transform.position + change * activeMoveSpeed * Time.fixedDeltaTime
        );

        if(Input.GetButtonDown("roll") && currentState != PlayerState.attack 
           && currentState != PlayerState.stagger && isRolling == false)
        { 
            StartCoroutine(RollCo());
        }
        }
    public void SpeedPowerup(){
        speed = speed * 1.1f;
        rollSpeed = rollSpeed * 1.1f;
        activeMoveSpeed = speed;
        print(speed);
        print(rollSpeed);
       
    }

    public void Knock(float knockTime, float damage)
    {
        currentHealth.RuntimeValue-=damage;
        playerHealthSignal.Raise();
        if(currentHealth.RuntimeValue > 0)
        {
        
        StartCoroutine(KnockCo(knockTime));
        }else{
            this.gameObject.SetActive(false);
        }
    }

    private IEnumerator KnockCo(float knockTime)
    {
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            currentState = PlayerState.idle;
            myRigidbody.velocity = Vector2.zero;
        }
    }
    
}