
namespace chatroom
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.IO;
    using System.Threading;
    using System.Collections;
    using System.Collections.Generic;

    public class Chatroom
    {
        private Hashtable chatrooms = new Hashtable();

        // create a new chatroom using a chatroom name
        public bool CreateChatroom(string chatroomName)
        {
            try
            {
                chatrooms.Add(chatroomName, new List<string>());
                return true;
            }
            catch (ArgumentException e) { return false; }
        }

        // list all chatrooms
        public string ListChatrooms()
        {
            string keylist = "";

            ICollection keys = chatrooms.Keys;

            foreach (var k in keys)
            {
                keylist = k.ToString() + " --- " + keylist;
            }

            return keylist;
        }

        // put a new message in the chatroom of chatroomName, using the name that sent it and the chat
        public bool SendChat(string Name, string chatroomName, string Chat) {
            try {
                //if doesnt work, try to cast chatroom to list<string>
                (chatrooms[chatroomName] as List <string>).Add(Name + " : " + Chat);   
                return true;    
            } catch (Exception e) {
                return false;
            }
        }

        //get a chatroom given the name of the chatroom
        public List <string> getChatroom(string chatroomName) 
        {
            List <string> chats = chatrooms[chatroomName] as List <string>;

            return chats;
        }



    }



    class Worker
    {
        TcpClient client;
        Chatroom myChatroom;


        public Worker(TcpClient Client, Chatroom MyChatroom)
        {
            client = Client;
            myChatroom = MyChatroom;

            new Thread(new ThreadStart(HandleRequest2)).Start();
        }

        //given a list of strings - send each string as a line in the chatroom seperatly, then send 567 once all the chats have been sent
        public void sendChatroom(List<string> chatroom, StreamWriter writer) {
            foreach (string c in chatroom)
            {
                writer.WriteLine(c);
                writer.Flush();
            };
            //567 is the code used to identified the end of sending a chatroom
            writer.WriteLine("567");
            writer.Flush();
            
        }

        public void HandleRequest2()
        {

            bool loop = true;

            // get streams
            StreamReader reader = new StreamReader(client.GetStream());
            StreamWriter writer = new StreamWriter(client.GetStream());
            

            
           


            while (loop)
            {
                {
                    string command = reader.ReadLine();
                    Console.WriteLine(command);

                    // command 1 is creating a chatroom
                    if (command == "1")
                    {
                        string chatroomName = reader.ReadLine();
                        myChatroom.CreateChatroom(chatroomName);
                    }
                    // command 2 is listing all the chatrooms
                    if (command == "2")
                    {
                        writer.WriteLine(myChatroom.ListChatrooms());
                        writer.Flush();
                    }

                    // command 3  is joining a chatroom
                    if (command == "3")
                    {
                        
                        string Name = reader.ReadLine();
                        string chatroomName = reader.ReadLine();
                        sendChatroom(myChatroom.getChatroom(chatroomName),writer); //send chatroom when user joins chatroom
                        string message = "";

                        
                        
                        // while we are still in the chat
                        while (message != "exit") {
                            message = reader.ReadLine(); //get the users message

                            myChatroom.SendChat(Name, chatroomName, message); //add it to the chatroom
                            sendChatroom(myChatroom.getChatroom(chatroomName),writer); //send the new chatroom to the user
                            message = "";

                            
                            

                        }
                        

                    }
                    if (command == "4")
                    {
                        loop = false;
                    }
                    
                }
            }


            writer.Flush();
            client.Close();


        }




    }
    class Server
    {
        static void Main(string[] args)
        {
            Chatroom myChatroom = new Chatroom();

            Int32 port = 8080;
            IPAddress localAddr = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
             
            TcpListener server = new TcpListener(localAddr, port);
            server.Start();

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("connection");

                //create a new worker with the newly connected client and a chatroom
                new Worker(client, myChatroom);
            }
        }
    }


}

