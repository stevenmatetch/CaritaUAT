using System;
using System.Collections.Generic;
using System.Text;

namespace CaritaUAT.Models
{
    public enum MenuItemType
    {
        Home,
        HealthCards,
        Admin,
        About
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
