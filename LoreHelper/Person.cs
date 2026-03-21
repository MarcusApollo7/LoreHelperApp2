using MathNet.Numerics.Distributions;

namespace LoreHelperAppBlazor.LoreHelper;
public class Person
{
    // Properites
    /* 
    Identity is an Identity which tracks the name, sex, and gender of Person
    BirthYear is an int which notes the year the person was born
    DeathYear is an int which can be null but notes the year the person died
    Parents is a list of Persons which lists the parents of the person
    Children is a list of Persons which lists the children of the person
    Partners is a list of Persons which lists the Partners of the person
    */
    public readonly Culture Culture;
    public Name Names;
    private readonly int Sex; // 0 Male, 1 Female, 2 Intersex
    public int Gender; // 0 Man, 1 Woman, 2 Nonbinary
    public int Orientation; // Kinsey Scale 0 Exclusively Hetro, 6 Exclusively Homosexual
    public int Ace; // 0 Allosexual, 1 Asexual 
    public int Trans; // 0 No, 1 Yes
    public int NB; // 0 No, 1 Yes
    public int BirthYear;
    public int? DeathYear;
    public Dictionary<Person, List<RelationshipType>> Relations;

    // Constructors
    public Person(string name, int birthyear, int sex, int gender, Culture culture, int trans, int nb)
    {
        Culture = culture;
        Names = new(name); 
        BirthYear = birthyear;
        Trans = trans;
        NB = nb;
        Sex = sex;
        Gender = gender;
        DeathYear = null;
        Relations = [];
        Orientation = new Random().Next(7);
        Ace = new Bernoulli(.03).Sample();
    }
    // Methods
    public Generation GetChildrenAsGeneration(int current_gen)
    {
        List<Person> children = GetRelations(RelationshipType.ParentOf);
        return new(current_gen + 1, children);
    }
    
    public void PrintInfo()
    {
        Console.WriteLine($"{Names.GivenNameatBirth} born: {BirthYear}, orientation: {Orientation}");
        Console.WriteLine("Relations:");
        foreach (KeyValuePair<Person, List<RelationshipType>> pair in Relations)
        {
            foreach(RelationshipType relation in pair.Value)
            {
                Console.WriteLine($"{Names.GivenNameatBirth} is the {relation} {pair.Key.Names.GivenNameatBirth}");
            }
        }
    }
    public void LiveLife()
    {
        bool living = true;
        int current_age = 0;
        int current_year = BirthYear;
        while (living)
        {
            if (Death(current_age)) // If person dies
            {
                DeathYear = current_age + BirthYear;
                living = false;
            }
            else // If person does not die
            {
                List<Person> Partners = GetRelations([RelationshipType.Marriage, RelationshipType.Sexual]);
                if (current_age == Culture.AgeOfMajority & Partners.Count <= Culture.MaxPartners)
                {
                    Console.WriteLine("Getting Married");
                    GetPartner(current_year, current_age);
                }
                if (CanGetPregnant()) // If person can get pregnant try to have child
                {
                    HaveChild(current_age, current_year);
                }
                foreach (Person partner in Partners) // Each partner of the person tries to have a child if able
                {
                    if (partner.CanGetPregnant())
                    {
                        partner.HaveChild(current_year - partner.BirthYear, current_year, this);
                    }
                }
                // increment age and year
                current_age++;
                current_year++;                
            }
        }
    }
    public bool Death(int age)
    {
        Bernoulli b = new(Culture.Tables.Death[age]);
        int death = b.Sample();
        return death == 1;
    }
    public bool CanGetPregnant()
    {
        if (Sex == 1 & Gender == 1)
        {

            return true;
        }
        else
        {
            return false;
        }
    }
    public void HaveChild(int current_age, int current_year)
    {
        Bernoulli b = new(Culture.Tables.FBirth[current_age]);
        if (b.Sample() == 1)
        {
            Console.WriteLine("Child Born");
            Person child = Culture.NewRandomPerson(current_year);
            child.AddRelation(this, RelationshipType.ChildOf);
            AddRelation(child, RelationshipType.ParentOf);
        }
    }
    public void HaveChild(int current_age, int current_year, Person sire)
    {
        Bernoulli b = new(Culture.Tables.FBirth[current_age]);
        if (b.Sample() == 1)
        {
            Console.WriteLine("Child Born");
            Person child = Culture.NewRandomPerson(current_year);
            child.AddRelation(this, RelationshipType.ChildOf);
            AddRelation(child, RelationshipType.ParentOf);

            child.AddRelation(sire, RelationshipType.ChildOf);
            sire.AddRelation(child, RelationshipType.ParentOf);
        }
    }
    public void GetPartner(int current_year, int current_age)
    {
        Culture PartnerCulture = Culture;
        Person partner = PartnerCulture.FindPartner(this, current_year, current_age);
        RelationshipType relation;
        if (Culture.Homosexuality == false & this.Gender == partner.Gender)
        {
            relation = RelationshipType.Sexual;
        }
        else if (Culture.Homosexuality == true & this.Gender == partner.Gender)
        {
            relation = RelationshipType.Marriage;
        }
        else
        {
            relation = RelationshipType.Marriage;
        }
        AddRelation(partner, relation);
        partner.AddRelation(this, [relation]);
    }
    public int OppositeGender()
    {
        if (Sex == 0)
        {
            return 1;
        }
        else if (Sex == 1)
        {
            return 0;
        }
        else
        {
            return 2;
        }
    }
    public void AddRelation(Person person, RelationshipType relation)
    // Used for adding a single relation
    {
        if (Relations.TryGetValue(person, out List<RelationshipType>? value))
        {
            value.Add(relation);
        }
        else
        {
            Relations[person] = [relation];
        }
    }
    public void AddRelation(Person person, List<RelationshipType> relations)
    // Used for adding a set of relations
    {
        if (Relations.TryGetValue(person, out List<RelationshipType>? value))
        {
            value.Concat(relations);
        }
        else
        {
            Relations[person] = [.. relations];
        }
    }
    public List<Person> GetRelations(RelationshipType relation)
    // Used for getting all people with a given relation
    {
        List<Person> output = [];
        foreach (KeyValuePair<Person, List<RelationshipType>> relationship in Relations)
        {
            if (relationship.Value.Contains(relation)){
                output.Add(relationship.Key);
            }
        }
        return output;
    }
    public List<Person> GetRelations(RelationshipType[] relations)
    // Used for getting all people with a given set of relations
    {
        List<Person> output = [];
        foreach (KeyValuePair<Person, List<RelationshipType>> relationship in Relations)
        {
            foreach (RelationshipType relation in relations)
            {
                if (relationship.Value.Contains(relation)){
                    output.Add(relationship.Key);
                }
            }
        }
        return output;
    }
    
}
public class Name(string givennameatbirth)
{
    public string GivenNameatBirth = givennameatbirth;
    public string? RegnalName {get; set;}
}

public enum RelationshipType
{
    Marriage,
    Romantic,
    Sexual,
    SiblingOf,
    ParentOf,
    ChildOf
}
