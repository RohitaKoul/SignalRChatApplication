using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;
using Microsoft.AspNet.SignalR;

namespace SignalRChatApplication.Hubs
{
    public class ChatHub : Hub
    {
        static ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();

        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }

        public void SendToSpecific(string name, string message, string to)
        {
            Clients.Caller.addNewMessageToPage(name, message);
            Clients.Client(dic[to]).addNewMessageToPage(name, message);
        }

        public void Notify(string name, string id)
        {
            if (dic.ContainsKey(name))
            {
                Clients.Caller.differentName();
            }
            else
            {
                dic.TryAdd(name, id);
                foreach (KeyValuePair<string, string> entry in dic)
                {
                    Clients.Caller.online(entry.Key);
                }
                Clients.Others.enters(name);
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var name = dic.FirstOrDefault(t => t.Value == Context.ConnectionId.ToString());
            if (!name.Equals(default(KeyValuePair<string, string>)))
            {
                string message;
                dic.TryRemove(name.Key, out message);
                return Clients.All.disconnected(name.Key);
            }
            else
            {
                return base.OnDisconnected(stopCalled);
            }
        }
    }
}