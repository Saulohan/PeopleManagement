using AutoMapper;
using Newtonsoft.Json;
using PeopleManagement.Application.DTOs;
using PeopleManagement.Domain.Entities;
using System.Text.RegularExpressions;

namespace PeopleManagement.Application.Mapping
{
    public class PersonProfileMap : Profile
    {
        public PersonProfileMap()
        {
            CreateMap<Person, PersonDto>()
                .ReverseMap();
        }
    }
}