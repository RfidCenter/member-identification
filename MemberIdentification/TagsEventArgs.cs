using System;

namespace MemberIdentification
{
    public sealed class TagsEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.EventArgs" /> class.
        /// </summary>
        public TagsEventArgs(string[] tags)
        {
            this.Tags = tags;
        }

        public string[] Tags { get; private set; }
    }
}