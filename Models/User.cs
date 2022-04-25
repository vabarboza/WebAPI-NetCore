using Microsoft.EntityFrameworkCore;
using NHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public int UserLevel { get; set; }

        public string RegisterIP { get; set; }

        [Timestamp]
        public string RegisterDate { get; set; }

    }
}
