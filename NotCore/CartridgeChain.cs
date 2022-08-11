using System.Collections;
using System.Collections.Generic;

namespace NotCore;

public class CartridgeChain
{
    private readonly LinkedList<ICartridge> _list = new();

    private ICartridge Current => _list.First!.Value;

    public void Update(float dt)
    {
        Current.Update(dt);

        if (Current.ShouldLoadNextCartridge())
        {
            IncrementCartridge();
        }
    }

    public void Draw(Painter painter)
    {
        Current.Draw(painter);
    }

    private void IncrementCartridge()
    {
        _list.RemoveFirst();
    }

    public void Append(ICartridge cartridge)
    {
        _list.AddLast(cartridge);
    }

    public void Prepend(ICartridge cartridge)
    {
        _list.AddFirst(cartridge);
    }

    public IEnumerable<ICartridge> GetAll()
    {
        foreach (var cartridge in _list)
        {
            yield return cartridge;
        }
    }
}
