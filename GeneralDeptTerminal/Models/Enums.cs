namespace GeneralDeptTerminal.Models
{
    public enum RequestStatus
    {
        New,
        OnCheck,
        Approved,
        Rejected
    }

    public enum RequestType
    {
        Excursion,
        Maintenance,
        Audit
    }

    public enum Department
    {
        IT,
        Security,
        Production
    }
}


