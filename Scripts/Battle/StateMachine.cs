using UnityEngine;

namespace Battle
{
    public abstract class StateMachine : MonoBehaviour
    {
        protected State State;

        public void SetState(State state)
        {
            State = state;
            StartCoroutine((State.StartBattle()));
        }
    }
}