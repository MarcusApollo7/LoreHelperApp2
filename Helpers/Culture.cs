using MathNet.Numerics.Distributions;

namespace LoreHelperAppBlazor.Helpers;
public class Culture
{
    public ITable Tables;
    public string CultureName;
    public string[] MaleNames;
    public string[] FemaleNames;
    public string[] NeuterNames;
    public int MaxPartners;
    public int AgeOfMajority;
    public bool Homosexuality;
    public bool Transition;
    public bool RegnalNames;

    public Culture(ITable tables, string culturename, string[] malenames, string[] femalenames, int maxpartners, int ageofmajority, bool homosexuality, bool transition, bool regnalname)
    {
        Tables = tables;
        CultureName = culturename;
        MaleNames = malenames;
        FemaleNames = femalenames;
        NeuterNames = [.. MaleNames, .. FemaleNames];
        MaxPartners = maxpartners;
        AgeOfMajority = ageofmajority;
        Homosexuality = homosexuality;
        Transition = transition;
        RegnalNames = regnalname;
    }
    public Culture(ITable tables, string culturename, string[] malenames, string[] femalenames, string[] neuternames, int maxpartners, int ageofmajority, bool homosexuality, bool transition, bool regnalname)
    {
        Tables = tables;
        CultureName = culturename;
        MaleNames = malenames;
        FemaleNames = femalenames;
        NeuterNames = neuternames;
        MaxPartners = maxpartners;
        AgeOfMajority = ageofmajority;
        Homosexuality = homosexuality;
        Transition = transition;
        RegnalNames = regnalname;
    }
    public Culture()
    {
        Tables = new DefaultTable();
        CultureName = "Culture";
        MaleNames = ["TestM1", "TestM2"];
        FemaleNames = ["TestF1", "TestM2"];
        NeuterNames = [.. MaleNames, .. FemaleNames];
        AgeOfMajority = 16;
        Homosexuality = true;
        Transition = true;
        RegnalNames = false;

    }
    public string[] GetNames(int gender)
    {
        if (gender == 0)
        {
            return MaleNames;
        }
        else if (gender == 1)
        {
            return FemaleNames;
        }
        else
        {
            return NeuterNames;
        }
    }
    public Person NewRandomPerson(int birthyear)
    {
        Categorical SexPicker = new(Tables.SexPicker);
        Bernoulli Trans = new(Tables.TransRate);
        Bernoulli Nb = new(Tables.NbRate);
        int sex = SexPicker.Sample();
        int trans = Trans.Sample(); // 0 Cis, 1 Trans
        int nb = 0;
        if (trans == 1){
            nb = Nb.Sample(); // 0 binary, 1 nonbinary
        }
        int gender;
        string[] possiblenames;
        if (Transition == true & trans == 1)
        { 
            if (sex == 0 & nb == 0) // binary transwoman
            {
                gender = 1;
            }
            else if (sex == 1 & nb == 0) // binary transman
            {
                gender = 0;
            }
            else // nonbinary
            {
                gender = 2;
            }
        }
        else
        {
            gender = sex;
        }
        possiblenames = GetNames(gender);
        Random random = new();
        string givenName = possiblenames[random.Next(possiblenames.Length)];
        return new(givenName, birthyear, sex, gender, this, trans, nb);
    }
    public Person FindPartner(Person Partner1, int current_year, int current_age)
    // Computes a person Partner1 would enter a realtionship with regardless of social norms
    {
        int gender;
        Bernoulli GenderPicker = new(1 - Partner1.Orientation/6); 
        /* 
        If Partner1's orientation is 0 (exclusively heterosexual) then relationship will always be heterosexual.
        Likewise, if Partner1's orientation is 1 (exclusively homosexual) then relationship will always be homosexual.
        Probability changes linearly as you move along the Kinsey Scale.
        */
        if (GenderPicker.Sample() == 1)
        {
            gender = Partner1.OppositeGender();
        }
        else
        {
            gender = Partner1.Gender;
        }
        Bernoulli Trans = new(Tables.TransRate);
        int trans = Trans.Sample();
        Bernoulli Nb = new(Tables.NbRate);
        int nb = 0;
        if (trans == 1){
            nb = Nb.Sample(); // 0 binary, 1 nonbinary
        }
        int sex;
        if (Transition == true & trans == 1)
        { 
            if (gender == 0 & nb == 0) // binary transman
            {
                sex = 1;
            }
            else if (gender == 1 & nb == 0) // binary transwoman
            {
                sex = 0;
            }
            else // nonbinary
            {
                sex = 2;
            }
        }
        else
        {
            sex = gender;
        }
        Random random = new();
        Partner1.BirthYear ??= 0;
        int birthyear = (int) Partner1.BirthYear;
        string[] possiblenames = GetNames(gender);
        string givenName = possiblenames[random.Next(possiblenames.Length)];
        return new(givenName, birthyear, sex, gender, this, trans, nb); 
        // Person generated will always be same age as Partener1
    }
    public override string ToString()
    {
        return CultureName;
    }
}

