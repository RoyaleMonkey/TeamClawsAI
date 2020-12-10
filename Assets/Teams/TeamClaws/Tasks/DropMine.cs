using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace TeamClaws
{
    public class DropMine : Action
    {
        public override TaskStatus OnUpdate()
        {
            ref InputData inputData = ref GetComponent<BOTController>().inputData;
            inputData.dropMine = true;
            return TaskStatus.Success;
        }
    }
}
