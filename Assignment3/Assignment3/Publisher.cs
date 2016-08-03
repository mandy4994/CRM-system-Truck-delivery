using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;

namespace Assignment3
{
    public class Publisher
    {
        private bool _Disposed;
        private readonly ISession _Session;
        private readonly ITopic _Topic;

        public IMessageProducer Producer { get; private set; }
        public string DestinationName { get; private set; }

        public Publisher(ISession session, string topicName)
        {
            _Session = session;
            DestinationName = topicName;
            _Topic = new ActiveMQTopic(DestinationName);
            Producer = _Session.CreateProducer(_Topic);
        }

        public void SendMessage(string message)
        {
            if (_Disposed)
                throw new ObjectDisposedException("Object has been disposed");
            ITextMessage TextMessage = Producer.CreateTextMessage(message);
            Producer.Send(TextMessage);
        }

        public void Dispose()
        {
            if (_Disposed)
                return;
            Producer.Close();
            Producer.Dispose();
            _Disposed = true;
        }
    }
}
