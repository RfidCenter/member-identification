using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MemberIdentification
{
    public class PhraseChooser
    {
        private static readonly PhraseChooser _instance;

        private readonly Random _random = new Random();
        private string[] _phrases = null;

        static PhraseChooser()
        {
            _instance = new PhraseChooser();
        }

        private PhraseChooser()
        {
        }

        public static PhraseChooser Instance
        {
            get { return _instance; }
        }

        public string GeneratePhrase(PersonalCardRecord card)
        {
            if (this._phrases == null)
            {
                var lines = File.ReadAllLines(@"resources\phrases.txt");
                this._phrases = (from line in lines
                                 where !string.IsNullOrWhiteSpace(line)
                                 select line).ToArray();
            }

            var index = this._random.Next(0, this._phrases.Length - 1);

            return string.Format(CultureInfo.InvariantCulture,
                                 this._phrases[index],
                                 card.Initials,
                                 card.Occupation,
                                 card.Company,
                                 card.City,
                                 card.SawTimes);
        }
    }
}