using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace TourBackend
{
    public class SyncActor : IActor
    {

        private Stopwatch stopwatch;

        protected string id { get; }
        public volatile SyncObject syncobject; // Object which syncs Unity and the Actor framework

        public SyncActor(string _id, SyncObject _syncobject)
        {
            id = _id;
            syncobject = _syncobject;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;
            switch (msg)
            {
                case WriteCurrentTourState w:
                    lock (syncobject.thisLock)
                    {
                        syncobject.SetTimeStamp(stopwatch.ElapsedMilliseconds);
                        syncobject.dict = new Dictionary<int, CodeObject>(w.dict);
                    }
                    context.Sender.Tell(new RespondWriteCurrentTourState(w.id));
                    break;
            }
            return Actor.Done;
        }

    }
}
