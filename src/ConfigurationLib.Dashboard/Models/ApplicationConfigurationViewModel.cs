using ConfigurationLib.Models.Dtos;
using System.Collections.Generic;

namespace ConfigurationLib.Dashboard.Models
{
    public class ApplicationConfigurationViewModel
    {
        public List<ApplicationConfigurationDto> Listing { get; set; }

        public ApplicationConfigurationDto Editing { get; set; }

        public ApplicationConfigurationDto Creating { get; set; }
    }
}
