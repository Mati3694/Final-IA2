using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mati36.FSM
{
    public class FSM<Feed>
    {
        private State<Feed> currentState;
        public State<Feed> CurrentState { get { return currentState; } }

        public FSM(State<Feed> initialState)
        {
            currentState = initialState;
            currentState.OnEnter?.Invoke();
        }

        public void FeedFSM(Feed feed)
        {
            var nextState = currentState.GetTransition(feed);
            if (nextState != null)
            {
                currentState.OnExit?.Invoke();
                currentState = nextState;
                currentState.OnEnter?.Invoke();
            }
        }

        public void UpdateFSM()
        {
            currentState.OnUpdate?.Invoke();
        }
    }
}