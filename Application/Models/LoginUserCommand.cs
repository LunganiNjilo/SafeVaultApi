using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class LoginUserCommand
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
    }
}
