using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class PlayerCharacter : MonoBehaviour
{
    [Header("Дебіл")]
    [SerializeField] private float _runSpeed = 10f;
    [SerializeField] private float _speedInAir = 7f;
    [SerializeField] private float _crouchSpeed = 5f;
    [SerializeField] private float ClimbSpeed = 5f;
    [SerializeField] private float _speedInWater = 5f;
    [SerializeField] private float JumpForce = 17f;
    [SerializeField] private float DefaultGravity = 3f;
    [SerializeField] private Transform GroundCheckPoint;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private Animator Anim;
    [SerializeField] private Rigidbody2D Body;
    [SerializeField] private CircleCollider2D FootCircle;
    [SerializeField] private BoxCollider2D HitBox;

    private bool _facinRight = true; //виносити в старт
    private bool Grounded;
    private bool NearLadder;
    private bool OnPlatform;
    private float AxisX;
    private float AxisY; //виносити в вектор2
    private bool _esc;
    private bool LShift;
    private enum CharacterState { OnGround, InAir, OnLadder, InWater };

    private CharacterState PlayerState = CharacterState.InAir; //в старті
    private ControllState GameControll = ControllState.InGame;
    private CharacterState PrevPlayerState;

    private const float GROUND_RADIUS = 0.3f;

    private void OnValidate()
    {
        if (Anim == null)
        {
            Anim = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        Body.gravityScale = DefaultGravity;

        if (GroundCheckPoint == null)
        {
            Debug.Log("Перетягни дочірню Point обєкта ГГ в параметри скріпта бо не бачить землю!");
        }
    }

    private void FixedUpdate()
    {
        switch (GameControll)
        {
            case ControllState.InMenu:
                InMenuControll();
                break;
            case ControllState.InGame:
                PlayerStateMachine();
                break;
        }
    }
    
    private void InMenuControll()
    {
        if (_esc) 
        { 
            GameControll = ControllState.InGame;
        }
    }

    private void PlayerStateMachine()
    {
        if (_esc)
            GameControll = ControllState.InMenu;

        //тупа звичка з паскаля - треба позбуватися
        if (AxisX > 0 && !_facinRight)
            Flip();
        else if (AxisX < 0 && _facinRight)
            Flip();

        Grounded = Physics2D.OverlapCircle(GroundCheckPoint.position, GROUND_RADIUS, GroundLayer);

        switch (PlayerState) //StateMachineStart---------------------------------------------------
        {
            case CharacterState.InAir: //██████████████████████████████████████████████████████████

                Body.velocity = new Vector2(AxisX * _speedInAir, Body.velocity.y);

                if (Grounded)
                    SetState(CharacterState.OnGround);

                if (!OnPlatform && FootCircle.isTrigger)
                    FootCircle.isTrigger = false;

                if (NearLadder && AxisY != 0 && !LShift)
                {
                    Body.velocity = Vector2.zero;
                    Body.gravityScale = 0;
                    SetState(CharacterState.OnLadder);
                }

                break;

            case CharacterState.OnGround: //███████████████████████████████████████████████████████

                if (!LShift)
                    Body.velocity = new Vector2(AxisX * _runSpeed, Body.velocity.y);
                else
                    Body.velocity = new Vector2(AxisX * _crouchSpeed, Body.velocity.y);


                if (AxisY > 0)
                    Body.velocity = new Vector2(Body.velocity.x, JumpForce);

                if (!Grounded)
                    SetState(CharacterState.InAir);

                if (OnPlatform && AxisY < 0 && !NearLadder)
                    FootCircle.isTrigger = true;

                if (NearLadder && AxisY != 0 && !LShift)
                {
                    Body.velocity = Vector2.zero;
                    Body.gravityScale = 0;
                    SetState(CharacterState.OnLadder);
                }

                break;

            case CharacterState.OnLadder: //███████████████████████████████████████████████████████

                Body.velocity = new Vector2(AxisX * ClimbSpeed, AxisY * ClimbSpeed);

                if (LShift | !NearLadder)
                {
                    Body.gravityScale = DefaultGravity;
                    FootCircle.isTrigger = false;
                    SetState(CharacterState.InAir);
                }

                break;

            case CharacterState.InWater: //████████████████████████████████████████████████████████

                if (AxisX !=0 | AxisY !=0)
                    if (Grounded)
                        Body.velocity = new Vector2(AxisX * _runSpeed, AxisY * _speedInWater);
                    else
                        Body.velocity = new Vector2(AxisX * _speedInWater, AxisY * _speedInWater);

                if (OnPlatform && AxisY < 0)
                    FootCircle.isTrigger = true;

                if (!OnPlatform && FootCircle.isTrigger)
                    FootCircle.isTrigger = false;

                break;
        } //StateMachineEnd------------------------------------------------------------------------

        AnimationParametersUpdate();
    }

    private void Update()
    {
        AxisX = Input.GetAxis("Horizontal");
        AxisY = Input.GetAxis("Vertical");
        LShift = Input.GetKey(KeyCode.LeftShift);
        _esc = Input.GetKey(KeyCode.Escape);

        Debug.Log($" PlayerState {OnPlatform} OnPlatform    FCDisabled  {FootCircle.isTrigger}");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
            NearLadder = true;

        if (collision.gameObject.tag == "Water")
            SetState(CharacterState.InWater);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
            NearLadder = false;

        if (collision.gameObject.tag == "Platform" && PlayerState == CharacterState.OnLadder)
            FootCircle.isTrigger = false;

        if (collision.gameObject.tag == "Platform")
            OnPlatform = false;

        if (collision.gameObject.tag == "Water")
            SetState(CharacterState.InAir);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
            OnPlatform = true;

        if (collision.gameObject.tag == "Platform" && PlayerState == CharacterState.OnLadder)
            FootCircle.isTrigger = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
            OnPlatform = false;
    }

    private void SetState(CharacterState NewState)
    {
        if (PrevPlayerState != PlayerState)
            PrevPlayerState = PlayerState;

        PlayerState = NewState;
    }

    private void Flip()
    {
        _facinRight = !_facinRight;

        Vector3 LocalScale = transform.localScale;
        LocalScale.x *= -1;

        transform.localScale = LocalScale;
    }

    private void AnimationParametersUpdate()
    {
        Anim.SetInteger("PlayerState", (int)PlayerState);
        Anim.SetFloat("Speed", Mathf.Abs(Body.velocity.x));
        Anim.SetFloat("VSpeed", Body.velocity.y);
        Anim.SetBool("Shift", LShift);
    }
}