using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Student initialState;

    //Aproved: true
    //knowledge : true

    void Start()
    {
        var actions = new List<GoapAction<Student>>()
        {
            new GoapAction<Student>(
                    "Study",
                    state => state.time && !state.tired,
                    state =>
                    {
                        state.time = false;
                        state.tired = true;
                        state.knowledge = true;
                        return state;
                    },
                    2f
                ),
            new GoapAction<Student>(
                    "Sleep",
                    state => state.tired,
                    state =>
                    {
                        state.tired = false;
                        return state;
                    },
                    1f
                ),
            new GoapAction<Student>(
                    "Rending",
                    state => state.knowledge && !state.tired,
                    state =>
                    {
                        state.aproved = true;
                        state.time = true;
                        return state;
                    },
                    3f
                ),
            new GoapAction<Student>(
                    "Dont sleep",
                    state => true,
                    state =>
                    {
                        state.tired = true;
                        state.time = true;
                        return state;
                    },
                    4f
                )
        };

        var actionPath = GOAP.Run(initialState, Satisfies, actions, Heuristic);

        if(actionPath == null)
        {
            Debug.Log("No podes hacer nada para llegar a tu goal.");
            return;
        }

        foreach (var action in actionPath)
        {
            Debug.Log(action.name);
        }
    }

    float Heuristic(Student state)
    {
        var count = 0;
        if (!state.aproved) count++;
        if (!state.knowledge) count++;
        return count;
    }

    bool Satisfies(Student state)
    {
        return state.aproved && state.knowledge;
    }

}
