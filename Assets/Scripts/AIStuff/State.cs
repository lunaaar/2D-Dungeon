using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public List<Transition> transitions;
    public int id;

    public State(int id)
    {
        transitions = new List<Transition>();
        this.id = id;
    }

    public State NextState()
    {
        foreach (var transition in transitions)
        {
            if (transition.condition())
            {
                return transition.target;
            }
        }
        return this;
    }
}
