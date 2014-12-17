using System.Collections.Generic;

public interface ISearchable {
    List<ISearchable> Neighbors { get; }
    int Value { get; }
}
