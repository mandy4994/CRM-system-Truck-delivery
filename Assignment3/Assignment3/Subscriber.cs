using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;

namespace Assignment3
{
    public delegate void MessageReceivedDelegate(string message); //This delegate is needed for creating a new event, so its handlers know what arguments to take and what to return

    public class Subscriber
    {
        private readonly ISession _Session;
        private readonly ITopic _Topic;
        private readonly string _Destination; //This is the name of our queue, ***that we have to create in http://localhost:8161***
        private bool _Disposed = false;

        public Subscriber(ISession session, string destination)
        {
            _Session = session;
            _Destination = destination;
            _Topic = new ActiveMQTopic(_Destination);
        }

        public event MessageReceivedDelegate OnMessageReceived; //We create a new event by specifying a delegate that defines how its event handlers should look (which is to say, their type signature)
        public IMessageConsumer Consumer { get; private set; }
        public string ConsumerId { get; private set; } //This is an ID of the subscriber (arbitrary), and is not the same thing as the name of the topic
        public void Start(string consumerId)
        {
            ConsumerId = consumerId;
            Consumer = _Session.CreateDurableConsumer(_Topic, ConsumerId, null, false); //A durable consumer will receive messages that were sent when they weren't subscribed.
            Consumer.Listener += _Consumer_Listener; //The "Listener" event triggers every time we receive a message from ActiveMQ. The reason we need our own event in addition is that we need
            //to do some error checking (e.g. message not null) as well as to extract the data from our message (in this case, our data in the message is just a string.)
        }

        private void _Consumer_Listener(IMessage message)
        {
            if (message == null)
                throw new InvalidCastException();

            ITextMessage TextMessage = (ITextMessage)message; //Note: If our message is not a string this will cause an error
            if (OnMessageReceived != null) //Checking if an event is not equal to null checks if there are any event handlers present. If there aren't, there's no point triggering the event.
            {
                OnMessageReceived(TextMessage.Text); //Trigger our own event. All event handlers associated with this event will execute.
            }
        }

        public void Dispose()
        {
            if (_Disposed)
                return;
            if (Consumer != null)
            {
                Consumer.Close();
                Consumer.Dispose();
            }
            _Disposed = true;
        }
    }
}
