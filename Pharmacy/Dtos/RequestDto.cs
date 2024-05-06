namespace Pharmacy.Dtos
{
    public class RequestDto
    {
        public int RequestId { get; set; } // Add the RequestId property

        public string PatientName { get; set; }
        public List<string> MedicinesNames { get; set; }
        public RequestStatus Status { get; set; }
    }
}
