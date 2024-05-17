using System;
using System.ComponentModel.DataAnnotations;
using Pustok.Models;

namespace Pustok.ViewModels
{
    public class ProfileViewModel
    {

        public ProfileEdirViewModel ProfileEditVM { get; set; }
        public List<Order> Orders { get; set; }
    }

}

