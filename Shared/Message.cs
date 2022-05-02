namespace TicTacToe.Shared
{
    public enum Message
    {
        Start, // Start game on both clients
        Move, // Alternately notify clients to make their move, also sent from client to notify server they've made their move
        End, // End game with winner or draw on both clients

        //SendMove, // Sent to server, a client has made their move
        //Rematch, // Sent to server, must be accepted by other client
        //AcceptRematch, // Sent to server by client who did not initiate rematch
        //DenyRematch,  // Sent to server by client who did not initiate rematch  
    }
}
