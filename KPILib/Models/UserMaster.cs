using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KPILib.Models
{
    public class UserMaster
    {
        public int UserID { get; set; }
        
        [Required(ErrorMessage = "Required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required")]
        public int RoleID { get; set; }

        [DataType(DataType.EmailAddress)]

        [Required(ErrorMessage = "Required")]

        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter correct email")]
        public string Email { get; set; } = "";

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Required")]

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string Mobile { get; set; }
        public bool IsDiscontinued { get; set; }
        public DateTime AddedOn { get; set; }
        public int AddedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class UserMasterResponse
    {
        public UserMaster userMasterObj { get; set; }
        public List<UserMaster> data { get; set; }
        public ResponseObj Response { get; set; }

        public UserMasterResponse()
        {
            this.userMasterObj = new UserMaster();
            this.data = new List<UserMaster>();
            this.Response = new ResponseObj();
        }
    }
}
