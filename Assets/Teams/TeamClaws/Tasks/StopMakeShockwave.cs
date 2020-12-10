using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace TeamClaws
{
    public class StopMakeShockwave : Action
    {
        public override TaskStatus OnUpdate()
        {
            ref InputData inputData = ref GetComponent<BOTController>().inputData;
            inputData.fireShockwave = false;
            return TaskStatus.Success;
        }
    }
}
