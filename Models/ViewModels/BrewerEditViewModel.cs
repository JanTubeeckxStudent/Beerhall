using System;
using Beerhall.Models.Domain;

namespace Beerhall.Models.ViewModels
{
    public class BrewerEditViewModel
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public int? Turnover { get; set; }

        public BrewerEditViewModel()
        {

        }

        public BrewerEditViewModel(Brewer brewer) : this()
        {
            Name = brewer.Name;
            Street = brewer.Street;
            PostalCode = brewer.Location?.PostalCode;
            Turnover = brewer.Turnover;
        }
    }
}
