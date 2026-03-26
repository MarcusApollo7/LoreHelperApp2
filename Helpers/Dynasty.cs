using System.Collections;
using System.Drawing;
using LoreHelperAppBlazor.Components.Pages;

namespace LoreHelperAppBlazor.Helpers;

public class Dynasty : IEnumerable<Person>
{
    public string House;
    public Person? Founder;
    public List<Person> Members;
    public Culture? Culture;
    public Dynasty(string house, Person founder)
    {
        House = house;
        Founder = founder;
        Members = [Founder];
        Culture = Founder.Culture;
    }
    public Dynasty(string house, List<Person> FoundingGeneration)
    {
        House = house;
        FoundingGeneration.Sort();
        Founder = FoundingGeneration[0];
        Members = [];
        for(int i = 0; i < FoundingGeneration.Count; i++)
        {
            Members.Add(FoundingGeneration[i]);
        }
        Culture = Founder.Culture;
    }
    public Dynasty(Culture culture)
    {
        House = "";
        Members = [];
        Culture = culture;
    }
    public int Count()
    {
        return Members.Count;
    }
    public IEnumerator<Person> GetEnumerator()
    {
        return new DynastyEnumerator(this);
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    public void BeFruitful(int generations)
    {
        for (int i = 0; i < generations; i++)
        {
            List<Person> children = [];
            foreach (Person person in Members)
            {
                person.LiveLife();
                children = [.. children, .. person.GetChildren()];
            }
            Members = [.. Members, .. children];
        }
    }
    public Person GetPerson(int index)
    {
        return Members[index];
    }
    public void AddFounder(Person founder)
    {
        if (Founder == null)
        {
            Founder = founder;
            Members.Add(Founder);
        }
        else
        {
            Console.WriteLine("Attempting to Add Founder when one already exists");
        }
    }
}
    
public class DynastyEnumerator(Dynasty dynasty) : IEnumerator<Person>
{
    private Dynasty _dynasty = dynasty;
    private int CurrentIndex = -1;
    private Person CurrentMember = new();

    public bool MoveNext()
    {
        //Avoids going beyond the end of the collection.
        if (++CurrentIndex >= _dynasty.Count())
        {
            return false;
        }
        else
        {
            // Set current person to next item in collection.
            CurrentMember = _dynasty.GetPerson(CurrentIndex);
        }
        return true;
    }
    public void Reset() { CurrentIndex = -1; }

    void IDisposable.Dispose() { }
    object IEnumerator.Current
    {
        get { return Current; }
    }
    public Person Current
    {
        get
        {
            try
            {
                return _dynasty.GetPerson(CurrentIndex);
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
public class PersonComparer: IComparer<Person>
{
    public int Compare(Person? x, Person? y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        if (x.BirthYear > y.BirthYear)
        {
            return -1;
        }
        else if (x.BirthYear < y.BirthYear)
        {
            return 1;
        }
        else
        {
            return 0;
        }  
    }
}
