namespace Core.Domain.Entities.SupportClasses
{
    public class AvailabilityInfo
    {
        public AvailabilityInfo(string? status, int remainingSlots)
        {
            Status = status;
            RemainingSlots = remainingSlots;
        }

        public AvailabilityInfo()
        {
            
            
        }


        public string? Status { get; set; }
        public int RemainingSlots { get; set; }

        public bool IsAvailable => Status?.ToLower() != "not available" && RemainingSlots > 0;
    }
}
