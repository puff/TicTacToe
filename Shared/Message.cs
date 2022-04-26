using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Shared
{
    public enum Message
    {
        // Server Messages
        Start, // Start game on both clients
        Move, // Alternately notify clients to make their move
        End, // End game with winner on both clients

        // Client Messages
        SendMove, // Sent to server, a client has made their move
        Rematch, // Sent to server, must be accepted by other client
        AcceptRematch, // Sent to server by client who did not initiate rematch
        DenyRematch,  // Sent to server by client who did not initiate rematch  
    }
}
