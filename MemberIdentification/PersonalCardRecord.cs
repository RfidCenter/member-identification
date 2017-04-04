using System.Collections.Generic;

using MemberIdentification.DatabaseCode;

namespace MemberIdentification
{
    public sealed class PersonalCardRecord
    {
        public PersonalCardRecord(Persona dbRecord)
        {
            this.Initials = dbRecord.Initials;
            this.Occupation = dbRecord.Occupation;
            this.Company = dbRecord.Company;
            this.City = dbRecord.City;
            this.SawTimes = dbRecord.SawTimes;

            var compAndOcc = new List<string>();
            if (!string.IsNullOrWhiteSpace(this.Occupation))
            {
                compAndOcc.Add(this.Occupation);
            }

            if (!string.IsNullOrWhiteSpace(this.Company))
            {
                compAndOcc.Add(this.Company);
            }

            this.CompanyAndOccupation = string.Join(", ", compAndOcc);
        }

        public string Initials { get; }

        public string Occupation { get; }

        public string Company { get; }

        public string City { get; }

        public int SawTimes { get; }

        public string CompanyAndOccupation { get; }
    }
}
