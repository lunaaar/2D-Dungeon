using System;
using System.Collections.Generic;

public class PairingHeap<T> where T : IComparable<T>
{
    public T Element;
    List<PairingHeap<T>> subheaps;
    static readonly public PairingHeap<T> EmptyHeap = new PairingHeap<T>();

    public PairingHeap(T _element = default)
    {
        Element = _element;
        subheaps = new List<PairingHeap<T>>();
    }

    public PairingHeap(T _element, List<PairingHeap<T>> _subheaps)
    {
        Element = _element;
        subheaps = _subheaps;
    }

	public T FindMin()
    {
        return Element;
    }

    public bool Empty()
    {
        return Element == null;
    }

    public static PairingHeap<T> Meld(PairingHeap<T> a, PairingHeap<T> b)
    {
        if (a.Empty())
        {
            return b;
        }
        else if (b.Empty())
        {
            return a;
        }
        if (a.Element.CompareTo(b.Element) < 0)
        {
            var heaps = new List<PairingHeap<T>>(a.subheaps);
            heaps.AddRange(b.subheaps);
            return new PairingHeap<T>(a.Element, heaps);
        }
        else
        {
            var heaps = new List<PairingHeap<T>>(b.subheaps);
            heaps.AddRange(a.subheaps);
            return new PairingHeap<T>(b.Element, heaps);
        }

    }

    public PairingHeap<T> Insert(T element)
    {
        return Meld(new PairingHeap<T>(element), this);
    }

    public PairingHeap<T> DeleteMin()
    {
        if (Empty())
        {
            return this;
        }
        else
        {
            return MergePairs(this.subheaps);
        }
    }

    public static PairingHeap<T> MergePairs(List<PairingHeap<T>> list)
    {
        if (list.Count == 0)
        {
            return EmptyHeap;
        }
        else if (list.Count == 1)
        {
            return list[0];
        }
        else
        {
            var fst = list[0];
            var snd = list[1];
            var rest = list.GetRange(2, list.Count - 1);
            return Meld(Meld(fst, snd), MergePairs(rest));
        }
    }
}
