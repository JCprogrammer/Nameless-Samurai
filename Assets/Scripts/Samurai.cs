using UnityEngine;
using System.Collections;
using FSM;
public class SamuraiAnimationMotor
{
    public SGAnimation[] animationMotor;
    public void ChangeAnimation(int index)
    {
        foreach (var item in animationMotor)
        {
            item.ChangeAnimation(index);
        }
    }

}
public class Samurai : MonoBehaviour {

    public SamuraiAnimationMotor animationMotor;
    public State state;
	// Use this for initialization
	void Start () {
        animationMotor = new SamuraiAnimationMotor(); 
            animationMotor.animationMotor = transform.GetComponentsInChildren<SGAnimation>();

        State.samurai = this;
        state = new Running();
        
        if (state == null)
            Debug.Log("its null you fucking idiot");
	}
	
	// Update is called once per frame
	void Update () {
        if (state == null)
            Debug.Log("its null");
        state.Update();
        state.HandleInput();
	}
}

namespace FSM
{
    public class State
    {
        public static Samurai samurai;
        public State(Samurai samurai)
        {
            FSM.State.samurai = samurai;
        }
        public State()
        {
           
        }
        public virtual void HandleInput( )
        {   }
        public virtual void Update( )
        {   }
    }


    public class Running : State
    {
        float timer;
        float maxSliceDelay = 0.13f;
        float maxRiseAttackDelay = 0.005f;
        public Running()
        {
            timer = 0;
            Debug.Log("Running State");
            if(FSMDiagram.instance != null)
            FSMDiagram.instance.ChangeState("Running");
            samurai.animationMotor.ChangeAnimation(2);
            
        }
        public override void HandleInput( )
        {
             ;
            if (Input.GetKeyUp(Slash1.keyTriggerType))
            {
                samurai.state = new Slash1();
            }
            //if (Input.GetKeyDown(Slash1.keyTriggerType) &&
            //    Input.GetKeyUp(Jumping.keyTriggerType))
            //{
            //    samurai.state = new RisingAttack();
            //    samurai.animationMotor.ChangeAnimation(3);
            //}
            if (Input.GetKeyUp(Jumping.keyTriggerType))
            {
                if (timer > maxRiseAttackDelay)
                {
                    samurai.state = new RisingAttack();
                }
                else
                {
                    samurai.state = new Jumping();
                }
            }
        }
        public override void Update( )
        {
            if (samurai.GetComponent<GravityMotor>().gravityOn)
            {
                samurai.state = new Falling();;
            }
            if (Input.GetKey(SliceAttack.keyTriggerType))
                timer += 0.002f;
            if (timer > maxSliceDelay)
                samurai.state = new SliceAttack();
             ;
        }
    }


    public class Slash1 : State
    {
        public static KeyCode keyTriggerType = KeyCode.A;
        float timer = 0;
        float maxSlashTime = 0.15f;
        float maxSliceTime = 0.13f;
        float maxRiseAttackDelay = 0.005f;
        public Slash1()
        {
            Debug.Log("Slash1 State");
            timer = 0;
            FSMDiagram.instance.ChangeState("Slash1");
            samurai.animationMotor.ChangeAnimation(5);
        }
        public override void HandleInput( )
        {
             ;
            if (Input.GetKeyUp(Slash2.keyTriggerType))
            {
                samurai.state = new Slash2();
            }
            if (Input.GetKeyUp(Jumping.keyTriggerType))
            {
                if (timer > maxRiseAttackDelay)
                {
                    samurai.state = new RisingAttack();
                }
                else
                {
                    samurai.state = new Jumping();
                }
            }
            if (Input.GetKey(SliceAttack.keyTriggerType))
                if(timer > maxSliceTime)
                samurai.state = new SliceAttack();
            
        }
        public override void Update( )
        {
            timer += 0.002f;

            if (timer > maxSlashTime)
            {
                samurai.state = new Sheathe();
            }
        }
    }


    public class Slash2 : State
    {
        public static KeyCode keyTriggerType = KeyCode.A;
        float timer = 0;
        float maxSlashTime = 0.15f;
        float maxSliceTime = 0.13f;
        float maxRiseAttackDelay = 0.005f;
        public Slash2()
        {
            Debug.Log("Slash2 State");
            timer = 0;
            FSMDiagram.instance.ChangeState("Slash2");
            samurai.animationMotor.ChangeAnimation(5);
        }
        public override void HandleInput( )
        {
             ;

            if (Input.GetKeyUp(Slash3.keyTriggerType))
            {
                samurai.state = new Slash3();
            }
            if (Input.GetKeyUp(Jumping.keyTriggerType))
            {
                if (timer > maxRiseAttackDelay)
                {
                    samurai.state = new RisingAttack();
                }
                else
                {
                    samurai.state = new Jumping();
                }
            }
            if (Input.GetKey(SliceAttack.keyTriggerType))
                if (timer > maxSliceTime)
                    samurai.state = new SliceAttack();
        }
        public override void Update( )
        {
            timer += 0.002f;

            if (timer > maxSlashTime)
            {
                samurai.state = new Sheathe();
            }
        }
    }


    public class Slash3 : State
    {
        public static KeyCode keyTriggerType = KeyCode.A;
        float timer = 0;
        float maxSlashTime = 0.15f;
        float maxSliceTime = 0.13f;
        float maxRiseAttackDelay = 0.005f;
        public Slash3()
        {
            samurai.animationMotor.ChangeAnimation(5);
            Debug.Log("Slash3 State");
            timer = 0;
            FSMDiagram.instance.ChangeState("Slash3");
        }
        public override void HandleInput( )
        {
             ;
            if (Input.GetKeyUp(Jumping.keyTriggerType))
            {
                if (timer > maxRiseAttackDelay)
                {
                    samurai.state = new RisingAttack();
                }
                else
                {
                    samurai.state = new Jumping();
                }
            }
            if (Input.GetKey(SliceAttack.keyTriggerType))
                if (timer > maxSliceTime)
                    samurai.state = new SliceAttack();
        }
        public override void Update( )
        {
            timer += 0.002f;
            if (timer > maxSlashTime)
            {
                samurai.state = new Sheathe();
            }
        }
    }


