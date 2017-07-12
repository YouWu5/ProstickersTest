namespace ProStickers.ViewModel.Core
{
    public class StatusEnum
    {
        public enum TimeSlotStatus
        {
            Available = 1,
            Booked = 2,
            AvailableAfterBooked = 3
        }

        public enum AppointmentStatus
        {
            Scheduled = 1,
            Initiated = 2,
            Completed = 3,
            Cancelled = 4
        }

        public enum Status
        {
            AppointmentScheduled = 1,
            AppointmentCompleted = 2,
            AppointmentCancelled = 3,
            OrderCreated = 4,
            OrderShipped = 5,
        }

        public enum OrderStatus
        {
            Placed = 1,
            Shipped = 2
        }

        public enum AppointmentRequestStatus
        {
            Created = 1,
            Processed = 2
        }

        public enum PurchaseType
        {
            DesignSticker = 1,
            VectorFile = 2,
            Both = 3
        }

        public enum Blob
        {
            customerfiles = 1,
            userfiles = 2,
            vectorimage = 3,
            designimage = 4,
            userimage = 5,
            colorimage = 6
        }

        public enum UploadImageStatus
        {
            UploadDesignImage = 1,
            UploadVectorImage = 2,
            UploadBoth = 3,
        }
    }
}
