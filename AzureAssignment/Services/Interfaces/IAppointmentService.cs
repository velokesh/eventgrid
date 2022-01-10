using AzureAssignment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureAssignment.Services.Implementation
{
    public interface IAppointmentService
    {
        Task<(string msg, bool isErr)> CreateService(HttpRequest req);
        Task<(string msg, bool isErr)> UpdateAppointment(HttpRequest req, int id);

        public Task<List<Appointment>> GetAll();

        public Task<Appointment> GetAppointmentById(int id);

        public Task<string> DeleteAppointmentById(int id);
    }
}