    public class Sheathe : State
    {
        float timer;
        float sheathingTime = 0.02f;

        public Sheathe()
        {
            timer = 0;
            Debug.Log("Sheathe State");
            FSMDiagram.instance.ChangeState("Sheathe");
            samurai.animationMotor.ChangeAnimation(6);
        }
        public override void HandleInput( )
        {
        }
        public override void Update( )
        {
            timer += 0.001f;
            if (timer > sheathingTime)
            {
                samurai.state = new Running();
            }
             ;
        }
    }


    public class Jumping : State
    {
        public static KeyCode keyTriggerType = KeyCode.Space;
        float timer;
        float maxSmashTime = 0.05f;
        public Jumping()
        {
            Debug.Log("Jumping State");
            timer = 0;
            FSMDiagram.instance.ChangeState("Jumping");
            samurai.animationMotor.ChangeAnimation(0);
            samurai.SendMessage("Jump");

        }
        public override void HandleInput( )
        {
            //if (Input.GetKeyUp(Slash1.keyTriggerType))
            //{
            //    samurai.animationMotor.ChangeAnimation(4);
            //    samurai.state = new FallingAttack();
            //}
        }
        public override void Update( )
        {
            if (Input.GetKey(SliceAttack.keyTriggerType))
                timer += 0.002f;
          
            if (timer > maxSmashTime)
            {
                samurai.state = new SmashAttack();
            }
            if (samurai.GetComponent<GravityMotor>().gravityOn)
            {
                samurai.state = new Falling();
            }
        }
    }


    public class Falling : State
    {
        float timer;
        float maxSmashTime = 0.05f;
        public Falling()
        {
            Debug.Log("Falling State");
            timer = 0;
            FSMDiagram.instance.ChangeState("Falling");
            samurai.animationMotor.ChangeAnimation(1);
        }
        public override void HandleInput( )
        {
            if (Input.GetKeyUp(Slash1.keyTriggerType))
            {
                samurai.state = new FallingAttack();
            }
        }
        public override void Update( )
        {
             ;
            if (Input.GetKey(SliceAttack.keyTriggerType))
                timer+= 0.002f;
            if (samurai.GetComponent<JumpMotor>().allowedToJump &&
                !samurai.GetComponent<GravityMotor>().gravityOn)
            {
                samurai.state = new Running();
            }
            if (timer > maxSmashTime)
            {
                samurai.state = new SmashAttack();
            }

        }
    }


    public class RisingAttack : State
    {
         float timer;
        float maxSmashTime = 0.05f;
        public RisingAttack()
        {
            Debug.Log("RisingAttack State");
            timer = 0;
            FSMDiagram.instance.ChangeState("RisingAttack");
            samurai.animationMotor.ChangeAnimation(3);
            samurai.SendMessage("Jump");
        }
        public override void HandleInput( )
        {
             ;
            //if (Input.GetKeyDown(Slash1.keyTriggerType))
            //{
            //    samurai.animationMotor.ChangeAnimation(4);
            //    samurai.state = new FallingAttack();
            //}
        }
        public override void Update( )
        {
             ;

            if (Input.GetKey(SliceAttack.keyTriggerType))
                timer += 0.002f;

            if (timer > maxSmashTime)
            {
                samurai.state = new SmashAttack();
            }

            if (samurai.GetComponent<GravityMotor>().gravityOn)
            {
                samurai.animationMotor.ChangeAnimation(1);
                samurai.state = new Falling();

            }
        }
    }


    public class FallingAttack : State
    {
        public FallingAttack()
        {
            Debug.Log("FallingAttack State");
            FSMDiagram.instance.ChangeState("FallingAttack");
            samurai.animationMotor.ChangeAnimation(4);
        }

        public override void HandleInput( )
        {
             ;
        }
        public override void Update( )
        {
             ;
            if (samurai.GetComponent<JumpMotor>().allowedToJump)
            {
                samurai.state = new Sheathe();
                samurai.animationMotor.ChangeAnimation(6);
            }
           

        }
    }


    public class SmashAttack : State
    {
        float timer;
        float maxCharacterDelay = 0.2f;
        public SmashAttack()
        {
            timer = 0;
            Debug.Log("SmashAttack State");
            FSMDiagram.instance.ChangeState("SmashAttack");
        }
        public override void HandleInput( )
        {
             ;
        }
        public override void Update( )
        {
             ;
            if (samurai.GetComponent<JumpMotor>().allowedToJump)
                timer += 0.002f;
            if (timer > maxCharacterDelay)
            {
                samurai.state = new Sheathe();
                samurai.animationMotor.ChangeAnimation(6);
            }
        }
    }


    public class SliceAttack: State
    {
        float timer;
        float maxSliceTime = 0.25f;
        public static KeyCode keyTriggerType = KeyCode.A;
        public SliceAttack()
        {
            timer = 0;
            Debug.Log("SliceAttack State");
            FSMDiagram.instance.ChangeState("SliceAttack");
        }
        public override void HandleInput( )
        {
             ;
        }
        public override void Update( )
        {
            timer += 0.002f;
            if (timer > maxSliceTime)
            {
                samurai.state = new Sheathe();
                samurai.animationMotor.ChangeAnimation(6);
            }
             ;
        }
    }
}