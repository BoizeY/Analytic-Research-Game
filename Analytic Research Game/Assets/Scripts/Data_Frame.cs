public struct Data_Frame
{
    // Constructor
    public Data_Frame(int _id, char _group, NotificationType _not, float _avgErr)
    {
        // Init the data
        this.participantID = _id;
        this.participantGroup = _group;
        this.notificationType = _not;
        this.averageErrorRate = _avgErr;
    }

    // Methods
    public override string ToString()
    {
        // Return this object in the format for a csv
        return this.participantID.ToString() + "," + this.participantGroup.ToString() + "," + this.notificationType.ToString() + "," + this.averageErrorRate.ToString();
    }

    // Data
    public int participantID;
    public char participantGroup;
    public NotificationType notificationType;
    public float averageErrorRate;
}
