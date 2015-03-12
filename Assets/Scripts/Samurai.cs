using UnityEngine;
using System.Collections;
using FSM;
public class Samurai : MonoBehaviour {

    public SGAnimation animationMotor;
    public State state;
	// Use this for initialization
	void Start () {
        animationMotor = transform.GetChild(0).GetComponent<SGAnimation>();
        state = new Running();
        if (state == null)
            Debug.Log("its null you fucking idiot");
	}
	
	// Update is called once per frame
	void Update () {
        if (state == null)
            Debug.Log("its null");
        state.Update(this);
        state.HandleInput(this);
	}
}

namespace FSM
{
    public class State
    {
        public State()
        {

        }
        public virtual void HandleInput(Samurai samurai)
        {   }
        public virtual void Update(Samurai samurai)
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
            
        }
        public override void HandleInput(Samurai samurai)
        {
            base.HandleInput(samurai);
            if (Input.GetKeyUp(Slash1.keyTriggerType))
            {
                samurai.animationMotor.ChangeAnimation(5);
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
                    samurai.animationMotor.ChangeAnimation(3);
                    samurai.SendMessage("Jump");
                }
                else
                {
                    samurai.state = new Jumping();
                    samurai.animationMotor.ChangeAnimation(0);
                    samurai.SendMessage("Jump");
                }
            }
        }
        public override void Update(Samurai samurai)
        {
            if (samurai.GetComponent<GravityMotor>().gravityOn)
            {
                samurai.state = new Falling();
                samurai.animationMotor.ChangeAnimation(1);
            }
            if (Input.GetKey(SliceAttack.keyTriggerType))
                timer += 0.002f;
            if (timer > maxSliceDelay)
                samurai.state = new SliceAttack();
            base.Update(samurai);
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
        }
        public override void HandleInput(Samurai samurai)
        {
            base.HandleInput(samurai);
            if (Input.GetKeyUp(Slash2.keyTriggerType))
            {
                samurai.state = new Slash2();
                samurai.animationMotor.ChangeAnimation(5);
            }
            if (Input.GetKeyUp(Jumping.keyTriggerType))
            {
                if (timer > maxRiseAttackDelay)
                {
                    samurai.SendMessage("Jump");
                    samurai.animationMotor.ChangeAnimation(3);
                    samurai.state = new RisingAttack();
                }
                else
                {
                    samurai.state = new Jumping();
                    samurai.SendMessage("Jump");
                    samurai.animationMotor.ChangeAnimation(0);
                }
            }
            if (Input.GetKey(SliceAttack.keyTriggerType))
                if(timer > maxSliceTime)
                samurai.state = new SliceAttack();
            
        }
        public override void Update(Samurai samurai)
        {
            base.Update(samurai);
            timer += 0.002f;

            if (timer > maxSlashTime)
            {
                samurai.state = new Sheathe();
                samurai.animationMotor.ChangeAnimation(6);
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
        }
        public override void HandleInput(Samurai samurai)
        {
            base.HandleInput(samurai);

            if (Input.GetKeyUp(Slash3.keyTriggerType))
            {
                samurai.animationMotor.ChangeAnimation(5);
                samurai.state = new Slash3();
            }
            if (Input.GetKeyUp(Jumping.keyTriggerType))
            {
                if (timer > maxRiseAttackDelay)
                {
                    samurai.SendMessage("Jump");
                    samurai.animationMotor.ChangeAnimation(3);
                    samurai.state = new RisingAttack();
                }
                else
                {
                    samurai.state = new Jumping();
                    samurai.SendMessage("Jump");
                    samurai.animationMotor.ChangeAnimation(0);
                }
            }
            if (Input.GetKey(SliceAttack.keyTriggerType))
                if (timer > maxSliceTime)
                    samurai.state = new SliceAttack();
        }
        public override void Update(Samurai samurai)
        {
            base.Update(samurai);
            timer += 0.002f;

            if (timer > maxSlashTime)
            {
                samurai.state = new Sheathe();
                samurai.animationMotor.ChangeAnimation(6);
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
            Debug.Log("Slash3 State");
            timer = 0;
            FSMDiagram.instance.ChangeState("Slash3");
        }
        public override void HandleInput(Samurai samurai)
        {
            base.HandleInput(samurai);
            if (Input.GetKeyUp(Jumping.keyTriggerType))
            {
                if (timer > maxRiseAttackDelay)
                {
                    samurai.SendMessage("Jump");
                    samurai.animationMotor.ChangeAnimation(3);
                    samurai.state = new RisingAttack();
                }
                else
                {
                    samurai.state = new Jumping();
                    samurai.SendMessage("Jump");
                    samurai.animationMotor.ChangeAnimation(0);
                }
            }
            if (Input.GetKey(SliceAttack.keyTriggerType))
                if (timer > maxSliceTime)
                    samurai.state = new SliceAttack();
        }
        public override void Update(Samurai samurai)
        {
            base.Update(samurai);
            timer += 0.002f;

            if (timer > maxSlashTime)
            {
                samurai.state = new Sheathe();
                samurai.animationMotor.ChangeAnimation(6);
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
        }
        public override void HandleInput(Samurai samurai)
        {
            base.HandleInput(samurai);
        }
        public override void Update(Samurai samurai)
        {
            timer += 0.001f;
            if (timer > sheathingTime)
            {
                samurai.animationMotor.ChangeAnimation(2);
                samurai.state = new Running();
            }
            base.Update(samurai);
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

        }
        public override void HandleInput(Samurai samurai)
        {
            base.HandleInput(samurai);
            //if (Input.GetKeyUp(Slash1.keyTriggerType))
            //{
            //    samurai.animationMotor.ChangeAnimation(4);
            //    samurai.state = new FallingAttack();
            //}
        }
        public override void Update(Samurai samurai)
        {
            base.Update(samurai);
          
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


    public class Falling : State
    {
        float timer;
        float maxSmashTime = 0.05f;
        public Falling()
        {
            Debug.Log("Falling State");
            timer = 0;
            FSMDiagram.instance.ChangeState("Falling");
        }
        public override void HandleInput(Samurai samurai)
        {
            base.HandleInput(samurai);
            if (Input.GetKeyUp(Slash1.keyTriggerType))
            {
                samurai.animationMotor.ChangeAnimation(4);
                samurai.state = new FallingAttack();
            }
        }
        public override void Update(Samurai samurai)
        {
            base.Update(samurai);
            if (Input.GetKey(SliceAttack.keyTriggerType))
                timer+= 0.002f;
            if (samurai.GetComponent<JumpMotor>().allowedToJump &&
                !samurai.GetComponent<GravityMotor>().gravityOn)
            {
                samurai.state = new Running();
                samurai.animationMotor.ChangeAnimation(2);
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
        }
        public override void HandleInput(Samurai samurai)
        {
            base.HandleInput(samurai);
            //if (Input.GetKeyDown(Slash1.keyTriggerType))
            //{
            //    samurai.animationMotor.ChangeAnimation(4);
            //    samurai.state = new FallingAttack();
            //}
        }
        public override void Update(Samurai samurai)
        {
            base.Update(samurai);

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
        }

        public override void HandleInput(Samurai samurai)
        {
            base.HandleInput(samurai);
        }
        public override void Update(Samurai samurai)
        {
            base.Update(samurai);
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
        public override void HandleInput(Samurai samurai)
        {
            base.HandleInput(samurai);
        }
        public override void Update(Samurai samurai)
        {
            base.Update(samurai);
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
        public override void HandleInput(Samurai samurai)
        {
            base.HandleInput(samurai);
        }
        public override void Update(Samurai samurai)
        {
            timer += 0.002f;
            if (timer > maxSliceTime)
            {
                samurai.state = new Sheathe();
                samurai.animationMotor.ChangeAnimation(6);
            }
            base.Update(samurai);
        }
    }
}