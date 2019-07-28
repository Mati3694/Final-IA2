using System;

[Serializable]
public struct Student
{
    public bool aproved;
    public bool time;
    public bool tired;
    public bool knowledge;//conocimiento

    public Student Clone()
    {
        return new Student()
        {
            aproved = aproved,
            time = time,
            tired = tired,
            knowledge = knowledge
        };
    }
}
