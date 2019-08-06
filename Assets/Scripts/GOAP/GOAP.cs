using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class GOAP
{
    public static List<GoapAction<Model>> Run<Model>(Model initalState,
                                                 Func<Model, bool> satisfies,
                                                 List<GoapAction<Model>> actions,
                                                 Func<Model, float> heuristic,
                                                 int maxSteps = 5000)
    {
        var fakeInitialState = (initalState, default(GoapAction<Model>));

        Func<(Model, GoapAction<Model>), bool> fakeSatisfies = t =>
        {
            return satisfies(t.Item1);
        };

        Func<(Model, GoapAction<Model>), float> fakeHeuristic = t =>
        {
            return heuristic(t.Item1);
        };

        Func<(Model, GoapAction<Model>), IEnumerable<((Model, GoapAction<Model>), float)>> expand = state =>
        {
            var list = new List<((Model, GoapAction<Model>), float)>();

            foreach (var action in actions)
            {
                if (action.condition(state.Item1))
                {
                    list.Add(((action.effect(state.Item1), action), action.cost));
                }
            }

            return list;
        };
        var result = AStar.Run(fakeInitialState, fakeSatisfies, expand, fakeHeuristic,  maxSteps);
        if (result == null)
            return null;
        return result.Skip(1).Select(t => t.Item2).ToList();
    }
}
