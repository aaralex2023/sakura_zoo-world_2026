using System;
using UnityEngine;
using ZooWorld.Animals;

namespace ZooWorld.Tests
{
    public sealed class StateLifecycleProbe : MonoBehaviour, IAnimalContextUser, IAnimalStateEnterListener,
        IAnimalStateExitListener, IAnimalStateTickListener, IAnimalStatePredictor
    {
        public Action TickAction;
        public int Setups;
        public int Enters;
        public int Exits;
        public int Ticks;
        public int Predictions;

        public void SetupContext(AnimalContext context) => Setups++;
        public void OnEnter() => Enters++;
        public void OnExit() => Exits++;

        public void OnTick(float dt)
        {
            Ticks++;
            TickAction?.Invoke();
        }

        public bool TryPopNextState(out AnimalStateId state)
        {
            Predictions++;
            state = null;
            return false;
        }
    }
}
