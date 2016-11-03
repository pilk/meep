using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animation))]
public class EntityAnimation : EntityComponent
{
    private Animation m_animation = null;
    private int m_currentPriority = 0;

    public Animation AnimationComponent
    {
        get { return m_animation; }
    }

    private readonly Dictionary<string, int> ANIMATION_PRIORITY_TABLE = new Dictionary<string, int>(System.StringComparer.Ordinal)
    {
        {"Idle_1", 1},
        {"Run", 1},
        {"Damaged", 2},
        {"Death", 10},
    };

    protected override void Start()
    {
        base.Start();
        m_animation = this.GetComponent<Animation>();

        m_entityController.Move += this.entityController_Move;
        m_entityController.FinishedMoving += this.entityController_FinishedMoving;


        m_animation.wrapMode = WrapMode.Once;
        PlayAnimation("Idle_1", true);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        m_entityController.Move -= this.entityController_Move;
        m_entityController.FinishedMoving -= this.entityController_FinishedMoving;

    }

    private void entityController_FinishedMoving()
    {
        if (PlayAnimation("Idle_1"))
        {
            m_animation.wrapMode = WrapMode.Loop;
        }
    }

    private void entityController_Move(Vector3 direction)
    {
        if (PlayAnimation("Run"))
        {
            m_animation.wrapMode = WrapMode.Loop;
        }
    }

    private void Update()
    {
        if (m_animation.isPlaying == false)
        {
            m_currentPriority = 0;
            PlayAnimation("Idle_1");
        }
    }

    public bool PlayAnimation(string animation, bool force = false)
    {
        int priority = 1;
        if (ANIMATION_PRIORITY_TABLE.ContainsKey(animation))
        {
            priority = ANIMATION_PRIORITY_TABLE[animation];
        }
        return this.PlayAnimation(animation, priority, force);
    }


    public bool PlayAnimation(string animation, int priority, bool force = false)
    {
        //if (m_animation.IsPlaying(animation) == false)
        {
            if (m_currentPriority <= priority)
            {
                m_animation.wrapMode = WrapMode.Once;
                m_currentPriority = priority;
                if (force && m_animation.IsPlaying(animation))
                {
                    m_animation.Stop();
                }
                m_animation.Play(animation);
                return true;
            }
        }
        return false;
    }

}
