
import java.io.*;
import java.net.*;
import java.util.*;




class client{

    //This is a function that can be used to receive a chatroom correctly

    public static void receiveChatroom(BufferedReader br) {
        String chatline = "";

        //read the first line of the chatroom
        try{
            chatline = br.readLine();
        } catch (Exception e) { System.out.println("oops\n");}

        //while we have not reached the end of the chatroom (deliminated by the code 567), loop through each chatline and print it out
        while (!chatline.contains("567"))
        {
            
            System.out.println(chatline);
            try{
            chatline = br.readLine();
        } catch (IOException e) { System.out.println("oops\n");}
        
            
        }
    }



    public static void main(String[] Args) {

        Console console = System.console();
        try {

            boolean loop = true;

            Socket s = new Socket("mono", 8080);

            InputStreamReader isr;
            BufferedReader br;
            PrintStream ps;
            

            isr = new InputStreamReader(s.getInputStream());
            br = new BufferedReader(isr);
            ps = new PrintStream(s.getOutputStream());

            while (loop) {


                System.out.println("OPTIONS");
                System.out.println("1 Create Chatroom"); //create chatroom
                System.out.println("2 List Chatrooms"); // list existing chatrooms
                System.out.println("3 Join a Chatroom"); // join a chatroom (exit will be contained within this command)
                System.out.println("4 Exit");


                String command = console.readLine();
                 
                // ask for a chatroom name and send it to the server
                if (command.equals("1")) {
                    System.out.println("What is the name of the chatroom you want to create?\n");
                    String chatroomName = console.readLine();
                    ps.println("1");
                    ps.println(chatroomName);
                    ps.flush();
                }
                
                // ask for list of chatrooms from the server, display them
                if (command.equals("2")) {
                    ps.println("2");
                    ps.flush();
                    System.out.println(br.readLine());
                }
                
                // join a chatroom, continues until the user types exit
                if (command.equals("3")) {

                    System.out.println("What is the name of the chatroom you want to join?\n");
                    String chatroomName = console.readLine();
                    
                    System.out.println("What is your name?\n");

                    String Name = console.readLine();
                    ps.println("3");
                    ps.println(Name);
                    ps.println(chatroomName);

                    ps.flush();
                    boolean notExit = true;
                    String message = "";
                    

                    while (notExit) 
                    {
                        //receive the state of the chatroom (contains all the chats deliminated by 567) and print it to console
                        receiveChatroom(br);

                        System.out.println("Type a message (or type exit to leave):\n");

                        //blocks until the user types a message and presses enter
                        message = console.readLine();
                        
                        ps.println(message);
                        
                        if (message.contains("exit"))
                        {
                            notExit = false;
                        }

                        ps.flush();
                        message = "";

                    }


                }
                
                if (command.equals("4")) {
                    ps.println("4");
                    ps.flush();
                    loop = false;
                }
                
            }

            s.close();

        } catch (

        Exception E) {
            System.out.println("oops");
        }
        
    }

}
