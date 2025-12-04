using System;
using System.Collections.Generic;

namespace GeneralDeptTerminal.Models
{
    public class Request
    {
        public int Id { get; set; }
        public RequestType Type { get; set; }
        public Department Department { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.New;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? VisitDate { get; set; }
        public TimeSpan? VisitTime { get; set; }

        public DateTime? ActualVisitStartTime { get; set; }
        public DateTime? ActualVisitEndTime { get; set; }

        public List<Applicant> Applicants { get; set; } = new();
        public int AttachedFilesCount { get; set; }

        public int FakeDataRejectCount { get; set; }
    }
}


