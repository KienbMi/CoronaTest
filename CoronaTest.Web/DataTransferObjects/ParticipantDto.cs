using CoronaTest.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoronaTest.Web.DataTransferObjects
{
    public class ParticipantDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Der {0} ist verpflichtend")]
        [DisplayName("Vorname")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Der {0} ist verpflichtend")]
        [DisplayName("Nachname")]
        public string Lastname { get; set; }

        [DisplayName("Name")]
        public string Fullname => $"{Firstname} {Lastname}";

        [Required(ErrorMessage = "Der {0} ist verpflichtend")]
        [DisplayName("Geburtstag")]
        public DateTime Birthdate { get; set; }

        [Required(ErrorMessage = "Das {0} ist verpflichtend")]
        [DisplayName("Geschlecht")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Die {0} ist verpflichtend")]
        [StringLength(10, ErrorMessage = "Die {0} muss genau 10 Zeichen lang sein!", MinimumLength = 10)]
        [DisplayName("Sozialversicherungsnummer")]
        public string SocialSecurityNumber { get; set; }

        [Required(ErrorMessage = "Die {0} ist verpflichtend")]
        [StringLength(16, ErrorMessage = "Die {0} muss zw. {1} und {2} Zeichen lang sein!", MinimumLength = 5)]
        [DisplayName("Handynummer")]
        public string Mobilephone { get; set; }

        [Required(ErrorMessage = "Die {0} ist verpflichtend")]
        [DisplayName("Straße")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Die {0} ist verpflichtend")]
        [DisplayName("Hausnummer")]
        public string HouseNumber { get; set; }

        [DisplayName("Stiege")]
        public string Stair { get; set; }

        [DisplayName("Tür")]
        public string Door { get; set; }

        [Required(ErrorMessage = "Die {0} ist verpflichtend")]
        [DisplayName("PLZ")]
        public string Postalcode { get; set; }

        [Required(ErrorMessage = "Die {0} ist verpflichtend")]
        [DisplayName("Ort")]
        public string City { get; set; }

        public ParticipantDto()
        {

        }

        public ParticipantDto(Participant participant)
        {
            Id = participant.Id;
            Firstname = participant.Firstname;
            Lastname = participant.Lastname;
            Birthdate = participant.Birthdate;
            Gender = participant.Gender;
            SocialSecurityNumber = participant.SocialSecurityNumber;
            Mobilephone = participant.Mobilephone;
            Street = participant.Street;
            HouseNumber = participant.HouseNumber;
            Stair = participant.Stair;
            Door = participant.Door;
            Postalcode = participant.Postalcode;
            City = participant.City;
        }

        public void CopyContentToModel(ref Participant model)
        {
            model.Id = Id;
            model.Firstname = Firstname;
            model.Lastname = Lastname;
            model.Birthdate = Birthdate;
            model.Gender = Gender;
            model.SocialSecurityNumber = SocialSecurityNumber;
            model.Mobilephone = Mobilephone;
            model.Street = Street;
            model.HouseNumber = HouseNumber;
            model.Stair = Stair;
            model.Door = Door;
            model.Postalcode = Postalcode;
            model.City = City;           
        }

        public Participant GetNewModel()
        {
            var participant = new Participant();
            CopyContentToModel(ref participant);
            return participant;
        }
    }
}
