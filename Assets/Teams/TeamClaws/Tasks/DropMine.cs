using UnityEngine;
using DoNotModify;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

namespace TeamClaws
{
    public class DropMine : Action
    {
        public SharedFloat EnergyToDropMine;
        public override TaskStatus OnUpdate()
        {
            ref InputData inputData = ref GetComponent<BOTController>().inputData;
            
            if (GetComponent<BOTController>()._blackboard._latestGameData.SpaceShips[GetComponent<BOTController>()._blackboard._owner].Energy > EnergyToDropMine.Value)
                inputData.dropMine = true;
            return TaskStatus.Success;
        }
    }
}
