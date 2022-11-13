using System.Collections.Generic;

namespace ExplogineMonoGame.Cartridges;

public interface ILoadEventProvider
{
    public IEnumerable<ILoadEvent?> LoadEvents(Painter painter);
}
