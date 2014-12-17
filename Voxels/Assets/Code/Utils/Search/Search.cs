using System.Collections;
using System.Collections.Generic;

public class Search {
    public delegate bool InclusionCondition(ISearchable from, ISearchable to);

    public static IEnumerable<ISearchable> BreadthFirstSearch(ISearchable start, InclusionCondition inclusionCondition) {
        Queue<ISearchable> queue = new Queue<ISearchable>();
        HashSet<ISearchable> set = new HashSet<ISearchable>();

        queue.Enqueue(start);
        set.Add(start);

        while(queue.Count > 0) {
            ISearchable next = queue.Dequeue();

            yield return next;

            foreach(ISearchable neighbor in next.Neighbors) {
                if(!set.Contains(neighbor) && inclusionCondition(next, neighbor)) {
                    queue.Enqueue(neighbor);
                    set.Add(neighbor);
                }
            }
        }
    }
}
