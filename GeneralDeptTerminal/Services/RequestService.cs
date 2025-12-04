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

            var r1 = new Request
            {
                Id = 1,
                Type = RequestType.Excursion,
                Department = Department.IT,
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
                AttachedFilesCount = 1,
                Applicants =
                {
                    new Applicant { FullName = "Плохиш Б.Б.", PassportNumber = "99998888", Email = "b@example.com" }
                }
            };

            Requests.Add(r1);
            Requests.Add(r2);

            Blacklist.Add(new BlacklistEntry
            {
                PassportNumber = "99998888",
                Reason = "Нарушение правил посещения объекта КИИ"
            });
        }

        public Employee? Authorize(string code) =>
            Employees.FirstOrDefault(e => e.Code == code);

        public IEnumerable<Request> GetRequests(
            RequestType? type = null,
            Department? department = null,
            RequestStatus? status = null)
        {
            return Requests.Where(r =>
                (!type.HasValue || r.Type == type.Value) &&
                (!department.HasValue || r.Department == department.Value) &&
                (!status.HasValue || r.Status == status.Value));
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
    }
}


