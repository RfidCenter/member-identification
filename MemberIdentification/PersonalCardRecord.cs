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
                compAndOcc.Add(this.Occupation);
            if (!string.IsNullOrWhiteSpace(this.Company))
                compAndOcc.Add(this.Company);
            this.CompanyAndOccupation = string.Join(", ", compAndOcc);
        }

        public string Initials { get; private set; }
        public string Occupation { get; private set; }
        public string Company { get; private set; }
        public string City { get; private set; }
        public int SawTimes { get; private set; }

        public string CompanyAndOccupation { get; private set; }
    }
}