using System.ComponentModel.DataAnnotations;
using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;

namespace LoreHelperAppBlazor.Helpers;
public class PersonItem
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
    [Required(ErrorMessage = "Need culture")]
    public Culture Culture {get; set;}
    [Required(ErrorMessage = "Need Name")]
    public Name Names {get;}
    private readonly int Sex; // 0 Male, 1 Female, 2 Intersex
    [Required(ErrorMessage = "Need Gender")]
    public int Gender {get; set;} // 0 Man, 1 Woman, 2 Nonbinary
    public int Orientation; // Kinsey Scale 0 Exclusively Hetro, 6 Exclusively Homosexual
    public int Ace; // 0 Allosexual, 1 Asexual 
    public int Trans; // 0 No, 1 Yes
    public int NB; // 0 No, 1 Yes
    public int? BirthYear;
    public int? DeathYear;
    public Dictionary<PersonItem, List<RelationshipType>> Relations;

    // Constructors
    public PersonItem(string name, int birthyear, int sex, int gender, Culture culture, int trans, int nb)
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
    public PersonItem()
    {
        Culture = new();
        Names = new("");
        BirthYear = null;
        Relations = [];
        Trans = -1;
        NB = -1;
        Sex = -1;
        Gender = -1;
        DeathYear = null;
        Relations = [];
        Orientation = -1;
        Ace = -1;
        
    }
    // Methods
    public List<PersonItem> GetChildren()
    {
        return GetRelations(RelationshipType.ParentOf);
        
    }
    public string GetGenderIdentity()
    {
        if (Gender == 0)
        {
            return "Man";
        }
        else if (Gender == 1)
        {
            return "Woman";
        }
        else if (Gender == -1)
        {
            return "";
        }
        else
        {
            return "Non-binary";
        }
    }
    public void PrintInfo()
    {
        Console.WriteLine($"{Names.TrueName} born: {BirthYear}, orientation: {Orientation}");
        Console.WriteLine("Relations:");
        foreach (KeyValuePair<PersonItem, List<RelationshipType>> pair in Relations)
        {
            foreach(RelationshipType relation in pair.Value)
            {
                Console.WriteLine($"{Names.TrueName} is the {relation} {pair.Key.Names.TrueName}");
            }
        }
    }
    public void LiveLife()
    {
        bool living = true;
        int current_age = 0;
        BirthYear ??= 0;
        int current_year = (int)BirthYear;
        while (living)
        {
            if (Death(current_age)) // If person dies
            {
                DeathYear = current_age + BirthYear;
                living = false;
            }
            else // If person does not die
            {
                List<PersonItem> Partners = GetRelations([RelationshipType.Marriage, RelationshipType.Sexual]);
                if (current_age == Culture.AgeOfMajority & Partners.Count <= Culture.MaxPartners)
                {
                    Console.WriteLine("Getting Married");
                    GetPartner(current_year, current_age);
                }
                if (CanGetPregnant()) // If person can get pregnant try to have child
                {
                    HaveChild(current_age, current_year);
                }
                foreach (PersonItem partner in Partners) // Each partner of the person tries to have a child if able
                {
                    if (partner.CanGetPregnant())
                    {
                        partner.BirthYear ??= 0;
                        partner.HaveChild((int)(current_year - partner.BirthYear), current_year, this);
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
            PersonItem child = Culture.NewRandomPerson(current_year);
            child.AddRelation(this, RelationshipType.ChildOf);
            AddRelation(child, RelationshipType.ParentOf);
        }
    }
    public void HaveChild(int current_age, int current_year, PersonItem sire)
    {
        Bernoulli b = new(Culture.Tables.FBirth[current_age]);
        if (b.Sample() == 1)
        {
            Console.WriteLine("Child Born");
            PersonItem child = Culture.NewRandomPerson(current_year);
            child.AddRelation(this, RelationshipType.ChildOf);
            AddRelation(child, RelationshipType.ParentOf);

            child.AddRelation(sire, RelationshipType.ChildOf);
            sire.AddRelation(child, RelationshipType.ParentOf);
        }
    }
    public void GetPartner(int current_year, int current_age)
    {
        Culture PartnerCulture = Culture;
        PersonItem partner = PartnerCulture.FindPartner(this, current_year, current_age);
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
    public void AddRelation(PersonItem person, RelationshipType relation)
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
    public void AddRelation(PersonItem person, List<RelationshipType> relations)
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
    public List<PersonItem> GetRelations(RelationshipType relation)
    // Used for getting all people with a given relation
    {
        List<PersonItem> output = [];
        foreach (KeyValuePair<PersonItem, List<RelationshipType>> relationship in Relations)
        {
            if (relationship.Value.Contains(relation)){
                output.Add(relationship.Key);
            }
        }
        return output;
    }
    public List<PersonItem> GetRelations(RelationshipType[] relations)
    // Used for getting all people with a given set of relations
    {
        List<PersonItem> output = [];
        foreach (KeyValuePair<PersonItem, List<RelationshipType>> relationship in Relations)
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
    private string GivenNameatBirth = givennameatbirth;
    public string TrueName = givennameatbirth;
    public string? RegnalName {get; set;}

    public override string ToString()
    {
        return TrueName;
    }
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

