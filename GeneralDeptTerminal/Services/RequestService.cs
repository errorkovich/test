using System;
using System.Collections.Generic;
using System.Linq;
using GeneralDeptTerminal.Models;

namespace GeneralDeptTerminal.Services
{
    public class RequestService
    {
        public List<Employee> Employees { get; } = new();
        public List<Request> Requests { get; } = new();
        public List<BlacklistEntry> Blacklist { get; } = new();

        public RequestService()
        {
            Seed();
        }

        private void Seed()
        {
            Employees.Add(new Employee { Id = 1, FullName = "Иванов И.И.", Code = "1234" });
            Employees.Add(new Employee { Id = 2, FullName = "Петров П.П.", Code = "5678" });
            Employees.Add(new Employee { Id = 3, FullName = "Тестовый сотрудник", Code = "777" });
            Employees.Add(new Employee { Id = 4, FullName = "Охранник Тестовый", Code = "888" }); // New security guard

            var r1 = new Request
            {
                Id = 1,
                Type = RequestType.Excursion,
                Department = Department.IT,
                Status = RequestStatus.Approved, // Set to approved for guard terminal
                CreatedAt = DateTime.Now.AddDays(-5),
                VisitDate = DateTime.Today.AddDays(1),
                VisitTime = new TimeSpan(10, 0, 0),
                AttachedFilesCount = 2,
                Applicants =
                {
                    new Applicant { FullName = "Сидоров А.А.", PassportNumber = "11112222", Email = "a@example.com" }
                }
            };

            var r2 = new Request
            {
                Id = 2,
                Type = RequestType.Maintenance,
                Department = Department.Security,
                Status = RequestStatus.Approved, // Set to approved for guard terminal
                CreatedAt = DateTime.Now.AddDays(-2),
                VisitDate = DateTime.Today,
                VisitTime = new TimeSpan(14, 30, 0),
                AttachedFilesCount = 1,
                Applicants =
                {
                    new Applicant { FullName = "Плохиш Б.Б.", PassportNumber = "99998888", Email = "b@example.com" }
                }
            };

            var r3 = new Request // New request, not in blacklist, approved
            {
                Id = 3,
                Type = RequestType.Audit,
                Department = Department.Production,
                Status = RequestStatus.Approved,
                CreatedAt = DateTime.Now.AddDays(-1),
                VisitDate = DateTime.Today.AddDays(2),
                VisitTime = new TimeSpan(9, 0, 0),
                AttachedFilesCount = 3,
                Applicants =
                {
                    new Applicant { FullName = "Григорьев Г.Г.", PassportNumber = "33334444", Email = "g@example.com" },
                    new Applicant { FullName = "Дмитриев Д.Д.", PassportNumber = "55556666", Email = "d@example.com" }
                }
            };

            var r4 = new Request // New request, with an applicant in blacklist, approved initially but will be rejected by CheckRequestForm
            {
                Id = 4,
                Type = RequestType.Excursion,
                Department = Department.IT,
                Status = RequestStatus.Approved,
                CreatedAt = DateTime.Now.AddHours(-3),
                VisitDate = DateTime.Today,
                VisitTime = new TimeSpan(11, 0, 0),
                AttachedFilesCount = 1,
                Applicants =
                {
                    new Applicant { FullName = "Злодей З.З.", PassportNumber = "99998888", Email = "z@example.com" } // Passport in blacklist
                }
            };

            Requests.Add(r1);
            Requests.Add(r2);
            Requests.Add(r3);
            Requests.Add(r4);

            Blacklist.Add(new BlacklistEntry
            {
                PassportNumber = "99998888",
                Reason = "Нарушение правил посещения объекта КИИ"
            });

            // Add more blacklist entries if needed for testing different scenarios
            Blacklist.Add(new BlacklistEntry
            {
                PassportNumber = "12345678",
                Reason = "Постоянный нарушитель"
            });
        }

        public Employee? Authorize(string code) =>
            Employees.FirstOrDefault(e => e.Code == code);

        public IEnumerable<Request> GetRequests(
            RequestType? type = null,
            Department? department = null,
            RequestStatus? status = null,
            DateTime? createdAt = null,
            string? search = null)
        {
            var query = Requests.AsEnumerable();

            if (type.HasValue)
                query = query.Where(r => r.Type == type.Value);
            if (department.HasValue)
                query = query.Where(r => r.Department == department.Value);
            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);
            if (createdAt.HasValue)
                query = query.Where(r => r.CreatedAt.Date == createdAt.Value.Date);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r => r.Applicants.Any(a =>
                    a.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    a.PassportNumber.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            return query;
        }

        public bool IsInBlacklist(Request request, out List<BlacklistEntry> entries)
        {
            var passports = request.Applicants.Select(a => a.PassportNumber).ToHashSet();
            entries = Blacklist.Where(b => passports.Contains(b.PassportNumber)).ToList();
            return entries.Count > 0;
        }

        public void AddToBlacklistIfNeeded(Request request)
        {
            if (request.FakeDataRejectCount >= 2)
            {
                foreach (var a in request.Applicants)
                {
                    if (!Blacklist.Any(b => b.PassportNumber == a.PassportNumber))
                    {
                        Blacklist.Add(new BlacklistEntry
                        {
                            PassportNumber = a.PassportNumber,
                            Reason = "Два отклонения по причине заведомо недостоверных данных"
                        });
                    }
                }
            }
        }

        public void SendMessageToApplicants(Request request, string text)
        {
            foreach (var a in request.Applicants)
            {
                Console.WriteLine($"Сообщение для {a.Email}: {text}");
            }
        }

        public void UpdateRequest(Request updatedRequest)
        {
            var existingRequest = Requests.FirstOrDefault(r => r.Id == updatedRequest.Id);
            if (existingRequest != null)
            {
                existingRequest.ActualVisitStartTime = updatedRequest.ActualVisitStartTime;
                existingRequest.ActualVisitEndTime = updatedRequest.ActualVisitEndTime;
                existingRequest.VisitDate = updatedRequest.VisitDate;
                existingRequest.VisitTime = updatedRequest.VisitTime;
                existingRequest.Status = updatedRequest.Status;
                existingRequest.FakeDataRejectCount = updatedRequest.FakeDataRejectCount;
            }
        }
    }
}


