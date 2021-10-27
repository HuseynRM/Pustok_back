using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.ViewModels.AccountViewModel
{
    public class ResetpasswordVM
    {
        public string Id { get; set; }
        public string Token { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, DataType(DataType.Password)][Compare("Password")]
        public string Confirmpassword { get; set; }

    }
}     
