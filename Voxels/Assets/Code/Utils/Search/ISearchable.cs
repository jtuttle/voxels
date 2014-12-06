using System.Collections.Generic;

public interface ISearchable {
    List<ISearchable> Neighbors();
}
