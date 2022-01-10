using AzureAssignment.DTOs;
using AzureAssignment.Models;
using AzureAssignment.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureAssignment.Services.Implementation
{
    public class AppointmentService : IAppointmentService
    {
        private IRepository<Appointment> _repository;
        public AppointmentService( IRepository<Appointment> repository)
        {
            _repository = repository;
        }

        public async Task<List<Appointment>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<string> DeleteAppointmentById(int id)
        {
            Appointment appointment = await _repository.Get(id);
            await _repository.Delete(appointment);
            return "Appointement deleted successfully";
        }

        public async Task<Appointment> GetAppointmentById(int id)
        {
            return await _repository.Get(id);
        }

        public async Task<(string msg, bool isErr)> CreateService(HttpRequest req)
        {
            //Get request body data
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var body = JsonConvert.DeserializeObject<CreateAppointmentDto>(requestBody);


            //Check whether Name exists or not
            if (body.Name == null)
            {
                return ("Name is Required", true);
            }

            //Check whether appointment exsist or not
            var isAppointmentPresent = await _repository.Get(body.Id);
            if (isAppointmentPresent != null)
            {
                return ("Appointment Already present" ,true);
            }

            //Create Object from Dto
            Appointment appointment = new Appointment()
            {
                Id = body.Id,
                Name = body.Name
            };

            await _repository.Insert(appointment);

            return ("Appointment Created", false);
        }


        public async Task<(string msg, bool isErr)> UpdateAppointment(HttpRequest req, int id)
        {
            //Get request body data
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var body = JsonConvert.DeserializeObject<UpdateAppointmentDTO>(requestBody);

            if (body.Id != id)
            {
                return ("Id cannot be changed", true);
            }

            //Check whether Name exists or not
            if (body.Name == null)
            {
                return ("Name is Required", true);
            }

            //Check whether appointment exists or not
            var appointment = await _repository.Get(id);
            if (appointment == null)
            {
                return ("Appointment with id-" + id + " not found", true);
            }

            //Create Object from Dto
            Appointment updatedAppointment = new Appointment()
            {
                Id = body.Id,
                Name = body.Name
            };

            await _repository.Update(updatedAppointment);
            return ("Appointment with id-" + id + " updated successfully", false);

        }
    }
}
