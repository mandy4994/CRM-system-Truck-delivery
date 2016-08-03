using System;
using System.Collections.Generic;
using System.Linq;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace Assignment3
{
    public class Rebate_Processer
    {
        public WebForm1 SendingForm { get; private set; }

        public Rebate_Processer(WebForm1 sendingForm)
        {
            SendingForm = sendingForm;

            string TOPIC_NAME = "Assignment3";
            IConnectionFactory ConFac = new ConnectionFactory(@"tcp://localhost:61616");
            IConnection Con = ConFac.CreateConnection();
            Con.Start();
            ISession Ses = Con.CreateSession();
            Subscriber Sub = new Subscriber(Ses, TOPIC_NAME);
            Sub.OnMessageReceived += Sub_OnMessageReceived;
            Sub.Start("My Processor");
        }

        void Sub_OnMessageReceived(string message)
        {
            Label OutputLabel = (Label)SendingForm.FindControl("lblTestOutput"); //Note: When implementing multithreading in your assignment 3 using a label like this won't work! You can't access a control from a
            double rebate = 0.0;                                                                     //thread that doesn't own that control without jumping through some extra hoops
                                                                                 //Instead, in assignment 3, send a message back to your assignment 1 and handle any web form output there.
            StringReader StrReader = new StringReader(message);
            XmlTextReader Reader = new XmlTextReader(StrReader);
            while (Reader.Read())
            {

                //For assignment 3 here is where you calculate all your business rules, and then send a message back to assignment 1 when you're done (hint: you may need to create a second topic for the return messages)
                if (Reader.NodeType == XmlNodeType.Element)
                {
                    if (Reader.Name == "Class")
                    {
                        Reader.Read();
                        if (Reader.Value.Contains("gold")) rebate = 300;
                        if (Reader.Value.Contains("regular")) rebate = 200;
                    }

                }

            }
        }
    }
}
