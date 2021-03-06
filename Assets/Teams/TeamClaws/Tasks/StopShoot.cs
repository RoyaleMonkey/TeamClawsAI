﻿using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace TeamClaws
{
    public class StopShoot : Action
    {
        public override TaskStatus OnUpdate()
        {
            ref InputData inputData = ref GetComponent<BOTController>().inputData;
            inputData.shoot = false;
            return TaskStatus.Success;
        }
    }
}
