using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattEland.AutomatingMyDog.Core
{
    public record AppMessage
    {
        public AppMessage(string message, MessageSource source)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Source = source;
        }

        public string Message { get;  }
        public MessageSource Source { get;  }
        public string? SpeakText { get; init; }
    }
}
