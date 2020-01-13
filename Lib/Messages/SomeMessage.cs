using System;

namespace Lib.Messages
{
    public class SomeMessage
    {
        public Guid RequestId { get; set; }

        public string Reference { get; set; }

        public DateTime CreationDate { get; set; }
    }
}