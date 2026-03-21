using System.Drawing;

namespace LoreHelperAppBlazor.LoreHelper;


public class Dyansty
{
    public string House;
    public Person Founder;
    public List<Generation> Members;
    public Culture Culture;
    public Dyansty(string house, Person founder)
    {
        House = house;
        Founder = founder;
        Members = [new(0, [founder])];
        Culture = Founder.Culture;
    }
    public Dyansty(string house, Generation FoundingGeneration)
    {
        House = house;
        FoundingGeneration.SortMembers();
        Founder = FoundingGeneration.GetMember(0);
        Members = [FoundingGeneration];
        Culture = Founder.Culture;
    }
    public void BeFruitful(int generations)
    {
        for (int i = 0; i < generations; i++)
        {
            Generation newgen = new(i + 1, []);
            foreach (Person person in Members[i])
            {
                person.LiveLife();
                Generation children = person.GetChildrenAsGeneration(i);
                newgen.ConcatGens(children);
            }
            Members.Add(newgen);
        }
    }
    public void PrintMembers()
    {
        foreach (Generation generation in Members)
        {
            Console.WriteLine($"Generation: {generation.GenNum}");
            foreach (Person person in generation)
            {
                person.PrintInfo();
            }
        }
    }

   
}
    


public class Generation(int gennum, List<Person> members) : IEnumerable<Person>
{
    public int GenNum = gennum;
    private List<Person> _members = members;
    public void AddMember(Person member)
    {
        _members.Add(member);
    }
    public void SortMembers()
    {
        _members.Sort(new PersonComparer());
    }
    public Person GetMember(int index)
    {
        return _members[index];
    }
    public int GetLength()
    {
        return _members.Count;
    }
    public void ConcatGens(Generation RHS)
    {
        if (GenNum != RHS.GenNum)
        {
            throw new Exception("Generations Numbers are not the same");
        }
        else
        {
        
            foreach (Person member in RHS)
            {
                AddMember(member);
            }
            
        }
    }
    public IEnumerator<Person> GetEnumerator()
    {
        foreach (Person person in _members)
        {
            yield return person;
        }
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    } 
    
}

public class PersonComparer: IComparer<Person>
{
    public int Compare(Person? x, Person? y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        return x.BirthYear.CompareTo(y.BirthYear);
    }
}
