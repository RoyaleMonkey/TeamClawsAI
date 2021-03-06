﻿using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace TeamClaws
{
    public class Shoot : Action
    {
        public override TaskStatus OnUpdate()
        {
            GetComponent<BOTController>()._blackboard._latestGameData.SpaceShips[GetComponent<BOTController>()._blackboard._owner].Shoot();
            return TaskStatus.Success;
        }
    }
}
