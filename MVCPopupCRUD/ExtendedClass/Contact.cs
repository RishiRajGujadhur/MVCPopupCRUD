using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCPopupCRUD
{
    //You apply the MetadataType attribute to your main class file, specifying the type of the class that stores the validation attributes you want to apply to your main class members. You must define this as partial class, as shown here. 
    [MetadataType(typeof(ContactMetaData))]
    //Partial Classes and Methods (C# Programming Guide) Visual Studio 2015. It is possible to split the definition of a class or a struct, an interface or a method over two or more source files. Each source file contains a section of the type or method definition, and all parts are combined when the application is compiled.
    public partial class Contact
    {
        public string CountryName { get; set; }
        public string StateName { get; set; }
    }
    public class ContactMetaData
    {
        [Required(ErrorMessage = "Contact Name required", AllowEmptyStrings = false)]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [Required(ErrorMessage = "Contact No required", AllowEmptyStrings = false)]
        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }

        [Required(ErrorMessage = "Country required")]
        [Display(Name = "Country")]
        public int CountryID { get; set; }

        [Required(ErrorMessage = "State required")]
        [Display(Name = "State")]
        public int StateID { get; set; }
    }
}