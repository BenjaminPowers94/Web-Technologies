using HMBPClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace HMBPSail.Models /*changed name space to be same as models*/
{
    [ModelMetadataTypeAttribute(typeof(MemberMetadata))]
    public partial class Member : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // first name
            FirstName = FirstName.Trim();
            FirstName = HMBPClassLibrary.HMBPValidations.HMBPCapitalize(FirstName);

            // last name
            LastName = LastName.Trim();
            LastName = HMBPClassLibrary.HMBPValidations.HMBPCapitalize(LastName);

            // spouse first name
            SpouseFirstName = SpouseFirstName.Trim();
            SpouseFirstName = HMBPClassLibrary.HMBPValidations.HMBPCapitalize(SpouseFirstName);

            // spouse last name
            SpouseLastName = SpouseLastName.Trim();
            SpouseLastName = HMBPClassLibrary.HMBPValidations.HMBPCapitalize(SpouseLastName);

            if (String.IsNullOrEmpty(SpouseFirstName))
            {
                SpouseFirstName = null;
            }
            else
            {
                SpouseFirstName = HMBPClassLibrary.HMBPValidations.HMBPCapitalize(SpouseFirstName.Trim());
            }
            if (String.IsNullOrEmpty(SpouseLastName))
            {
                SpouseLastName = null;
            }
            else
            {
                SpouseLastName = HMBPClassLibrary.HMBPValidations.HMBPCapitalize(SpouseLastName.Trim());
            }

            // street
            Street = Street.Trim();
            Street = HMBPClassLibrary.HMBPValidations.HMBPCapitalize(Street);

            if (String.IsNullOrEmpty(Street))
            {
                Street = null;
            }
            else
            {
                Street = HMBPClassLibrary.HMBPValidations.HMBPCapitalize(Street.Trim());
            }

            // city 
            City = City.Trim();
            City = HMBPClassLibrary.HMBPValidations.HMBPCapitalize(City);

            if (String.IsNullOrEmpty(City))
            {
                City = null;
            }
            else
            {
                City = HMBPClassLibrary.HMBPValidations.HMBPCapitalize(City.Trim());
            }

            // postal code
            if (PostalCode == null || PostalCode == "")
            {
                PostalCode = "";
            }
            else
            {
                PostalCode = PostalCode.Trim();
                if(HMBPClassLibrary.HMBPValidations.HMBPPostalCodeValidation(PostalCode))
                {
                    PostalCode = HMBPClassLibrary.HMBPValidations.HMBPPostalCodeFormat(PostalCode);
                }
                else
                {
                    string _postalCode = "";
                    _postalCode = PostalCode;
                    if(HMBPClassLibrary.HMBPValidations.HMBPZipCodeValidation(ref _postalCode))
                    {
                        PostalCode = _postalCode;
                    }
                    else
                    {
                        yield return new ValidationResult("error", new[] { "PostalCode" });
                    }
                }
                if(ProvinceCode == null)
                {
                    yield return new ValidationResult("error", new[] { "ProvinceCode" });
                }
            }

            // email
            if (string.IsNullOrEmpty(Email))
            {
                Email = null;
            }
            else // trim
            {
                Email = Email.Trim();
            }

            // comment
            if (string.IsNullOrEmpty(Comment))
            {
                Comment = null;
            }
            else // trim
            {
                Comment = Comment.Trim();
            }

            //home phone reformat
            HomePhone = HMBPClassLibrary.HMBPValidations.HMBPExtractDigits(HomePhone.Trim());
            if (HomePhone.Length != 10)
            {
                yield return new ValidationResult("The home phone can only contain 10 digits",
                                                                    new[] { nameof(HomePhone) });
            }
            else
            {
                HomePhone = HomePhone.Insert(3, "-").Insert(7, "-");
            }

            //validate Joined year
            if (YearJoined.HasValue)
            {
                if(YearJoined > DateTime.Now.Year)
                {
                    YearJoined = null;
                    yield return new ValidationResult("The year cant be in the future", new[] { nameof(YearJoined) });
                }
            }

            //Full name
            if (String.IsNullOrEmpty(SpouseFirstName) && String.IsNullOrEmpty(SpouseLastName))
            {
                FullName = LastName + ", " + FirstName;
            }
            else if (!String.IsNullOrEmpty(SpouseFirstName))
            {
                if (String.IsNullOrEmpty(SpouseLastName) || SpouseLastName == LastName)
                {
                    FullName = LastName + ", " + FirstName + " & " + SpouseFirstName;
                }
                else if (!String.IsNullOrEmpty(SpouseLastName))
                {
                    FullName = LastName + ", " + FirstName + " & " + SpouseLastName + ", " + SpouseFirstName;
                }
            }

            yield return ValidationResult.Success;
        }
    }

    public class MemberMetadata
    {
        public int MemberId { get; set; }
        public string FullName { get; set; }


        [Required] /*first and last name are required*/
        [Display(Name = "First Name")]

        public string FirstName { get; set; }

        [Required] /*first and last name are required*/
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Spouse First Name")] /*optional*/
        public string SpouseFirstName { get; set; }

        [Display(Name = "Spouse Last Name")] /*optional*/
        public string SpouseLastName { get; set; }

        [Display(Name = "Street Address")] /*optional*/
        public string Street { get; set; }

        public string City { get; set; }

        [Display(Name = "Province Code")] /*optional*/
        public string ProvinceCode { get; set; }

        [RegularExpression(@"^[A-Za-z]\d[A-Za-z] ?\d[A-Za-z]\d$")] 
        [Display(Name = "Postal Code")] /*optional*/
        public string PostalCode { get; set; }

        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$")] 
        [Display(Name = "Home Phone")] /*required*/
        [Required]
        public string HomePhone { get; set; }


        [DataType(DataType.EmailAddress)] /*optional*/
        public string Email { get; set; }

        [Display(Name = "Year Joined")] /*optional*/
        public int? YearJoined { get; set; }
        public string Comment { get; set; }

        [Display(Name = "Task Exempt")] /*optional*/
        public bool TaskExempt { get; set; }

        [Display(Name = "Use Canada Post")] /*optional*/
        public bool UseCanadaPost { get; set; }

        public Province ProvinceCodeNavigation { get; set; }
        public ICollection<Boat> Boat { get; set; }
        public ICollection<MemberTask> MemberTask { get; set; }
        public ICollection<Membership> Membership { get; set; }
    }
}
