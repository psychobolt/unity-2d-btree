
using System;
using System.Collections.Generic;
using BTree;
using UnityEngine;

[RequireComponent(typeof(EnemyAIActor))]
[RequireComponent(typeof(BTreeSelectorGroup))]
[AddComponentMenu("AI Behaviour Tree/Respawn")]
public class TakeDamageBehaviour : AbstractBTreeBehaviour
{
    public float disappearAnimationTime = 1.0f;
    public float appearAnimationTime = 1.0f;
    public int hitPercentage = 100;
    public float reappearDelay = 1.0f;
    public bool randomPosition;

    private EnemyAIActor actor;

    protected override void Start()
    {
        base.Start();
        actor = gameObject.GetComponent<EnemyAIActor>();
    }
    
    public bool IsLowHealth()
    {
        return ((actor.totalHP - actor.health) / actor.totalHP) >= hitPercentage;
    }

    public BehaviourTree.State Disappear(BehaviourTreeNode<float> node)
    {
        if (node.Result == 0)
        {
            animationController.Disappear();
        }
        node.Result += Time.deltaTime;
        if (node.Result > disappearAnimationTime)
        {
            return BehaviourTree.State.SUCCESS;
        }
        return BehaviourTree.State.RUNNING;
    }

    public BehaviourTree.State Appear(BehaviourTreeNode<float> node)
    {
        if (node.Result == 0)
        {
            animationController.Appear();
        }
        node.Result += Time.deltaTime;
        if (node.Result > appearAnimationTime)
        {
            node.Result = 0;
            if (randomPosition)
            {
                gameObject.transform.position = GetRandomPosition();
            }
            return BehaviourTree.State.SUCCESS;
        }
        return BehaviourTree.State.RUNNING;
    }

    public Vector3 GetRandomPosition()
    {
        return new Vector3(0f, 0f, 0f);
    }

    public override BehaviourTree.Node GetBehaviourTree()
    {
        return new BinaryTreeNode(
            IsLowHealth,
            new SequenceTreeNode(new BehaviourTree.Node[]
            {
                new ActionTreeNode<float>(Disappear),
                new ActionTreeNode<float>(Appear),
                new ActionTreeNode<System.Object>(actor.Revive)
            }),
            new ActionTreeNode<System.Object>(node => BehaviourTree.State.FAILURE)
        );
    }
}