using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mati36.FSM
{
    public class State<Feed>
    {
        public string stateName;
        public Action OnEnter, OnUpdate, OnExit;
        public Dictionary<Feed, State<Feed>> transitions = new Dictionary<Feed, State<Feed>>();

        public State(string name, Action onEnter, Action onUpdate, Action onExit)
        {
            stateName = name;
            OnEnter = onEnter; OnUpdate = onUpdate; OnExit = onExit;
        }

        public State<Feed> GetTransition(Feed feed)
        {
            if (!transitions.ContainsKey(feed)) return null;
            return transitions[feed];
        }

        public void AddTransition(Feed feed, State<Feed> transitionTo)
        {
            transitions[feed] = transitionTo;
        }
    }
}