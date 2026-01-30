using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BildungsBericht.Models
{
    public class CLBenutzer
    {
        [JsonPropertyName( "benutzerId" )]
        public int BenutzerId { get; set; }

        [Required]
        [StringLength( 100, MinimumLength = 2 )]
        [JsonPropertyName( "firstname" )]
        public string FirstName { get; set; }

        [Required]
        [JsonPropertyName( "lastname" )]
        public string LastName { get; set; }

        //[Required]
        //[JsonPropertyName( "email" )]
        //public string Email { get; set; }

        //[JsonPropertyName( "dateofbrith" )]
        //public DateTime DateOfBrith { get; set; }

        //[JsonPropertyName( "gender" )]
        //public Gender Gender { get; set; }

        //[JsonPropertyName( "departmentid" )]
        //public int DepartmentId { get; set; }

        //[JsonPropertyName( "photopath" )]
        //public string PhotoPath { get; set; }

        //public Benutzer( int iBenutzerId, String iFirstName, String iLastName, string iEmail, DateTime iDateOfBrith, Gender iGender,
        //    int iDepartmentId, string iPhotoPath )
        //{
        //    BenutzerId = iBenutzerId;
        //    FirstName = iFirstName;
        //    LastName = iLastName;
        //    Email = iEmail;
        //    DateOfBrith = iDateOfBrith;
        //    Gender = iGender;
        //    DepartmentId = iDepartmentId;
        //    PhotoPath = iPhotoPath;
        //}

        //public Benutzer( int iBenutzerId, String iFirstName, String iLastName )
        //{
        //    BenutzerId = iBenutzerId;
        //    FirstName = iFirstName;
        //    LastName = iLastName;
        //    //Email = "";
        //    //DateOfBrith = DateTime.Now;
        //    //Gender = Gender.Female;
        //    //DepartmentId = 1;
        //    //PhotoPath = "";
        //}
    }
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    // Department Class

    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public Department( int iDepartmentId, string iDepartmentName )
        {
            DepartmentId = iDepartmentId;
            DepartmentName = iDepartmentName;
        }

    }
}
