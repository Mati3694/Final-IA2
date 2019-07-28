using System.Collections.Generic;
using System;

public static class AStar
{
	public static List<Node> Run<Node>
		(
			Node start,
			Func<Node, bool> satisfies,
			Func<Node, IEnumerable<(Node, float)>> expand,
			Func<Node, float> heuristic,
            Func<Node, bool> isValid,
            int maxSteps = 5000
		)
	{
		//Nodos sin visitar
		var open = new MinHeap<Node>(64);
		//Arranca con el primer nodo
		open.Push(start, 0);

		//Nodos ya visitados
		var closed = new HashSet<Node>();

		//Diccionario de padres, Key: Hijo, Value: Padre
		var parents = new Dictionary<Node, Node>();

		//Diccionarios de costos, Key: Nodo, Value: costo tentativo para llegar
		var costs = new Dictionary<Node, float>();
		costs[start] = 0;

        int watchdog = maxSteps;

		while (open.Count > 0 && watchdog > 0)//Todavia haya nodos para chequear
		{
            watchdog --;
			//Obtenemos el nodo con el camino mas corto
			var current = open.Pop();

			var currentCost = costs[current];

			if (satisfies(current))//Si el nodo cumple la condicion
			{
				return ConstructPath(current, parents);//Devolvemos el camino a ese nodo
			}

			//Ponemos al current en el closed asi no lo volvemos a chequear
			closed.Add(current);

			//Para cada hijo del current
			foreach (var childPair in expand(current))
			{
				var child = childPair.Item1;
				var childCost = childPair.Item2;

				//Si el nodo ya lo habimos procesado lo salteamos
				if (closed.Contains(child)) continue;

				var tentativeCost = currentCost + childCost;
				if (costs.ContainsKey(child) && tentativeCost > costs[child]) continue;

				parents[child] = current;//Le seteamos el padre

				costs[child] = tentativeCost;
				open.Push(child, tentativeCost + heuristic(child));//Lo agregamos a la cola
			}

		}

		//Si ningun nodo cumplio la condicion
		return null;
	}

	private static List<Node> ConstructPath<Node>(Node end, Dictionary<Node, Node> parents)
	{
		//Conocemos el final del camino y de donde venimos por los parents
		//Vamos a armar el camino del final al inicio y despues lo revertimos

		var path = new List<Node>();
		path.Add(end);//Emppezamos con el ultimo

		//Mientras el ultimo nodo tenga un padre
		while (parents.ContainsKey(path[path.Count - 1]))
		{
			var lastNode = path[path.Count - 1];//El ultimo nodo

			//Agregamos el padre del ultimo nodo al final
			path.Add(parents[lastNode]);
		}

		path.Reverse();//Lo damos vuenta
		return path;
	}

}