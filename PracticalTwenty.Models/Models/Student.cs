using PracticalTwenty.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalTwenty.Models.Models
{
    public class Student : IAuditable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

    }
}
