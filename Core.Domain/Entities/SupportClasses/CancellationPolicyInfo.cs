namespace Core.Domain.Entities.SupportClasses
{

    public class CancellationPolicyInfo
    {
        public bool FreeCancellation { get; set; }
        public string Deadline { get; set; } = string.Empty;
        public string Penalty { get; set; } = string.Empty;
    }


}
