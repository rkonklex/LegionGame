using System;
using System.Collections.Generic;
using AwaitableCoroutine;
using Legion.Utils;

namespace Legion.Model.Types
{
    public class Message
    {
        public MessageType Type { get; set; }
        public MapObject MainObject { get; set; }
        public MapObject OtherObject { get; set; }

        private readonly CoroutineCompletionSource _completionSource = new();
        public Coroutine MessageClosed => _completionSource.Coroutine;
        public void NotifyMessageClosed() => _completionSource.SetCompleted();
    }
}