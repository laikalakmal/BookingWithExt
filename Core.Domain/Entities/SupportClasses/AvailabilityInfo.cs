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

        private bool? _isAvailableOverride = null;
        
        public bool IsAvailable
        {
            get => _isAvailableOverride ?? (Status?.ToLower() != "not available" && RemainingSlots > 0);
            set => _isAvailableOverride = value;
        }
    }
}
