using System.Collections.Generic;

namespace ExplogineMonoGame.Cartridges;

public interface ILoadEventProvider
{
    public IEnumerable<LoadEvent> LoadEvents(Painter painter);
}
